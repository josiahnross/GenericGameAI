using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer
{
    public class MonteCarloTree<T, T1> where T : ITurnBasedGame<T, T1>
        where T1 : struct
    {
        //List<MonteCarloNode<T, T1>> allNodes;
        public MonteCarloNode<T, T1> Root { get; private set; }
        Func<MonteCarloNode<T, T1>, bool, double, double> selectionFunction;
        public double ExplorationParam { get; }
        Func<ITurnBasedGame<T, T1>, Dictionary<int, T1>, Players, int> chooseMoveFunc;
        public MonteCarloTree(ITurnBasedGame<T, T1> game, Func<MonteCarloNode<T, T1>, bool, double, double> selectionFunction, double explorationParam, Func<ITurnBasedGame<T, T1>, Dictionary<int, T1>, Players, int> chooseMoveFunc)
        {
            this.chooseMoveFunc = chooseMoveFunc;
            ExplorationParam = explorationParam;
            this.selectionFunction = selectionFunction;
            Root = new MonteCarloNode<T, T1>(null, game, (-1, default(T1)), Players.YouOrFirst);
            //allNodes = new List<MonteCarloNode<T, T1>>();
            //allNodes.Add(Root);
        }
        public void RunMonteCarloSims(int amountOfSims, MonteCarloNode<T, T1> startNode = null)
        {
            if (startNode == null)
            {
                startNode = Root;
            }
            bool player1Perspective = true;
            for (int i = 0; i < amountOfSims; i++)
            {
                RunMonteCarloSim(startNode, player1Perspective);
                player1Perspective = !player1Perspective;
            }
        }
        private void RunMonteCarloSim(MonteCarloNode<T, T1> node, bool player1Perspective)
        {
            var newNode = Selection(node, player1Perspective);
            var contInfo = ContinueWithNode(newNode);
            if (contInfo.continueWithNode)
            {
                newNode = Expansion(newNode, player1Perspective);
                contInfo = ContinueWithNode(newNode);
                if (contInfo.continueWithNode && newNode.AvailableMoves.Count > 0 && !newNode.FullyExplored)
                {
                    var info = SimulationAndPartialBackprop(newNode, player1Perspective);
                    Backprop(newNode, info);
                }
                else
                {
                    if (newNode != null)
                    {
                        var info = GetGameInfo(contInfo.boardState, player1Perspective);
                        newNode.GameInfo += info;
                        Backprop(newNode, info);
                    }
                }
            }
            else
            {
                if (newNode != null)
                {
                    var info = GetGameInfo(contInfo.boardState, player1Perspective);
                    newNode.GameInfo += info;
                    Backprop(newNode, info);
                }
            }
        }

        public void TrimTree(MonteCarloNode<T, T1> newRoot)
        {
            Root = newRoot;
            Root.KillParent();
        }

        private MonteCarloNode<T, T1> Selection(MonteCarloNode<T, T1> node, bool player1Perspective)
        {
            if (node.Children.Count >= node.TotalAvialableMovesCount)
            {
                double bestVal = double.MinValue;
                int bestIndex = -1;
                Dictionary<int, T1> bestMoves = new Dictionary<int, T1>();
                foreach (var i in node.Children)
                {
                    if (!i.Value.FullyExplored)
                    {
                        double currentVal = selectionFunction.Invoke(i.Value, player1Perspective, ExplorationParam);
                        if (currentVal > bestVal)
                        {
                            bestMoves.Clear();
                            bestMoves.Add(i.Key, node.Children[i.Key].MoveIndex.move);
                            bestVal = currentVal;
                            bestIndex = i.Key;
                        }
                        else if (currentVal == bestVal)
                        {
                            bestMoves.Add(i.Key, node.Children[i.Key].MoveIndex.move);
                        }
                    }
                }
                if (bestIndex < 0)
                {
                    if (!node.FullyExplored)
                    {
                        node.FullyExplored = true;
                        node.AvailableMoves.Clear();
                        Backprop(node, new NodeGameInfo(0, 0, 0, 0));
                    }
                    return null;
                }
                if (bestMoves.Count > 1)
                {
                    bestIndex = chooseMoveFunc.Invoke(node.CurrentState, bestMoves, node.Player);
                }
                return Selection(node.Children[bestIndex], player1Perspective);
            }
            else
            {
                return node;
            }
        }

        private MonteCarloNode<T, T1> Expansion(MonteCarloNode<T, T1> node, bool player1Perspective)
        {
            List<int> availableMovesToRemove = new List<int>();
            foreach (var m in node.AvailableMoves)
            {
                if (!node.Children.ContainsKey(m.Key))
                {
                    var state = node.CurrentState.Copy();
                    var childMove = new GameMove<T1>(m.Value, node.Player);
                    state.MakeMove(childMove);
                    Players childPlayer = GetOtherPlayer(node.Player);
                    var child = new MonteCarloNode<T, T1>(node, state, (m.Key, m.Value), childPlayer);
                    //allNodes.Add(child);

                    node.Children.Add(m.Key, child);
                    BoardState childBoardState = child.CurrentState.CheckBoardState(childMove);
                    if (childBoardState != BoardState.Continue || child.TotalAvialableMovesCount == 0)
                    {
                        child.EndOfGame = true;
                        child.FullyExplored = true;
                        availableMovesToRemove.Add(m.Key);
                        if (node.AvailableMoves.Count == 0)
                        {
                            node.FullyExplored = true;
                        }
                        var info = GetGameInfo(childBoardState, player1Perspective);
                        node.GameInfo += info;
                        child.GameInfo += info;
                        Backprop(node, info);
                    }
                }
            }
            foreach (var i in availableMovesToRemove)
            {
                node.AvailableMoves.Remove(i);
            }
            if (node.AvailableMoves.Count > 0)
            {
                var move = chooseMoveFunc.Invoke(node.CurrentState, node.AvailableMoves, node.Player);
                return node.Children[move];
            }
            if (!node.FullyExplored)
            {
                node.FullyExplored = true;
                Backprop(node, new NodeGameInfo(0, 0, 0, 0));
            }
            return null;
        }

        private NodeGameInfo SimulationAndPartialBackprop(MonteCarloNode<T, T1> node, bool player1Perspective)
        {
            Players player = node.Player;
            var move = chooseMoveFunc.Invoke(node.CurrentState, node.AvailableMoves, player);
            ITurnBasedGame<T, T1> state;
            GameMove<T1> gameMove = new GameMove<T1>(node.AvailableMoves[move], player); ;
            MonteCarloNode<T, T1> child;
            if (!node.Children.ContainsKey(move))
            {
                state = node.CurrentState.Copy();
                state.MakeMove(gameMove);
                child = new MonteCarloNode<T, T1>(node, state, (move, gameMove.Move), GetOtherPlayer(player));
                //allNodes.Add(child);
                node.Children.Add(move, child);
            }
            else
            {
                child = node.Children[move];
                state = child.CurrentState;
            }
            BoardState boardState = state.CheckBoardState(gameMove);
            if (boardState == BoardState.Continue && child.TotalAvialableMovesCount != 0)
            {
                NodeGameInfo childGameInfo = SimulationAndPartialBackprop(child, player1Perspective);
                node.GameInfo += childGameInfo;
                if (child.FullyExplored)
                {
                    node.AvailableMoves.Remove(move);
                    if (node.AvailableMoves.Count == 0)
                    {
                        node.FullyExplored = true;
                    }
                }
                return childGameInfo;
            }
            else
            {
                child.EndOfGame = true;
                child.FullyExplored = true;
                node.AvailableMoves.Remove(move);
                if (node.AvailableMoves.Count == 0)
                {
                    node.FullyExplored = true;
                }

                var info = GetGameInfo(boardState, player1Perspective);
                node.GameInfo += info;
                child.GameInfo += info;
                return info;
            }
        }

        private NodeGameInfo GetGameInfo(BoardState boardState, bool player1Perspective)
        {
            NodeGameInfo info = new NodeGameInfo(0, 0, 0, 0);
            if (boardState != BoardState.IllegalMove)
            {
                if (player1Perspective)
                {
                    info.Player1AmountOfGames = 1;
                }
                else
                {
                    info.Player2AmountOfGames = 1;
                }
                if (boardState == BoardState.Win)
                {
                    info.Player1Wins = 1;
                }
                else if (boardState == BoardState.Loss)
                {
                    info.Player2Wins = 1;
                }
            }
            return info;
        }

        private void Backprop(MonteCarloNode<T, T1> node, NodeGameInfo gameInfo)
        {
            if (node.Parent != null)
            {
                if (node.FullyExplored)
                {
                    node.Parent.AvailableMoves.Remove(node.MoveIndex.index);
                    if (node.Parent.AvailableMoves.Count == 0)
                    {
                        node.Parent.FullyExplored = true;
                    }
                }
                node.Parent.GameInfo += gameInfo;
                Backprop(node.Parent, gameInfo);
            }
        }

        public static double UTCSelection(MonteCarloNode<T, T1> node, bool player1Perspective, double explotationParam)
        {
            double amountOfWins = node.GameInfo.Player1Wins;
            double amountOfGames = node.GameInfo.Player1AmountOfGames;
            double parentAmountOfGames = node.Parent.GameInfo.Player1AmountOfGames;
            if (!player1Perspective)
            {
                amountOfWins = node.GameInfo.Player2Wins;
                amountOfGames = node.GameInfo.Player2AmountOfGames;
                parentAmountOfGames = node.Parent.GameInfo.Player2AmountOfGames;
            }
            if (amountOfGames == 0)
            {
                return double.MaxValue;
            }

            return (amountOfWins / amountOfGames) + (explotationParam * Math.Sqrt(Math.Log(parentAmountOfGames) / amountOfGames));
        }

        public static Players GetOtherPlayer(Players player)
        {
            if (player == Players.OpponentOrSecond)
            {
                return Players.YouOrFirst;
            }
            else if (player == Players.YouOrFirst)
            {
                return Players.OpponentOrSecond;
            }
            return player;
        }

        public static (bool continueWithNode, BoardState boardState) ContinueWithNode(MonteCarloNode<T, T1> node)
        {
            bool continueWithNode = node != null && !node.EndOfGame;
            BoardState boardState = BoardState.IllegalMove;
            if (continueWithNode)
            {
                boardState = node.CurrentState.CheckBoardState(new GameMove<T1>(node.MoveIndex.move, GetOtherPlayer(node.Player)));
                continueWithNode &= node.Parent == null || boardState == BoardState.Continue;
            }
            return (continueWithNode, boardState);
        }
    }
}
