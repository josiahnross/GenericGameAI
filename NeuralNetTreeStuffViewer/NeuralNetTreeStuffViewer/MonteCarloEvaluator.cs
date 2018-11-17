using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer
{
    public class MonteCarloEvaluator<T, T1> : IEvaluateableTurnBasedGame<T, T1>
        where T : ITurnBasedGame<T, T1>
        where T1 : struct
    {
        MonteCarloTree<T, T1> tree;
        public MonteCarloNode<T, T1> CurrentNode;
        Func<MonteCarloNode<T, T1>, bool, double, double> selectionFunction;
        Func<ITurnBasedGame<T, T1>, Dictionary<int, T1>, Players, int> chooseMoveFunc;
        double explorationParam;
        int startSimulations;
        int simulationsPerTurn;
        int maxDepth;
        bool checkForLoops;
        public ITurnBasedGame<T, T1> Game { get { return CurrentNode.CurrentState; } }

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
        }

        public IEvaluateableTurnBasedGame<T, T1> CopyEInterface(bool copyEval = true)
        {
            return new MonteCarloEvaluator<T, T1>(this);
        }
        public MonteCarloEvaluator(Func<MonteCarloNode<T, T1>, bool, double, double> selectionFunction, double explorationParam,
            Func<ITurnBasedGame<T, T1>, Dictionary<int, T1>, Players, int> chooseMoveFunc, int startSimulations, int simulationsPerTurn, 
            ITurnBasedGame<T, T1> game, int maxDepth, bool checkForLoops, Players startPlayer = Players.YouOrFirst)
        {
            this.selectionFunction = selectionFunction;
            this.chooseMoveFunc = chooseMoveFunc;
            this.explorationParam = explorationParam;
            this.startSimulations = startSimulations;
            this.simulationsPerTurn = simulationsPerTurn;
            this.maxDepth = maxDepth;
            this.checkForLoops = checkForLoops;
            tree = new MonteCarloTree<T, T1>(game, selectionFunction, explorationParam, chooseMoveFunc, maxDepth, startPlayer);
            CurrentNode = tree.Root;
            tree.RunMonteCarloSims(startSimulations, checkForLoops, CurrentNode);
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

        public double EvaluateCurrentState(Players player)
        {
            tree.RunMonteCarloSims(simulationsPerTurn, checkForLoops, CurrentNode);
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

        public double EvaluateCurrentState(ITurnBasedGame<T, T1> state, Players player)
        {
            MonteCarloTree<T, T1> stateTree = new MonteCarloTree<T, T1>(state, selectionFunction, explorationParam, chooseMoveFunc, maxDepth, player);
            stateTree.RunMonteCarloSims(simulationsPerTurn, checkForLoops, stateTree.Root);
            if (stateTree.Root.EndOfGame)
            {
                if (stateTree.Root.GameInfo.Player1Wins > stateTree.Root.GameInfo.Player2Wins)
                {
                    return double.MaxValue;
                }
                if (stateTree.Root.GameInfo.Player2Wins > stateTree.Root.GameInfo.Player1Wins)
                {
                    return double.MinValue;
                }
                return 0;
            }
            return (stateTree.Root.GameInfo.Player1Wins - stateTree.Root.GameInfo.Player2Wins);
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
                    tree.RunMonteCarloSims(1, checkForLoops, CurrentNode);
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

    }
}
