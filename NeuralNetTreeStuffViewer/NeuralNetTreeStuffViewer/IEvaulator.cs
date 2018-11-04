using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer
{
    public interface IEvaulator<T, T1>
        where T : ITurnBasedGame<T, T1>
        where T1 : struct
    {
        void Init(T game, bool aiFirst);
        IEvaulator<T, T1> Copy();
        void MakeMove(GameMove<T1> move, int moveIndex);
        double Evaluate(ITurnBasedGame<T, T1> currentState);
        void Restart();
    }
}
