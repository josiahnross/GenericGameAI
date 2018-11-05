using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer
{
    public class JustEvaluator<T, T1> : IEvaulator<T, T1>
        where T : ITurnBasedGame<T, T1>
        where T1 : struct
    {
        Func<ITurnBasedGame<T, T1>, double> evaluateFunc;
        public JustEvaluator(Func<ITurnBasedGame<T, T1>, double> evaluateFunc)
        {
            if(evaluateFunc == null)
            {
                throw new NullReferenceException();
            }
            this.evaluateFunc = evaluateFunc;
        }
        public IEvaulator<T, T1> Copy()
        {
            return new JustEvaluator<T, T1>(evaluateFunc);
        }

        public double Evaluate(ITurnBasedGame<T, T1> currentState)
        {
            return evaluateFunc.Invoke(currentState);
        }

        public void Init(T game, bool aiFirst)
        {
        }

        public void MakeMove(GameMove<T1> move, int moveIndex)
        {
        }

        public void Restart()
        {
        }
    }
}
