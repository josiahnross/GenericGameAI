using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer
{
    public class MonteCarloEvaluator<T, T1> : IEvaluateableTurnBasedGame<T, T1>
        where T : ITurnBasedGame<T, T1>,new()
        where T1 : struct
    {
        MonteCarloTree<T, T1> tree;
        MonteCarloTree<T, T1> tempTree = null;
        public MonteCarloNode<T, T1> CurrentNode;
        Func<MonteCarloNode<T, T1>, bool, double, double> selectionFunction;
        Func<ITurnBasedGame<T, T1>, Dictionary<int, T1>, Players, (int key, ITurnBasedGame<T, T1> newBoardState)> chooseMoveFunc;
        double explorationParam;
        int startSimulations;
        int simulationsPerTurn;
        int maxDepth;
        bool checkForLoops;
        public ITurnBasedGame<T, T1> Game { get { return CurrentNode.CurrentState; } }
        IEvaluateableTurnBasedGame<T, T1> parentEval = null;
        public IEvaluateableTurnBasedGame<T, T1> ParentEval { get => parentEval; set { parentEval = value; } }
        public double DepthMultiplier { get; set; }
        private MonteCarloEvaluator(MonteCarloEvaluator<T, T1> other)
        {
            selectionFunction = other.selectionFunction;
            chooseMoveFunc = other.chooseMoveFunc;
            explorationParam = other.explorationParam;
            startSimulations = other.startSimulations;
            simulationsPerTurn = other.simulationsPerTurn;
            tree = other.tree;
            maxDepth = other.maxDepth;
            checkForLoops = other.checkForLoops;
            CurrentNode = other.CurrentNode;
            DepthMultiplier = other.DepthMultiplier;
        }

        public IEvaluateableTurnBasedGame<T, T1> CopyEInterface(bool copyEval = true)
        {
            return new MonteCarloEvaluator<T, T1>(this);
        }
        public MonteCarloEvaluator(Func<MonteCarloNode<T, T1>, bool, double, double> selectionFunction, double explorationParam,
            Func<ITurnBasedGame<T, T1>, Dictionary<int, T1>, Players, (int key, ITurnBasedGame<T, T1> newBoardState)> chooseMoveFunc, int startSimulations, int simulationsPerTurn,
            ITurnBasedGame<T, T1> game, int maxDepth, bool checkForLoops, double depthMultiplier = 0, Players startPlayer = Players.YouOrFirst)
        {
            this.selectionFunction = selectionFunction;
            this.chooseMoveFunc = chooseMoveFunc;
            this.explorationParam = explorationParam;
            this.startSimulations = startSimulations;
            this.simulationsPerTurn = simulationsPerTurn;
            this.maxDepth = maxDepth;
            this.checkForLoops = checkForLoops;
            DepthMultiplier = depthMultiplier;
            tree = new MonteCarloTree<T, T1>(game, selectionFunction, explorationParam, chooseMoveFunc, maxDepth, startPlayer);
            CurrentNode = tree.Root;
            tree.RunMonteCarloSims(startSimulations, checkForLoops, true, true, CurrentNode);
        }


        public IEvaluateableTurnBasedGame<T, T1> CopyWithNewState(ITurnBasedGame<T, T1> state, Players player)
        {
            var copy = new MonteCarloEvaluator<T, T1>(this);
            copy.tree = null;
            copy.CurrentNode = null;
            copy.tree = new MonteCarloTree<T, T1>(state, selectionFunction, explorationParam, chooseMoveFunc, maxDepth, player);
            copy.CurrentNode = copy.tree.Root;
            return copy;
        }

        public IEvaluateableTurnBasedGame<T, T1> Copy()
        {
            return new MonteCarloEvaluator<T, T1>(this);
        }

        public double? EvaluateCurrentState(Players player, int depth = -1)
        {
            if (depth >= 0)
            {
                CurrentNode.Depth = depth;
            }
            tree.RunMonteCarloSims(simulationsPerTurn, checkForLoops, false, player == Players.YouOrFirst, CurrentNode);
            if(tree.Stop)
            {
                return null;
            }
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
            double winDifference = CurrentNode.GameInfo.Player1Wins - CurrentNode.GameInfo.Player2Wins;
            Players winningPlayer = player;
            if (winDifference > 0)
            {
                winningPlayer = Players.YouOrFirst;
            }
            else if (winDifference < 0)
            {
                winningPlayer = Players.OpponentOrSecond;
            }
            return winDifference + Funcs.Clamp(GetBadValue(CurrentNode.GameInfo.Player1TotalDepth + CurrentNode.GameInfo.Player2TotalDepth, winningPlayer) * DepthMultiplier, -1, 1);
        }

        public double? EvaluateCurrentState(ITurnBasedGame<T, T1> state, Players player, int depth = -1)
        {
            tempTree = null;
            tempTree = new MonteCarloTree<T, T1>(state, selectionFunction, explorationParam, chooseMoveFunc, maxDepth, player);
            if (depth >= 0)
            {
                tempTree.Root.Depth = depth;
            }
            tempTree.RunMonteCarloSims(simulationsPerTurn, checkForLoops, false, player == Players.YouOrFirst, tempTree.Root);
            if (tempTree.Stop)
            {
                return null;
            }
            if (tempTree.Root.EndOfGame)
            {
                if (tempTree.Root.GameInfo.Player1Wins > tempTree.Root.GameInfo.Player2Wins)
                {
                    return double.MaxValue;
                }
                if (tempTree.Root.GameInfo.Player2Wins > tempTree.Root.GameInfo.Player1Wins)
                {
                    return double.MinValue;
                }
                return 0;
            }
            double winDifference = tempTree.Root.GameInfo.Player1Wins - tempTree.Root.GameInfo.Player2Wins;
            Players winningPlayer = player;
            if (winDifference > 0)
            {
                winningPlayer = Players.YouOrFirst;
            }
            else if (winDifference < 0)
            {
                winningPlayer = Players.OpponentOrSecond;
            }
            double depthStuff = tempTree.Root.GameInfo.Player1TotalDepth + tempTree.Root.GameInfo.Player2TotalDepth;
            tempTree = null;
            return winDifference + Funcs.Clamp(GetBadValue(depthStuff, winningPlayer) * DepthMultiplier, -1, 1);
        }


        double GetBadValue(double value, Players player)
        {
            if (player == Players.YouOrFirst)
            {
                return -value;
            }
            else
            {
                return value;
            }
        }

        public void MakeMove(GameMove<T1> move, int moveIndex, bool justCheckedAvaliableMoves, bool evalMakeMove = true)
        {
            if (CurrentNode.Children.ContainsKey(moveIndex))
            {
                CurrentNode = CurrentNode.Children[moveIndex];
            }
            else if (CurrentNode.AvailableMoves.ContainsKey(moveIndex))
            {
                while (!CurrentNode.Children.ContainsKey(moveIndex))
                {
                    tree.RunMonteCarloSims(1, checkForLoops, true, true, CurrentNode);
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

        public void Stop(bool stop)
        {
            if (tempTree != null)
            {
                tempTree.Stop = stop;
            }
            else
            {
                tree.Stop = stop;
            }
        }
        public IEvaluateableTurnBasedGame<T2, T11> Cast<T2, T11>()
           where T2 : ITurnBasedGame<T2, T11>, new()
           where T11 : struct
        {
            return (IEvaluateableTurnBasedGame<T2, T11>)this;
        }
    }
}
