using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer
{
    public interface IEvaluateableTurnBasedGame<T, T1>
    {
        ITurnBasedGame<T, T1> Game { get; }
        IEvaluateableTurnBasedGame<T, T1> CopyEInterface();
        double EvaluateCurrentState();
        void MakeMove(GameMove<T1> move, int moveIndex);
        void Restart();
    }
}
