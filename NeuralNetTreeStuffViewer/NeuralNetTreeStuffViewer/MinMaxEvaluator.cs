using NeuralNetTreeStuffViewer.MinMaxAlg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer
{
    public class MinMaxEvaluator<T, T1> : IEvaluateableTurnBasedGame<T, T1>
        where T : ITurnBasedGame<T, T1>,new()
        where T1 : struct
    {
        public uint MakeMoveMinMaxDepth { get; set; }
        public IEvaluateableTurnBasedGame<T, T1> Evaluator { get; set; }
        public ITurnBasedGame<T, T1> Game { get; }
        IEvaluateableTurnBasedGame<T, T1> parentEval = null;
        public IEvaluateableTurnBasedGame<T, T1> ParentEval { get => parentEval; set { parentEval = value; } }

        protected MinMaxEvaluator(MinMaxEvaluator<T, T1> minMaxEval, ITurnBasedGame<T, T1> game)
        {
            Evaluator = minMaxEval.Evaluator.CopyEInterface(false);

            Game = game;
            MakeMoveMinMaxDepth = minMaxEval.MakeMoveMinMaxDepth;
        }

        public MinMaxEvaluator(T game, IEvaluateableTurnBasedGame<T, T1> evaulator, uint makeMoveMinMaxDepth = 5, string debugStringPath = null)
        {
            Evaluator = evaulator;
            if (game == null)
            { throw new NullReferenceException(); }
            Game = game;
            MakeMoveMinMaxDepth = makeMoveMinMaxDepth;
        }
        public IEvaluateableTurnBasedGame<T, T1> CopyEInterface(bool copyEval = true)
        {
            return new MinMaxEvaluator<T, T1>(this, Game.Copy());
        }

        public void Restart()
        {
            Evaluator.Restart();
        }

        public IEvaluateableTurnBasedGame<T, T1> CopyWithNewState(ITurnBasedGame<T, T1> state, Players player)
        {
            var copy = new MinMaxEvaluator<T, T1>(this, state);
            return copy;
        }

        public double? EvaluateCurrentState(Players player, int depth = -1)
        {
            MinMaxNode<T, T1> node = MinMaxAlgorithm<T, T1>.EvaluateMoves(MakeMoveMinMaxDepth, Evaluator, isMaximizer(player));
            return node.Value;
        }
        public double? EvaluateCurrentState(ITurnBasedGame<T, T1> state, Players player, int depth = -1)
        {
            var tempEval = Evaluator.CopyWithNewState(state, player);
            MinMaxNode<T, T1> node = MinMaxAlgorithm<T, T1>.EvaluateMoves(MakeMoveMinMaxDepth, tempEval, isMaximizer(player));
            return node.Value;
        }

        public (double value, int? moveKey) EvaluateCurrentStateAndGetMove(ITurnBasedGame<T, T1> state, Players player)
        {
            var tempEval = Evaluator.CopyWithNewState(state, player);
            MinMaxNode<T, T1> node = MinMaxAlgorithm<T, T1>.EvaluateMoves(MakeMoveMinMaxDepth, tempEval, isMaximizer(player));
            MinMaxNode<T, T1> nextMoveChild = null;
            foreach (var n in node.Children)
            {
                if (n.Value == node.Value)
                {
                    nextMoveChild = n;
                    break;
                }
            }
            int? moveKey = null;
            if(nextMoveChild != null && nextMoveChild.MoveIndex != null)
            {
                moveKey = nextMoveChild.MoveIndex.Value.Index;
            }
            return (node.Value, moveKey);
        }

        public static bool isMaximizer(Players player)
        {
            if (player == Players.YouOrFirst)
            {
                return true;
            }
            return false;
        }
        public void MakeMove(GameMove<T1> move, int moveIndex, bool justCheckedAvaliableMoves, bool evalMakeMove = true)
        {
            if (evalMakeMove)
            {
                Evaluator?.MakeMove(move, moveIndex, justCheckedAvaliableMoves, false);
            }
            Game.MakeMove(move);
            Game.CheckBoardState(move, justCheckedAvaliableMoves);
        }

        public void Stop(bool stop)
        {
            throw new NotImplementedException();
        }
        public IEvaluateableTurnBasedGame<T2, T11> Cast<T2, T11>()
           where T2 : ITurnBasedGame<T2, T11>, new()
           where T11 : struct
        {
            return (IEvaluateableTurnBasedGame<T2, T11>)this;
        }
    }
}
