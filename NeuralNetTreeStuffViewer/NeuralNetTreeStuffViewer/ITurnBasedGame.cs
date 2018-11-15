using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeuralNetTreeStuffViewer
{
    public interface ITurnBasedGame<TSelf, TMove>
    {
        TSelf Game { get; }
        TSelf Copy();
        ITurnBasedGame<TSelf, TMove> CopyInterface();
        void Copy(TSelf newBoard);
        void Restart();
        BoardState PlayerMakeMove(GameMove<TMove> move);
        bool IsLegalMove(GameMove<TMove> move);

        void MakeMove(GameMove<TMove> move);

        BoardState CheckBoardState(GameMove<TMove> lastMove);
        BoardState CheckBoardState();

        Dictionary<int, TMove> AvailableMoves(Players player);
        
        void DisplayGame(Panel panel);
        void EnableDisplay(bool enable);
        void ComputerMakeMove(TMove move);
        int GetMoveUniqueIdentifier(TMove move);
        event EventHandler<GameButtonArgs<(GameMove<TMove> move, bool done)>> MoveMade;
        double[] GetInputs(Players currentPlayer);
        void InitializeStaticVariables();
        void DeserializeInit();
        bool BoardEquals(ITurnBasedGame<TSelf, TMove> other);
        int TotalAmountOfMoves { get; }
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
