using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer
{
    public class JustEvaluator : IEvaluateableTurnBasedGame
    {
        public IEvaluateableTurnBasedGame ParentEval;
        public Func<ITurnBasedGame, Players, double> EvaluateFunc;
        public JustEvaluator(Func<ITurnBasedGame, Players, double> evaluateFunc, IEvaluateableTurnBasedGame parentEval)
        {
            EvaluateFunc = evaluateFunc;
            ParentEval = parentEval;
        }

        public double? EvaluateCurrentState(Players player, int depth = -1)
        {
            throw new NotImplementedException();
        }

        public void Restart()
        {
            throw new NotImplementedException();
        }

        public void Stop(bool stop)
        {
            throw new NotImplementedException();
        }
        
        public IEvaluateableTurnBasedGame<T,T1> Cast<T,T1>()
            where T : ITurnBasedGame<T, T1>, new()
            where T1 : struct
        {
            return (JustEvaluator<T, T1>)this;
        }
    }
    public class JustEvaluator<T, T1> : IEvaluateableTurnBasedGame<T, T1>
        where T : ITurnBasedGame<T, T1>, new()
        where T1 : struct
    {
        public IEvaluateableTurnBasedGame<T, T1> ParentEval { get; set; }
        Func<ITurnBasedGame<T, T1>, Players, double> evaluateFunc;
        public JustEvaluator(Func<ITurnBasedGame<T, T1>, Players, double> evaluateFunc, IEvaluateableTurnBasedGame<T, T1> parentEval)
        {
            if (evaluateFunc == null)
            {
                throw new NullReferenceException();
            }
            this.evaluateFunc = evaluateFunc;
            this.ParentEval = parentEval;
        }

        public ITurnBasedGame<T, T1> Game { get { return ParentEval.Game; } }

        public IEvaluateableTurnBasedGame<T, T1> CopyEInterface(bool copyEval = true)
        {
            IEvaluateableTurnBasedGame<T, T1> newParentEval = ParentEval;
            if (copyEval)
            {
                newParentEval = ParentEval.CopyEInterface(false);
            }
            return new JustEvaluator<T, T1>(evaluateFunc, newParentEval);
        }

        public IEvaluateableTurnBasedGame<T, T1> CopyWithNewState(ITurnBasedGame<T, T1> state, Players player)
        {
            return new JustEvaluator<T, T1>(evaluateFunc, ParentEval.CopyWithNewState(state, player));
        }

        public double? EvaluateCurrentState(Players player, int depth = -1)
        {
            return evaluateFunc.Invoke(Game, player);
        }

        public double? EvaluateCurrentState(ITurnBasedGame<T, T1> state, Players player, int depth = -1)
        {
            return evaluateFunc.Invoke(state, player);
        }

        public void MakeMove(GameMove<T1> move, int moveIndex, bool justCheckedAvaliableMoves, bool evalMakeMove = true)
        {
            if (evalMakeMove)
            {
                ParentEval.MakeMove(move, moveIndex, justCheckedAvaliableMoves, false);
            }
        }
        public static explicit operator JustEvaluator<T, T1>(JustEvaluator m)
        {
            Func<ITurnBasedGame<T, T1>, Players, double> evaluateFunc = null;
            if (m.EvaluateFunc != null)
            {
                evaluateFunc = (g, p) => m.EvaluateFunc.Invoke(g, p);
            }
            return new JustEvaluator<T, T1>(evaluateFunc, (IEvaluateableTurnBasedGame<T, T1>)m.ParentEval);
        }
        public void Restart()
        {
        }

        public void Stop(bool stop)
        {
        }

        public IEvaluateableTurnBasedGame<T2, T11> Cast<T2, T11>()
            where T2 : ITurnBasedGame<T2, T11>, new()
            where T11 : struct
        {
            return (IEvaluateableTurnBasedGame<T2,T11>)this;
        }
    }
}
