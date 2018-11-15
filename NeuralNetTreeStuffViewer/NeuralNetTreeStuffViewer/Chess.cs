using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer
{
    public class Chess// : ITurnBasedGame<Chess, CheckersMove>
    {
        static My2dArray<ChessPiece> startBoard = null;
        static Dictionary<ChessPieces, MoveInfo> pieceRanges;
        My2dArray<ChessPiece> board;
        HashSet<ChessPiece> firstPieces;
        HashSet<ChessPiece> secondPieces;
        ChessPiece firstKing;
        ChessPiece secondKing;
        public Chess()
        {
            if (startBoard == null)
            {
                InitStatic();
            }
            Restart();
        }

        void MakeMove(GameMove<ChessMove> move)
        {
            ChessPiece piece = board[move.Move.InitPosition];
            ChessMove genMove = move.Move;
            genMove.InitPosition = new BoardPosition(-1, -1);
            BoardPosition change = pieceRanges[piece.Piece].Moves[genMove].PositionChange;
            if(move.Player == Players.OpponentOrSecond)
            {
                change.Y *= -1;
            }
            BoardPosition oldPiecePos = piece.Position;
            BoardPosition newPos = change + piece.Position;
            piece.Position = newPos;
            if(board[newPos].Player != Players.None)
            {
                if(move.Player == Players.YouOrFirst)
                {
                    secondPieces.Remove(board[newPos]);
                }
                else
                {
                    firstPieces.Remove(board[newPos]);
                }
            }
            board[newPos] = null;
            board[newPos] = piece;
            board[oldPiecePos] = new ChessPiece(oldPiecePos, Players.None, ChessPieces.Pawn);
        }

        public bool InCheck(Players player)
        {
            ChessPiece king;
            HashSet<ChessPiece> opposingForces;
            Players opposingPlayer = Funcs.OppositePlayer(player);
            if (player == Players.YouOrFirst)
            {
                king = firstKing;
                opposingForces = secondPieces;
            }
            else
            {
                king = secondKing;
                opposingForces = firstPieces;
            }
            foreach(var piece in opposingForces)
            {
                BoardPosition difference = king.Position - piece.Position;
                if (opposingPlayer == Players.OpponentOrSecond)
                {
                    difference.Y *= -1;
                }
                if (pieceRanges[piece.Piece].ReverseMoves.ContainsKey(difference))
                {
                    var move = pieceRanges[piece.Piece].ReverseMoves[difference];
                    if (move.CanMakeMove(GetRelativePosition(piece.Position)))
                    {
                        if (move.CanJump)
                        {
                            return true;
                        }
                        BoardPosition minMove = new BoardPosition(difference.X / Math.Abs(difference.X), difference.Y / Math.Abs(difference.Y));
                        if (opposingPlayer == Players.OpponentOrSecond)
                        {
                            minMove.Y *= -1;
                        }
                        for (BoardPosition pos = piece.Position + minMove; true; pos += minMove)
                        {
                            if (board[pos].Player != Players.None)
                            {
                                if(board[pos] == king)
                                {
                                    return true;
                                }
                                break;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public static BoardPosition GetRelativePosition(BoardPosition position)
        {
            return new BoardPosition(startBoard.XLength - position.X - 1, startBoard.YLength - position.Y - 1);
        }

        public Dictionary<int, ChessMove> AvailableMoves(Players player)
        {

            throw new NotImplementedException();
        }

        public void Restart()
        {
            board = new My2dArray<ChessPiece>(startBoard.XLength, startBoard.YLength);
            firstPieces = new HashSet<ChessPiece>();
            secondPieces = new HashSet<ChessPiece>();
            for (int y = 0; y < startBoard.YLength; y++)
            {
                for (int x = 0; x < startBoard.XLength; x++)
                {
                    board[x, y] = startBoard[x, y].Copy();
                    if (board[x, y].Player == Players.YouOrFirst)
                    {
                        firstPieces.Add(board[x, y]);
                        if (board[x, y].Piece == ChessPieces.King)
                        {
                            firstKing = board[x, y];
                        }
                    }
                    else if (board[x, y].Player == Players.OpponentOrSecond)
                    {
                        secondPieces.Add(board[x, y]);
                        if (board[x, y].Piece == ChessPieces.King)
                        {
                            secondKing = board[x, y];
                        }
                    }
                }
            }
        }

        void InitStatic()
        {
            startBoard = new My2dArray<ChessPiece>(8, 8);
            ChessPieces[] firstRow = new ChessPieces[] {ChessPieces.Rook, ChessPieces.Knight, ChessPieces.Bishop,
                                                        ChessPieces.Queen, ChessPieces.King, ChessPieces.Bishop,
                                                        ChessPieces.Knight, ChessPieces.Rook};
            for (int y = 0; y < startBoard.YLength; y++)
            {
                for (int x = 0; x < startBoard.XLength; x++)
                {
                    Players player = Players.None;
                    if (y < 2)
                    {
                        player = Players.YouOrFirst;
                    }
                    else if (y >= startBoard.YLength - 2)
                    {
                        player = Players.OpponentOrSecond;
                    }

                    ChessPiece piece;
                    if (player != Players.None)
                    {
                        if (y == 0 || y == startBoard.YLength - 1)
                        {
                            piece = new ChessPiece(new BoardPosition(x, y), player, firstRow[x]);
                        }
                        else
                        {
                            piece = new ChessPiece(new BoardPosition(x, y), player, ChessPieces.Pawn);
                        }
                    }
                    else
                    {
                        piece = new ChessPiece(new BoardPosition(x, y), player, ChessPieces.Pawn);
                    }
                    startBoard[x, y] = piece;
                }
            }

            var pieceRangesLists = new Dictionary<ChessPieces, List<MoveInfo>>();
            pieceRangesLists.Add(ChessPieces.Queen, new List<MoveInfo>()
            {
                new MoveInfo(new List<Directions>(){Directions.Top, Directions.Right, Directions.Bottom, Directions.Left}, new BoardPosition(0, 1), new Range(1, 7), false),
                new MoveInfo(new List<Directions>(){Directions.TopRight, Directions.BottomLeft, Directions.BottomRight, Directions.TopLeft }, new BoardPosition(1, 1), new Range(1, 7), false)
            });
            pieceRangesLists.Add(ChessPieces.Rook, new List<MoveInfo>()
            {
                new MoveInfo(new List<Directions>(){Directions.Top, Directions.Right, Directions.Bottom, Directions.Left}, new BoardPosition(0, 1), new Range(1, 7), false)
            });
            pieceRangesLists.Add(ChessPieces.Bishop, new List<MoveInfo>()
            {
                new MoveInfo(new List<Directions>(){Directions.TopRight, Directions.BottomLeft, Directions.BottomRight, Directions.TopLeft }, new BoardPosition(1, 1), new Range(1, 7), false)
            });
            pieceRangesLists.Add(ChessPieces.Knight, new List<MoveInfo>()
            {
                new MoveInfo(new List<Directions>(){Directions.Top, Directions.Right, Directions.Bottom, Directions.Left}, new BoardPosition(1, 3), new Range(1, 1), true),
                new MoveInfo(new List<Directions>(){Directions.TopRight, Directions.BottomLeft, Directions.BottomRight, Directions.TopLeft }, new BoardPosition(3, 1), new Range(1, 1), true)
            });
            pieceRangesLists.Add(ChessPieces.Pawn, new List<MoveInfo>()
            {
                new MoveInfo(new List<Directions>(){Directions.Top }, new BoardPosition(0, 1), new Range(1, 1), false),
                new MoveInfo(new List<Directions>(){Directions.Top }, new BoardPosition(0, 1), new Range(2, 2), false, (p)=>p.Y == 1)
            });
            pieceRangesLists.Add(ChessPieces.King, new List<MoveInfo>()
            {
                new MoveInfo(new List<Directions>(){Directions.Top, Directions.Right, Directions.Bottom, Directions.Left}, new BoardPosition(0, 1), new Range(1, 1), false),
                new MoveInfo(new List<Directions>(){Directions.TopRight, Directions.BottomLeft, Directions.BottomRight, Directions.TopLeft }, new BoardPosition(1, 1), new Range(1, 1), false)
            });
            pieceRanges = new Dictionary<ChessPieces, MoveInfo>(pieceRangesLists.Count);
            foreach (var r in pieceRangesLists)
            {
                if (r.Value.Count > 0)
                {
                    MoveInfo newMoveInfo = r.Value[0];
                    for (int i = 1; i < r.Value.Count; i++)
                    {
                        foreach (var m in r.Value[i].Moves)
                        {
                            newMoveInfo.Moves.Add(m.Key, m.Value);
                        }
                        foreach (var m in r.Value[i].ReverseMoves)
                        {
                            newMoveInfo.ReverseMoves.Add(m.Key, m.Value);
                        }
                    }
                    pieceRanges.Add(r.Key, newMoveInfo);
                }
            }
        }
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
        public ChessPiece Copy()
        {
            return new ChessPiece(Position, Player, Piece);
        }
    }
    public struct ChessMove
    {
        public BoardPosition InitPosition { get; set; }
        public Directions Direction { get; set; }
        public int MoveAmount { get; set; }
        public ChessMove(BoardPosition initPosition, Directions direction, int moveAmount)
        {
            InitPosition = initPosition;
            Direction = direction;
            MoveAmount = moveAmount;
        }
    }

    public struct Range
    {
        public int Min { get; set; }
        public int Max { get; set; }
        public Range(int min, int max)
        {
            Min = min;
            Max = max;
        }
        public bool InRange(int value)
        {
            return value >= Min && value >= Max;
        }
    }
    public struct MoveInfo
    {
        public Dictionary<ChessMove, MoveChangeInfo<BoardPosition>> Moves { get; set; }
        public Dictionary<BoardPosition, MoveChangeInfo<ChessMove>> ReverseMoves { get; set; }
        public static Dictionary<Directions, (bool negX, bool negY, bool swap)> DirectionsRotations;
        public MoveInfo(List<Directions> directions, BoardPosition boardChange, Range multiplayerRange, bool canJump, Func<BoardPosition, bool> canMakeMove = null)
        {
            if (DirectionsRotations == null)
            {
                DirectionsRotations = new Dictionary<Directions, (bool negX, bool negY, bool swap)>();
                DirectionsRotations.Add(Directions.Top, (false, false, false));
                DirectionsRotations.Add(Directions.TopRight, (false, false, false));
                DirectionsRotations.Add(Directions.Right, (true, false, true));
                DirectionsRotations.Add(Directions.BottomRight, (true, false, true));
                DirectionsRotations.Add(Directions.Bottom, (true, true, false));
                DirectionsRotations.Add(Directions.BottomLeft, (true, true, false));
                DirectionsRotations.Add(Directions.Left, (false, true, true));
                DirectionsRotations.Add(Directions.TopLeft, (false, true, true));
            }
            Moves = new Dictionary<ChessMove, MoveChangeInfo<BoardPosition>>();
            ReverseMoves = new Dictionary<BoardPosition, MoveChangeInfo<ChessMove>>();
            foreach (var d in directions)
            {
                BoardPosition directionPos = boardChange;
                var rotationInfo = DirectionsRotations[d];
                directionPos.X *= rotationInfo.negX ? -1 : 1;
                directionPos.Y *= rotationInfo.negY ? -1 : 1;
                if (rotationInfo.swap)
                {
                    int temp = directionPos.X;
                    directionPos.X = directionPos.Y;
                    directionPos.Y = temp;
                }
                for (int i = multiplayerRange.Min; i <= multiplayerRange.Max; i++)
                {
                    ChessMove move = new ChessMove(new BoardPosition(-1,-1), d, i);
                    BoardPosition pos = directionPos * i;
                    Moves.Add(move, new MoveChangeInfo<BoardPosition>(pos, canJump, canMakeMove));
                    ReverseMoves.Add(pos, new MoveChangeInfo<ChessMove>(move, canJump, canMakeMove));
                }
            }
        }
    }
    public struct MoveChangeInfo<T> where T : struct
    {
        public T PositionChange { get; set; }
        public Func<BoardPosition, bool> CanMakeMove { get; }
        public bool CanJump { get; set; }
        public MoveChangeInfo(T positionChange, bool canJump, Func<BoardPosition, bool> canMakeMove)
        {
            PositionChange = positionChange;
            CanJump = canJump;
            CanMakeMove = canMakeMove;
        }
        public bool CanMakeAMove(BoardPosition relativePiecePosition)
        {
            if (CanMakeMove == null)
            {
                return true;
            }
            return CanMakeMove.Invoke(relativePiecePosition);
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
    public enum Directions
    {
        TopLeft = 0,
        Top = 1,
        TopRight = 2,
        Right = 3,
        BottomRight = 4,
        Bottom = 5,
        BottomLeft = 6,
        Left = 7
    }

}
