using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer
{
    public interface IEvaluateableTurnBasedGame<T, T1>: IEvaluateableTurnBasedGame
        where T : new()
    {
        IEvaluateableTurnBasedGame<T, T1> ParentEval { get; set; }
        ITurnBasedGame<T, T1> Game { get; }
        IEvaluateableTurnBasedGame<T, T1> CopyEInterface(bool copyEval = true);
        
        double? EvaluateCurrentState(ITurnBasedGame<T, T1> state, Players player, int depth = -1);
        IEvaluateableTurnBasedGame<T, T1> CopyWithNewState(ITurnBasedGame<T, T1> state, Players player);
        void MakeMove(GameMove<T1> move, int moveIndex, bool justCheckedAvaliableMoves, bool evalMakeMove = true);
    }
    public interface IEvaluateableTurnBasedGame
    {
        IEvaluateableTurnBasedGame<T, T1> Cast<T, T1>() 
            where T : ITurnBasedGame<T, T1>, new()
            where T1 : struct;

        void Stop(bool stop);
        double? EvaluateCurrentState(Players player, int depth = -1);
        void Restart();
    }
}
