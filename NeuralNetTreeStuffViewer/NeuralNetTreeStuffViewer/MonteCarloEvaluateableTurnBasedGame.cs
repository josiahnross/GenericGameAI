using NeuralNetTreeStuffViewer.MinMaxAlg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer
{
    public class MonteCarloEvaluateableTurnBasedGame<T, T1> : IEvaluateableTurnBasedGame<T, T1>
        where T : ITurnBasedGame<T, T1>
        where T1 : struct
    {
        MonteCarloTree<T, T1> tree;
        public MonteCarloNode<T, T1> CurrentNode { get; private set; }
        int simulationsPerTurn;
        public bool AiFirst { get; private set; }
        public ITurnBasedGame<T, T1> Game { get { return CurrentNode.CurrentState; } }
        public string DebugStringPath { get; set; }
        public T DisplayGame { get; }
        public uint MakeMoveMinMaxDepth { get; set; }
        private MonteCarloEvaluateableTurnBasedGame(MonteCarloEvaluateableTurnBasedGame<T, T1> evaluator)
        {
            tree = evaluator.tree;
            CurrentNode = evaluator.CurrentNode;
            simulationsPerTurn = evaluator.simulationsPerTurn;
            AiFirst = evaluator.AiFirst;
            DebugStringPath = evaluator.DebugStringPath;
            DisplayGame = evaluator.DisplayGame;
            MakeMoveMinMaxDepth = evaluator.MakeMoveMinMaxDepth;
        }
        public MonteCarloEvaluateableTurnBasedGame(T startBoard, Func<MonteCarloNode<T, T1>, bool, double, double> selectionFunction, double explorationParam, Func<ITurnBasedGame<T, T1>, Dictionary<int, T1>, Players, int> chooseMoveFunc,
            int startSimulations, int simulationsPerTurn, bool aiFirst, T displayGame, uint makeMoveMinMaxDepth = 5, string debugStringPath = null)
        {
            tree = new MonteCarloTree<T, T1>(startBoard, selectionFunction, explorationParam, chooseMoveFunc);
            CurrentNode = tree.Root;
            this.simulationsPerTurn = simulationsPerTurn;
            AiFirst = aiFirst;
            DebugStringPath = debugStringPath;
            DisplayGame = displayGame;
            MakeMoveMinMaxDepth = makeMoveMinMaxDepth;

            tree.RunMonteCarloSims(startSimulations, CurrentNode);
            if (DisplayGame != null)
            {
                displayGame.MoveMade += MoveMadeAndResponse;
                if (AiFirst)
                {
                    AIMakeMove();
                }
            }
        }

        private async void MoveMadeAndResponse(object sender, GameButtonArgs<(GameMove<T1> move, bool done)> e)
        {
            await Task.Run(() =>
            {
                if (!e.Info.done)
                {
                    int moveIndex = -1;
                    foreach (var i in CurrentNode.Children)
                    {
                        if (i.Value.MoveIndex.move.Equals(e.Info.move.Move))
                        {
                            moveIndex = i.Key;
                            break;
                        }
                    }
                    if (moveIndex >= 0)
                    {
                        MakeMove(e.Info.move, moveIndex);
                    }
                    AIMakeMove();
                }
            });
        }

        T1? AIMakeMove()
        {
            if (DisplayGame != null)
            {
                var node = MinMaxAlgorithm<T, T1>.EvaluateMoves(MakeMoveMinMaxDepth, this, AiFirst);
                MinMaxNode<T, T1> nextMoveChild = null;
                foreach (var n in node.Children)
                {
                    if (n.Value == node.Value)
                    {
                        nextMoveChild = n;
                        break;
                    }
                }
                if (nextMoveChild != null)
                {
                    if (DebugStringPath != null)
                    {
                        File.WriteAllText(DebugStringPath, nextMoveChild.Parent.DebugString);
                    }
                    var evaluator = (MonteCarloEvaluateableTurnBasedGame<T, T1>)nextMoveChild.CurrentState;
                    MakeMove(new GameMove<T1>(evaluator.CurrentNode.MoveIndex.move, AiFirst ? Players.YouOrFirst : Players.OpponentOrSecond), evaluator.CurrentNode.MoveIndex.index);
                    if (DisplayGame != null)
                    {
                        DisplayGame.ComputerMakeMove(evaluator.CurrentNode.MoveIndex.move);
                    }
                    Players humanPlayer = AiFirst ? Players.OpponentOrSecond : Players.YouOrFirst;
                    var moves = Game.AvailableMoves(humanPlayer);
                    if (moves.Count == 1 && moves.ContainsKey(-1))
                    {
                        T1 move = moves[-1];
                        DisplayGame.ComputerMakeMove(move);
                        MakeMove(new GameMove<T1>(move, humanPlayer), -1);
                        return AIMakeMove();
                    }
                    return evaluator.CurrentNode.MoveIndex.move;
                }
            }
            return null;
        }


        public IEvaluateableTurnBasedGame<T, T1> CopyEInterface()
        {
            return new MonteCarloEvaluateableTurnBasedGame<T, T1>(this);
        }

        public void MakeMove(GameMove<T1> move, int moveIndex)
        {
            if (CurrentNode.Children.ContainsKey(moveIndex))
            {
                CurrentNode = CurrentNode.Children[moveIndex];
            }
            else if (CurrentNode.AvailableMoves.ContainsKey(moveIndex))
            {
                while (!CurrentNode.Children.ContainsKey(moveIndex))
                {
                    tree.RunMonteCarloSims(1, CurrentNode);
                }
                CurrentNode = CurrentNode.Children[moveIndex];
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
            tree.TrimTree(CurrentNode);
        }

        public void Restart()
        {
            CurrentNode = tree.Root;
        }

        public double EvaluateCurrentState()
        {
            if (typeof(T) == typeof(Checkers))
            {
                Checkers checkers = (Checkers)Game;
                return checkers.AmountOfFirstPlayerCheckers - checkers.AmountOfSecondPlayerCheckers;
            }
            else
            {
                tree.RunMonteCarloSims(simulationsPerTurn, CurrentNode);
                if (CurrentNode.EndOfGame)
                {
                    if (CurrentNode.GameInfo.Player1Wins > CurrentNode.GameInfo.Player2Wins)
                    {
                        return double.MaxValue;
                    }
                    if (CurrentNode.GameInfo.Player2Wins > CurrentNode.GameInfo.Player1Wins)
                    {
                        return double.MinValue;
                    }
                    return 0;
                }
                return (CurrentNode.GameInfo.Player1Wins - CurrentNode.GameInfo.Player2Wins);
            }
        }
        public override string ToString()
        {
            return Game.ToString();
        }


    }
}
