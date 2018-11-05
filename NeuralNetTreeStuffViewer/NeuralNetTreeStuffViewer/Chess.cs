using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer
{
    public class Chess
    {
        
    }
    public class ChessPiece
    {
        public BoardPosition Position { get; set; }
        public Players Player { get; set; }
        public ChessPieces Piece { get; set; }
        public ChessPiece(BoardPosition pos, Players player, ChessPieces piece)
        {
            Position = pos;
            Player = player;
            Piece = piece;
        }
    }
    public enum ChessPieces
    {
        Pawn,
        Knight,
        Bishop,
        Rook,
        Queen,
        King
    }
}
