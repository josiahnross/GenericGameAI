using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer
{
    public struct BoardPosition
    {
        public int X { get; set; }
        public int Y { get; set; }
        public BoardPosition(int x, int y)
        {
            X = x;
            Y = y;
        }
        public static BoardPosition operator +(BoardPosition left, BoardPosition right)
        {
            return new BoardPosition(left.X + right.X, left.Y + right.Y);
        }
        public static BoardPosition operator -(BoardPosition left, BoardPosition right)
        {
            return new BoardPosition(left.X - right.X, left.Y - right.Y);
        }
        public static BoardPosition operator *(BoardPosition left, BoardPosition right)
        {
            return new BoardPosition(left.X * right.X, left.Y * right.Y);
        }
        public static BoardPosition operator /(BoardPosition left, BoardPosition right)
        {
            return new BoardPosition(left.X / right.X, left.Y / right.Y);
        }
        public static BoardPosition operator *(BoardPosition left, int right)
        {
            return new BoardPosition(left.X * right, left.Y * right);
        }
        public static BoardPosition operator /(BoardPosition left, int right)
        {
            return new BoardPosition(left.X / right, left.Y / right);
        }
        public static bool operator ==(BoardPosition left, BoardPosition right)
        {
            return left.X == right.X && left.Y == right.Y;
        }
        public static bool operator !=(BoardPosition left, BoardPosition right)
        {
            return !(left == right);
        }
        public override string ToString()
        {
            return X.ToString() + " , " + Y.ToString();
        }
    }
}
