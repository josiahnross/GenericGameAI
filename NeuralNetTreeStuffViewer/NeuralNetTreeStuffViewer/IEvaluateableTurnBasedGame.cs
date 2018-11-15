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
        IEvaluateableTurnBasedGame<T, T1> CopyEInterface(bool copyEval = true);
        double EvaluateCurrentState(Players player);
        double EvaluateCurrentState(ITurnBasedGame<T, T1> state, Players player);
        IEvaluateableTurnBasedGame<T, T1> CopyWithNewState(ITurnBasedGame<T, T1> state, Players player);
        void MakeMove(GameMove<T1> move, int moveIndex, bool evalMakeMove = true);
        void Restart();
    }
}
