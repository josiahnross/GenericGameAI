using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer
{
    public class JustEvaluator<T, T1> : IEvaluateableTurnBasedGame<T, T1>
        where T : ITurnBasedGame<T, T1>
        where T1 : struct
    {
        IEvaluateableTurnBasedGame<T, T1> parentEval;
        Func<ITurnBasedGame<T, T1>, double> evaluateFunc;
        public JustEvaluator(Func<ITurnBasedGame<T, T1>, double> evaluateFunc, IEvaluateableTurnBasedGame<T, T1> parentEval)
        {
            if(evaluateFunc == null)
            {
                throw new NullReferenceException();
            }
            this.evaluateFunc = evaluateFunc;
            this.parentEval = parentEval;
        }

        public ITurnBasedGame<T, T1> Game { get { return parentEval.Game; } }

        public IEvaluateableTurnBasedGame<T, T1> CopyEInterface(bool copyEval = true)
        {
            IEvaluateableTurnBasedGame<T, T1> newParentEval = parentEval;
            if(copyEval)
            {
                newParentEval = parentEval.CopyEInterface(false);
            }
            return new JustEvaluator<T, T1>(evaluateFunc, newParentEval);
        }

        public IEvaluateableTurnBasedGame<T, T1> CopyWithNewState(ITurnBasedGame<T, T1> state, Players player)
        {
            return new JustEvaluator<T, T1>(evaluateFunc, parentEval.CopyWithNewState(state, player));
        }

        public double EvaluateCurrentState(Players player)
        {
            return evaluateFunc.Invoke(Game);
        }

        public double EvaluateCurrentState(ITurnBasedGame<T, T1> state, Players player)
        {
            return evaluateFunc.Invoke(state);
        }

        public void MakeMove(GameMove<T1> move, int moveIndex)
        {
        }

        public void Restart()
        {
        }
    }
}
