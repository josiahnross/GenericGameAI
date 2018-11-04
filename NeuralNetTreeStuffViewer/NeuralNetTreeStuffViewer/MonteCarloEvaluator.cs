using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer
{
    public class MonteCarloEvaluator<T, T1> : IEvaulator<T, T1>
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

        public MonteCarloEvaluator(Func<MonteCarloNode<T, T1>, bool, double, double> selectionFunction, double explorationParam,
            Func<ITurnBasedGame<T, T1>, Dictionary<int, T1>, Players, int> chooseMoveFunc, int startSimulations, int simulationsPerTurn)
        {
            this.selectionFunction = selectionFunction;
            this.chooseMoveFunc = chooseMoveFunc;
            this.explorationParam = explorationParam;
            this.startSimulations = startSimulations;
            this.simulationsPerTurn = simulationsPerTurn;
        }
        public void Init(T game, bool aiFirst)
        {
            tree = new MonteCarloTree<T, T1>(game, selectionFunction, explorationParam, chooseMoveFunc);
            CurrentNode = tree.Root;
            tree.RunMonteCarloSims(startSimulations, CurrentNode);
        }

        public IEvaulator<T, T1> Copy()
        {
            var eval = new MonteCarloEvaluator<T, T1>(selectionFunction, explorationParam, chooseMoveFunc, startSimulations, simulationsPerTurn);
            eval.tree = tree;
            eval.CurrentNode = CurrentNode;
            return eval;
        }

        public double Evaluate(ITurnBasedGame<T, T1> currentState)
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
    }
}
