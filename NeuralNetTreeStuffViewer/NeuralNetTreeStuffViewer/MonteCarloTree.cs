using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        Func<ITurnBasedGame<T, T1>, Dictionary<int, T1>, Players, (int key, ITurnBasedGame<T, T1> newBoardState)> chooseMoveFunc;
        public int MaxDepth { get; }
        public MonteCarloTree(ITurnBasedGame<T, T1> game, Func<MonteCarloNode<T, T1>, bool, double, double> selectionFunction, double explorationParam,
            Func<ITurnBasedGame<T, T1>, Dictionary<int, T1>, Players, (int key, ITurnBasedGame<T, T1> newBoardState)> chooseMoveFunc, int maxDepth, Players startPlayer)
        {
            this.chooseMoveFunc = chooseMoveFunc;
            ExplorationParam = explorationParam;
            this.selectionFunction = selectionFunction;
            Root = new MonteCarloNode<T, T1>(null, game, (-1, default(T1)), startPlayer, 0);
            MaxDepth = maxDepth;
            //allNodes = new List<MonteCarloNode<T, T1>>();
            //allNodes.Add(Root);
        }
        public void RunMonteCarloSims(int amountOfSims, bool checkForLoops, MonteCarloNode<T, T1> startNode = null, HashSet<MonteCarloNode<T, T1>> nodesSet = null)
        {
            if (startNode == null)
            {
                startNode = Root;
            }
            if (nodesSet != null && !nodesSet.Contains(Root))
            {
                nodesSet.Add(Root);
            }
            bool player1Perspective = true;
            for (int i = 0; i < amountOfSims; i++)
            {
                RunMonteCarloSim(startNode, player1Perspective, nodesSet, checkForLoops);
                
                player1Perspective = !player1Perspective;
            }
        }
        
        private void RunMonteCarloSim(MonteCarloNode<T, T1> node, bool player1Perspective, HashSet<MonteCarloNode<T, T1>> nodesSet, bool checkForLoops)
        {
            var newNode = Selection(node, player1Perspective);
            var contInfo = ContinueWithNode(newNode, true);
            if (contInfo.continueWithNode)
            {
                newNode = Expansion(newNode, player1Perspective, nodesSet);
                contInfo = ContinueWithNode(newNode, true);
                if (contInfo.continueWithNode && newNode.AvailableMoves.Count > 0 && !newNode.FullyExplored)
                {
                    var info = SimulationAndPartialBackprop(newNode, player1Perspective, nodesSet, checkForLoops);
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
                    bestIndex = chooseMoveFunc.Invoke(node.CurrentState, bestMoves, node.Player).key;
                }
                return Selection(node.Children[bestIndex], player1Perspective);
            }
            else
            {
                return node;
            }
        }

        private MonteCarloNode<T, T1> Expansion(MonteCarloNode<T, T1> node, bool player1Perspective, HashSet<MonteCarloNode<T, T1>> nodesSet)
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
                    var child = new MonteCarloNode<T, T1>(node, state, (m.Key, m.Value), childPlayer, node.Depth + 1);
                    if (nodesSet != null)
                    {
                        nodesSet.Add(child);
                    }

                    node.Children.Add(m.Key, child);
                    BoardState childBoardState = child.CurrentState.CheckBoardState(childMove, true);
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
                return node.Children[move.key];
            }
            if (!node.FullyExplored)
            {
                node.FullyExplored = true;
                Backprop(node, new NodeGameInfo(0, 0, 0, 0));
            }
            return null;
        }
        private NodeGameInfo SimulationAndPartialBackprop(MonteCarloNode<T, T1> node, bool player1Perspective, HashSet<MonteCarloNode<T, T1>> nodesSet, bool checkForLoops)
        {
            Players player = node.Player;
            if (node.AvailableMoves.Count > 0)
            {
                var move = chooseMoveFunc.Invoke(node.CurrentState, node.AvailableMoves, player);
                ITurnBasedGame<T, T1> state;
                GameMove<T1> gameMove = new GameMove<T1>(node.AvailableMoves[move.key], player); ;
                MonteCarloNode<T, T1> child;
                if (!node.Children.ContainsKey(move.key))
                {
                    if (move.newBoardState == null)
                    {
                        state = node.CurrentState.Copy();
                        state.MakeMove(gameMove);
                    }
                    else
                    {
                        state = move.newBoardState;
                    }
                    child = new MonteCarloNode<T, T1>(node, state, (move.key, gameMove.Move), GetOtherPlayer(player), node.Depth + 1);
                    if (nodesSet != null)
                    {
                        nodesSet.Add(child);
                    }
                    node.Children.Add(move.key, child);
                }
                else
                {
                    child = node.Children[move.key];
                    state = child.CurrentState;
                }
                BoardState boardState = state.CheckBoardState(gameMove,true);
                if (boardState == BoardState.Continue && child.TotalAvialableMovesCount != 0 && child.Depth < MaxDepth)
                {
                    if (node.Parent == null || !checkForLoops || !IsLoop(child, node.Parent))
                    {
                        NodeGameInfo childGameInfo = SimulationAndPartialBackprop(child, player1Perspective, nodesSet, checkForLoops);
                        node.GameInfo += childGameInfo;
                        if (child.FullyExplored)
                        {
                            node.AvailableMoves.Remove(move.key);
                            if (node.AvailableMoves.Count == 0)
                            {
                                node.FullyExplored = true;
                            }
                        }
                        return childGameInfo;
                    }
                }
                child.AvailableMoves.Clear();
                child.Children.Clear();
                child.ClearMoves();
                child.EndOfGame = true;
                child.FullyExplored = true;
                node.AvailableMoves.Remove(move.key);
                if (node.AvailableMoves.Count == 0)
                {
                    node.FullyExplored = true;
                }

                var info = GetGameInfo(boardState, player1Perspective);
                node.GameInfo += info;
                child.GameInfo += info;
                return info;
            }
            else
            {
                node.EndOfGame = true;
                node.FullyExplored = true;
                
                var info = GetGameInfo(BoardState.Continue, player1Perspective);
                node.GameInfo += info;
                if (node.Parent != null)
                {
                    node.Parent.AvailableMoves.Remove(node.MoveIndex.index);
                    if (node.AvailableMoves.Count == 0)
                    {
                        node.FullyExplored = true;
                    }
                    node.Parent.GameInfo += info;
                }
                return info;
            }
            //else
            //{
            
            //}
        }

        bool IsLoop(MonteCarloNode<T, T1> node, MonteCarloNode<T, T1> currentNode)
        {
            if (node.CurrentState.BoardEquals(currentNode.CurrentState))
            {
                return true;
            }
            if (currentNode.Parent == null || currentNode.Parent.Parent == null)
            {
                return false;
            }
            return IsLoop(node, currentNode.Parent.Parent);
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

        public (bool continueWithNode, BoardState boardState) ContinueWithNode(MonteCarloNode<T, T1> node, bool justCheckedBoardState)
        {
            bool continueWithNode = node != null && !node.EndOfGame && node.Depth < MaxDepth;
            BoardState boardState = BoardState.IllegalMove;
            if (continueWithNode)
            {
                boardState = node.CurrentState.CheckBoardState(new GameMove<T1>(node.MoveIndex.move, GetOtherPlayer(node.Player)), justCheckedBoardState);
                continueWithNode &= node.Parent == null || boardState == BoardState.Continue;
            }
            return (continueWithNode, boardState);
        }
    }
}
