using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeuralNetTreeStuffViewer
{
    public interface ITurnBasedGame<T, T1>
    {
        T Game { get; }
        T Copy();
        ITurnBasedGame<T, T1> CopyInterface();
        void Copy(T newBoard);
        void Restart();
        BoardState PlayerMakeMove(GameMove<T1> move);
        bool IsLegalMove(GameMove<T1> move);

        void MakeMove(GameMove<T1> move);

        BoardState CheckBoardState(GameMove<T1> lastMove);

        Dictionary<int, T1> AvailableMoves(Players player);
        
        void DisplayGame(Panel panel);
        void EnableDisplay(bool enable);
        void ComputerMakeMove(T1 move);
        int GetMoveUniqueIdentifier(T1 move);
        event EventHandler<GameButtonArgs<(GameMove<T1> move, bool done)>> MoveMade;
    }
    public struct BoardInfo
    {
        public string Board { get; set; }
        public string BoardName { get; set; }
        public BoardInfo(string board, string boardName)
        {
            Board = board;
            BoardName = boardName;
        }
    }
}
