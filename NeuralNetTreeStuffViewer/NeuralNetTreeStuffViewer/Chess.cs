using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeuralNetTreeStuffViewer
{
    public class Chess : ITurnBasedGame<Chess, ChessMove>
    {
        public ChessPiece this[BoardPosition position]
        {
            get
            {
                return board[position];
            }
        }
        static My2dArray<ChessPiece> startBoard = null;
        static Dictionary<ChessPieces, MoveInfo> pieceRanges;
        static Dictionary<Players, Dictionary<ChessPieces, Bitmap>> pieceImages;
        static Bitmap noneImage;
        public Dictionary<Players, HashSet<ChessPiece>> newUnToggleList { get; private set; }
        HashSet<ChessPiece> unToggleList;
        My2dArray<ChessPiece> board;
        HashSet<ChessPiece> firstPieces;
        HashSet<ChessPiece> secondPieces;
        ChessPiece firstKing;
        ChessPiece secondKing;
        BoardState? finishedGame;
        Players currentPlayer;
        Players startPlayer;

        public event EventHandler<GameButtonArgs<(GameMove<ChessMove> move, bool done)>> MoveMade;

        public Chess Game => this;

        static int totalAmountOfMoves = -1;

        public int TotalAmountOfMoves
        {
            get
            {
                if (totalAmountOfMoves < 0)
                {
                    totalAmountOfMoves = (board.YLength - 1) * board.YLength + (board.XLength - 1) * 2;
                    totalAmountOfMoves++;
                    totalAmountOfMoves *= 7 * 8 * 2;
                }
                return totalAmountOfMoves;
            }
        }
        public Chess(Chess other)
        {
            Copy(other, this);
        }
        public Chess(Players startPlayer = Players.YouOrFirst)
        {
            if (startBoard == null)
            {
                InitStatic();
            }
            this.startPlayer = startPlayer;
            Restart();
        }

        public static void Copy(Chess board, Chess newBoard)
        {
            newBoard.currentPlayer = board.currentPlayer;
            newBoard.startPlayer = board.startPlayer;
            newBoard.board = new My2dArray<ChessPiece>(board.board.XLength, board.board.YLength);
            newBoard.firstPieces = new HashSet<ChessPiece>();
            newBoard.secondPieces = new HashSet<ChessPiece>();
            newBoard.finishedGame = board.finishedGame;
            if (newBoard.unToggleList != null)
            {
                newBoard.unToggleList.Clear();
                newBoard.newUnToggleList[Players.YouOrFirst].Clear();
                newBoard.newUnToggleList[Players.OpponentOrSecond].Clear();
            }
            else
            {
                newBoard.unToggleList = new HashSet<ChessPiece>();
                newBoard.newUnToggleList = new Dictionary<Players, HashSet<ChessPiece>>();
                newBoard.newUnToggleList.Add(Players.YouOrFirst, new HashSet<ChessPiece>());
                newBoard.newUnToggleList.Add(Players.OpponentOrSecond, new HashSet<ChessPiece>());
            }
            for (int x = 0; x < board.board.XLength; x++)
            {
                for (int y = 0; y < board.board.YLength; y++)
                {
                    newBoard.board[x, y] = board.board[x, y].Copy();
                    Players player = newBoard.board[x, y].Player;
                    if (board.unToggleList.Contains(board.board[x, y]))
                    {
                        newBoard.unToggleList.Add(newBoard.board[x, y]);
                    }
                    if (player != Players.None && board.newUnToggleList[player].Contains(board.board[x, y]))
                    {
                        newBoard.newUnToggleList[player].Add(newBoard.board[x, y]);
                    }
                    if (player == Players.YouOrFirst)
                    {
                        newBoard.firstPieces.Add(newBoard.board[x, y]);
                        if (newBoard.board[x, y].Piece == ChessPieces.King)
                        {
                            newBoard.firstKing = newBoard.board[x, y];
                        }
                    }
                    else if (player == Players.OpponentOrSecond)
                    {
                        newBoard.secondPieces.Add(newBoard.board[x, y]);
                        if (newBoard.board[x, y].Piece == ChessPieces.King)
                        {
                            newBoard.secondKing = newBoard.board[x, y];
                        }
                    }
                }
            }
        }

        public void MakeMove(GameMove<ChessMove> move)
        {
            ChessPiece piece = board[move.Move.InitPosition];
            ChessMove genMove = move.Move;
            genMove.InitPosition = new BoardPosition(-1, -1);
            var moveInfo = pieceRanges[piece.Piece].Moves[genMove];
            BoardPosition change = moveInfo.PositionChange;
            if (move.Player == Players.OpponentOrSecond)
            {
                change.Y *= -1;
            }
            BoardPosition oldPiecePos = piece.Position;
            BoardPosition newPos = change + piece.Position;
            var makeMoveInfo = moveInfo.CanMakeAMove(oldPiecePos, piece, newPos, board[newPos], move.Player, this, null);
            moveInfo.BeforeMakeAMove(oldPiecePos, piece, newPos, board[newPos], move.Player, this, null, move.Move.TurnIntoQueenNotKnight);
            ChessPiece capturedPiece = null;
            if (makeMoveInfo.Item3 != null)
            {
                capturedPiece = board[makeMoveInfo.Item3.Value];
            }

            if (capturedPiece != null && capturedPiece.Player != Players.None)
            {
                if (move.Player == Players.YouOrFirst)
                {
                    secondPieces.Remove(capturedPiece);
                }
                else
                {
                    firstPieces.Remove(capturedPiece);
                }
                if (capturedPiece.Position != newPos)
                {
                    board[capturedPiece.Position] = new ChessPiece(capturedPiece.Position, Players.None, ChessPieces.Pawn);
                    capturedPiece = null;
                }
            }
            if (makeMoveInfo.Item2 != null)
            {
                foreach (var p in makeMoveInfo.Item2)
                {
                    if (p.Value != null)
                    {
                        BoardPosition oldPos = p.Key.Position;
                        p.Key.Position = p.Value.Value;
                        board[p.Value.Value] = null;
                        board[p.Value.Value] = p.Key;
                        board[oldPos] = new ChessPiece(oldPos, Players.None, ChessPieces.Pawn);
                    }
                }
            }
            else
            {
                piece.Position = newPos;
                board[newPos] = null;
                board[newPos] = piece;
                board[oldPiecePos] = new ChessPiece(oldPiecePos, Players.None, ChessPieces.Pawn);
            }

            foreach (var p in unToggleList)
            {
                if (!newUnToggleList[currentPlayer].Contains(p))
                {
                    p.Tag2ndBool(false, currentPlayer, this);
                }
            }
            currentPlayer = Funcs.OppositePlayer(currentPlayer);
            unToggleList = newUnToggleList[currentPlayer];
            newUnToggleList[currentPlayer] = new HashSet<ChessPiece>();
        }


        //TODO: Pawn Gets To the Other Side of the Board

        public bool InCheck(Players player, Dictionary<ChessPiece, BoardPosition?> movedPieces = null)
        {
            ChessPiece king;
            BoardPosition kingPosition;
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

            if (movedPieces != null && movedPieces.ContainsKey(king))
            {
                kingPosition = movedPieces[king].Value;
            }
            else
            {
                kingPosition = king.Position;
            }
            foreach (var piece in opposingForces)
            {
                if (movedPieces != null && movedPieces.ContainsKey(piece) && movedPieces[piece] == null)
                {
                    continue;
                }
                BoardPosition piecePosition;
                if (movedPieces != null && movedPieces.ContainsKey(piece))
                {
                    piecePosition = movedPieces[piece].Value;
                }
                else
                {
                    piecePosition = piece.Position;
                }
                BoardPosition difference = kingPosition - piecePosition;
                if (opposingPlayer == Players.OpponentOrSecond)
                {
                    difference.Y *= -1;
                }
                if (pieceRanges[piece.Piece].ReverseMoves.ContainsKey(difference))
                {
                    var moveInfo = pieceRanges[piece.Piece];
                    var move = moveInfo.ReverseMoves[difference];
                    BoardPosition minMove = new BoardPosition(difference.X == 0 ? 0 : difference.X / Math.Abs(difference.X), difference.Y == 0 ? 0 : difference.Y / Math.Abs(difference.Y));
                    if (opposingPlayer == Players.OpponentOrSecond)
                    {
                        minMove.Y *= -1;
                    }
                    BoardPosition newPos = minMove + piecePosition;
                    ChessPiece newPosPiece = board[newPos];
                    if (kingPosition == newPos)
                    {
                        newPosPiece = king;
                    }
                    var canMakeMoveInfo = move.CanMakeAMove(piecePosition, piece, newPos, newPosPiece, piece.Player, this, movedPieces);
                    if (canMakeMoveInfo.Item1)
                    {
                        if (move.CanJump)
                        {
                            return true;
                        }
                        if (canMakeMoveInfo.Item3 != null)
                        {
                            BoardPosition capturedPosition = canMakeMoveInfo.Item3.Value;
                            if (capturedPosition != newPos || moveInfo.OneRange)
                            {
                                if (capturedPosition == kingPosition)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                for (BoardPosition pos = newPos; board.InArray(pos); pos += minMove)
                                {
                                    if (pos == kingPosition)
                                    {
                                        return true;
                                    }
                                    Players chessPiece = board[pos].Player;
                                    if (movedPieces != null && movedPieces.ContainsKey(board[pos]))
                                    {
                                        bool foundNewPiece = false;
                                        foreach (var p in movedPieces)
                                        {
                                            if (p.Value == pos)
                                            {
                                                chessPiece = p.Key.Player;
                                                foundNewPiece = true;
                                                break;
                                            }
                                        }
                                        if (!foundNewPiece)
                                        {
                                            chessPiece = Players.None;
                                        }
                                    }
                                    if (chessPiece != Players.None)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        public static bool EnPasantOpportunity(BoardPosition pos, ChessPiece currentPiece, BoardPosition newBoardPosition, Players player, Chess board)
        {
            BoardPosition left = newBoardPosition - new BoardPosition(1, 0);
            BoardPosition right = newBoardPosition + new BoardPosition(1, 0);
            if (currentPiece.TagedBool)
            {
                if (board.board.InArray(left) && board[left].Player == Funcs.OppositePlayer(player) && board[left].Piece == ChessPieces.Pawn)
                {
                    board[left].Tag2ndBool(true, player, board);
                }
                if (board.board.InArray(right) && board[right].Player == Funcs.OppositePlayer(player) && board[right].Piece == ChessPieces.Pawn)
                {
                    board[right].Tag2ndBool(true, player, board);
                }
                return false;
            }
            else
            {
                if (board.board.InArray(left) && board[left].TagedBool && board[left].Player == Funcs.OppositePlayer(player) && board[left].Piece == ChessPieces.Pawn)
                {
                    return true;
                }
                if (board.board.InArray(right) && board[right].TagedBool && board[right].Player == Funcs.OppositePlayer(player) && board[right].Piece == ChessPieces.Pawn)
                {
                    return true;
                }
                return false;
            }
        }

        public static BoardPosition GetRelativePosition(Players player, BoardPosition position)
        {
            if (player == Players.OpponentOrSecond)
            {
                return new BoardPosition(startBoard.XLength - position.X - 1, startBoard.YLength - position.Y - 1);
            }
            return position;
        }

        public Dictionary<int, ChessMove> AvailableMoves(Players player)
        {
            HashSet<ChessPiece> pieces;
            Players oppositePlayer = Funcs.OppositePlayer(player);
            if (player == Players.YouOrFirst)
            {
                pieces = firstPieces;
            }
            else
            {
                pieces = secondPieces;
            }
            Dictionary<int, ChessMove> moves = new Dictionary<int, ChessMove>();
            if (finishedGame != null)
            {
                return moves;
            }
            foreach (var piece in pieces)
            {
                MoveInfo moveInfo = pieceRanges[piece.Piece];
                foreach (var moveDirections in moveInfo.DirectionMoves)
                {
                    for (int i = 0; i < moveDirections.Value.Count; i++)
                    {
                        BoardPosition positionChange = moveDirections.Value[i].PositionChange.pos;
                        if (player == Players.OpponentOrSecond)
                        {
                            positionChange.Y *= -1;
                        }
                        BoardPosition newPosition = piece.Position + positionChange;
                        bool breakAfter = false;
                        if (board.InArray(newPosition) && board[newPosition].Piece != ChessPieces.King)
                        {
                            if (board[newPosition].Player == player)
                            {
                                if (!moveDirections.Value[i].CanJump)
                                {
                                    break;
                                }
                                continue;
                            }
                            else
                            {
                                if(board[newPosition].Player == oppositePlayer)
                                {
                                    breakAfter = true;
                                }
                                var moveChangeInfo = moveDirections.Value[i].CanMakeAMove(piece.Position, piece, newPosition, board[newPosition], player, this, null);
                                if (moveChangeInfo.Item1)
                                {
                                    if (!InCheck(player, moveChangeInfo.Item2))
                                    {
                                        ChessMove move = moveDirections.Value[i].PositionChange.move;
                                        move.InitPosition = piece.Position;
                                        moves.Add(GetMoveUniqueIdentifier(move), move);
                                    }
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                        if(breakAfter)
                        {
                            break;
                        }
                    }
                }
            }
            if (moves.Count <= 0 && finishedGame == null)
            {
                if (InCheck(player))
                {
                    if (player == Players.YouOrFirst)
                    {
                        finishedGame = BoardState.Loss;
                    }
                    else
                    {
                        finishedGame = BoardState.Win;
                    }
                }
                else if (InCheck(oppositePlayer))
                {
                    if (player == Players.YouOrFirst)
                    {
                        finishedGame = BoardState.Win;
                    }
                    else
                    {
                        finishedGame = BoardState.Loss;
                    }
                }
                else
                {
                    finishedGame = BoardState.Draw;
                }
            }
            return moves;
        }

        public BoardState CheckBoardState(Players currentPlayer, bool justCheckedAvilableMoves)
        {
            if (finishedGame == null)
            {
                if (!justCheckedAvilableMoves && AvailableMoves(currentPlayer).Count == 0)
                {
                    if (InCheck(currentPlayer))
                    {
                        if (currentPlayer == Players.YouOrFirst)
                        {
                            return BoardState.Loss;
                        }
                        else
                        {
                            return BoardState.Win;
                        }
                    }
                    else
                    {
                        return BoardState.Draw;
                    }
                }
                return BoardState.Continue;
            }
            return finishedGame.Value;
        }

        public void Restart()
        {
            currentPlayer = startPlayer;
            unToggleList = new HashSet<ChessPiece>();
            newUnToggleList = new Dictionary<Players, HashSet<ChessPiece>>();
            newUnToggleList.Add(Players.YouOrFirst, new HashSet<ChessPiece>());
            newUnToggleList.Add(Players.OpponentOrSecond, new HashSet<ChessPiece>());
            board = new My2dArray<ChessPiece>(startBoard.XLength, startBoard.YLength);
            firstPieces = new HashSet<ChessPiece>();
            secondPieces = new HashSet<ChessPiece>();
            finishedGame = null;
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
                new MoveInfo(new List<Directions>(){Directions.Top, Directions.Right, Directions.Bottom, Directions.Left}, new BoardPosition(0, 1), new Range(1, 7), false,false),
                new MoveInfo(new List<Directions>(){Directions.TopRight, Directions.BottomLeft, Directions.BottomRight, Directions.TopLeft }, new BoardPosition(1, 1), new Range(1, 7), false,false)
            });
            pieceRangesLists.Add(ChessPieces.Rook, new List<MoveInfo>()
            {
                new MoveInfo(new List<Directions>(){Directions.Top, Directions.Right, Directions.Bottom, Directions.Left}, new BoardPosition(0, 1), new Range(1, 7), false,false)
            });
            pieceRangesLists.Add(ChessPieces.Bishop, new List<MoveInfo>()
            {
                new MoveInfo(new List<Directions>(){Directions.TopRight, Directions.BottomLeft, Directions.BottomRight, Directions.TopLeft }, new BoardPosition(1, 1), new Range(1, 7), false,false)
            });
            pieceRangesLists.Add(ChessPieces.Knight, new List<MoveInfo>()
            {
                new MoveInfo(new List<Directions>(){Directions.Top, Directions.Right, Directions.Bottom, Directions.Left}, new BoardPosition(1, 2), new Range(1, 1), true,false),
                new MoveInfo(new List<Directions>(){Directions.TopRight, Directions.BottomLeft, Directions.BottomRight, Directions.TopLeft }, new BoardPosition(2, 1), new Range(1, 1), true,false)
            });
            pieceRangesLists.Add(ChessPieces.Pawn, new List<MoveInfo>()
            {
                new MoveInfo(new List<Directions>(){Directions.Top }, new BoardPosition(0, 1), new Range(1, 1), false, false,
                (newPos, piece) => {return GetRelativePosition(piece.Player, newPos).Y == board.YLength-1; },
                (pos,currPiece, newPos, newPiece, player,board, movedPieces)=>
                {
                    if(newPiece.Player == Players.None)
                    {
                        Dictionary<ChessPiece, BoardPosition?> dict = new Dictionary<ChessPiece, BoardPosition?>();
                        dict.Add(currPiece, newPos);
                        return (true, dict, null);
                    }
                    return (false, null, null);
                },
                (pos,currPiece,newPos,newPiece,player, board, movedPieces, queenNotKnight)=>
                {
                    currPiece.TagedBool = false;
                    currPiece.Tag2ndBool(EnPasantOpportunity(pos,currPiece, newPos, player,board),player, board);
                    UpgradePawn(newPos, currPiece, queenNotKnight);
                }),
                new MoveInfo(new List<Directions>(){Directions.Top }, new BoardPosition(0, 1), new Range(2, 2), false,false,
                (newPos, piece) => {return GetRelativePosition(piece.Player, newPos).Y == board.YLength-1; },
                (pos,currPiece,newPos,newPiece,player,board, movedPieces)=>
                {
                    return (GetRelativePosition(player, pos).Y == 1, MoveInfo.DefaultMovedPieces(pos,currPiece, newPos,newPiece, board), newPos);
                },
                (pos,currPiece,newPos,newPiece,player,board, movedPieces, queenNotKnight)=>
                {
                    currPiece.TagedBool = true;
                    currPiece.Tag2ndBool(EnPasantOpportunity(pos,currPiece, newPos, player,board),player, board);
                    UpgradePawn(newPos, currPiece, queenNotKnight);
                }),
                new MoveInfo(new List<Directions>(){Directions.TopRight, Directions.TopLeft }, new BoardPosition(1, 1), new Range(1, 1), false,false,
                (newPos, piece) => {return GetRelativePosition(piece.Player, newPos).Y == board.YLength-1; },
                (pos,currPiece,newPos,newPiece,player,board, movedPieces)=>
                {
                    if(newPiece.Player==Funcs.OppositePlayer(player))
                    {
                        return (true, MoveInfo.DefaultMovedPieces(pos, currPiece,newPos,newPiece, board), newPos);
                    }
                    if(currPiece.TagedBool2 && newPiece.Player==Players.None)
                    {
                        BoardPosition capturedPositon = newPos;
                        if(player == Players.YouOrFirst)
                        {
                            capturedPositon -= new BoardPosition(0,1);
                        }
                        else
                        {
                            capturedPositon += new BoardPosition(0,1);
                        }
                        if(board.board.InArray(capturedPositon)&& board[capturedPositon].TagedBool && board[capturedPositon].Player==Funcs.OppositePlayer(player))
                        {
                            Dictionary<ChessPiece, BoardPosition?> dict = new Dictionary<ChessPiece, BoardPosition?>();
                            dict.Add(currPiece, newPos);
                            dict.Add(board[capturedPositon], null);
                            return (true, dict, capturedPositon);
                        }
                    }
                    return (false, null, newPos);
                },
                (pos,currPiece,newPos,newPiece,player,board, movedPieces,queenNotKnight)=>
                {
                    if(newPiece.Player==Funcs.OppositePlayer(player))
                    {
                        currPiece.TagedBool = false;
                        currPiece.Tag2ndBool(EnPasantOpportunity(pos,currPiece, newPos, player,board),player, board);
                    }
                    else
                    {
                        currPiece.TagedBool = false;
                       currPiece.Tag2ndBool(false, player,board);
                    }
                    UpgradePawn(newPos, currPiece, queenNotKnight);
                })
            });
            pieceRangesLists.Add(ChessPieces.King, new List<MoveInfo>()
            {
                new MoveInfo(new List<Directions>(){Directions.Top, Directions.Right, Directions.Bottom, Directions.Left}, new BoardPosition(0, 1), new Range(1, 1), false,false),
                new MoveInfo(new List<Directions>(){Directions.TopRight, Directions.BottomLeft, Directions.BottomRight, Directions.TopLeft }, new BoardPosition(1, 1), new Range(1, 1), false,false),
                new MoveInfo(new List<Directions>(){Directions.Right, Directions.Left}, new BoardPosition(0,1), new Range(2,2), false,false, null,
                (pos,currPiece,newPos,newPiece,player, board, movedPieces)=>
                {
                    if(newPiece.Player == Players.None)
                    {
                        if(!currPiece.HasMoved)
                        {
                            ChessPiece rook;
                            BoardPosition emptyPos;
                            BoardPosition rookNewPos;
                            if(newPos.X < pos.X)
                            {
                                rook = board[new BoardPosition(0, pos.Y)];
                                emptyPos = new BoardPosition(1, pos.Y);
                                rookNewPos = pos - new BoardPosition(1,0);
                            }
                            else
                            {
                                rook = board[new BoardPosition(board.board.XLength-1, pos.Y)];
                                emptyPos = new BoardPosition(board.board.XLength-2, pos.Y);
                                rookNewPos = pos + new BoardPosition(1,0);
                            }
                            if(!rook.HasMoved && rook.Piece == ChessPieces.Rook && rook.Player == player && board[emptyPos].Player == Players.None)
                            {
                                ChessPiece emptyPiece = board[emptyPos];
                                ChessPiece rookNewPiece = board[rookNewPos];
                                if(movedPieces != null)
                                {
                                bool containsEmptyPiece = movedPieces.ContainsKey(emptyPiece);
                                bool containsRookNewPiece = movedPieces.ContainsKey(rookNewPiece);
                                if(containsEmptyPiece || containsRookNewPiece)
                                {
                                    bool foundEmptyPiece = !containsEmptyPiece;
                                    bool foundNewRookPiece = !containsRookNewPiece;
                                    foreach(var p in movedPieces)
                                    {
                                        if(p.Value == emptyPos)
                                        {
                                            foundEmptyPiece = true;
                                            emptyPiece = p.Key;
                                            if(foundNewRookPiece)
                                            {
                                                break;
                                            }
                                        }
                                        if (p.Value == rookNewPos)
                                        {
                                            foundNewRookPiece = true;
                                            rookNewPiece = p.Key;
                                            if(foundEmptyPiece)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    if(!foundEmptyPiece)
                                    {
                                        emptyPiece = null;
                                    }
                                    if(!foundNewRookPiece)
                                    {
                                        rookNewPiece = null;
                                    }
                                }
                                }
                                if((emptyPiece == null || emptyPiece.Player == Players.None) && (rookNewPiece == null || emptyPiece.Player == Players.None))
                                {
                                    if(!InCheck(player, movedPieces))
                                    {
                                        BoardPosition middlePos = (newPos-pos)/2 + pos;
                                        bool movedPiecesWasNull = movedPieces == null;
                                        if(movedPieces == null)
                                        {
                                            movedPieces = new Dictionary<ChessPiece, BoardPosition?>();
                                        }
                                        bool hasOldPos = false;
                                        BoardPosition? oldPos = null;
                                        if (movedPieces.ContainsKey(currPiece))
                                        {
                                            hasOldPos = true;
                                            oldPos = movedPieces[currPiece];
                                            movedPieces[currPiece] = middlePos;
                                        }
                                        else
                                        {
                                            movedPieces.Add(currPiece, middlePos);
                                        }
                                        bool returnTrue = false;
                                        if(!InCheck(player, movedPieces))
                                        {
                                            returnTrue = true;
                                        }
                                        if(!movedPiecesWasNull)
                                        {
                                            if(hasOldPos)
                                            {
                                                movedPieces[currPiece] = oldPos;
                                            }
                                            else
                                            {
                                                movedPieces.Remove(currPiece);
                                            }
                                        }
                                        if(returnTrue)
                                        {
                                            Dictionary<ChessPiece, BoardPosition?> dict = new Dictionary<ChessPiece, BoardPosition?>();
                                            dict.Add(currPiece, newPos);
                                            dict.Add(rook, rookNewPos);
                                            return (true, dict, null);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return (false, null, null);
                },null,false)
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
                        foreach (var m in r.Value[i].DirectionMoves)
                        {
                            if (newMoveInfo.DirectionMoves.ContainsKey(m.Key))
                            {
                                newMoveInfo.DirectionMoves[m.Key].AddRange(m.Value);
                            }
                            else
                            {
                                newMoveInfo.DirectionMoves.Add(m.Key, m.Value);
                            }
                        }
                    }
                    pieceRanges.Add(r.Key, newMoveInfo);
                }
            }

            pieceImages = new Dictionary<Players, Dictionary<ChessPieces, Bitmap>>();
            var firstImages = new Dictionary<ChessPieces, Bitmap>();
            var secondImages = new Dictionary<ChessPieces, Bitmap>();
            pieceImages.Add(Players.YouOrFirst, firstImages);
            pieceImages.Add(Players.OpponentOrSecond, secondImages);
            firstImages.Add(ChessPieces.King, Properties.Resources.whiteKing);
            firstImages.Add(ChessPieces.Queen, Properties.Resources.whiteQueen);
            firstImages.Add(ChessPieces.Rook, Properties.Resources.whiteRook);
            firstImages.Add(ChessPieces.Bishop, Properties.Resources.whiteBishop);
            firstImages.Add(ChessPieces.Knight, Properties.Resources.whiteKnight);
            firstImages.Add(ChessPieces.Pawn, Properties.Resources.whitePawn);

            secondImages.Add(ChessPieces.King, Properties.Resources.blackKing);
            secondImages.Add(ChessPieces.Queen, Properties.Resources.blackQueen);
            secondImages.Add(ChessPieces.Rook, Properties.Resources.blackRook);
            secondImages.Add(ChessPieces.Bishop, Properties.Resources.blackBishop);
            secondImages.Add(ChessPieces.Knight, Properties.Resources.blackKnight);
            secondImages.Add(ChessPieces.Pawn, Properties.Resources.blackPawn);
            noneImage = new Bitmap(1, 1);
        }

        void UpgradePawn(BoardPosition newPos, ChessPiece piece, bool turnIntoQueenNotKnight)
        {
            Players player = piece.Player;
            if (GetRelativePosition(player, newPos).Y == board.YLength - 1)
            {
                if (turnIntoQueenNotKnight)
                {
                    piece.Piece = ChessPieces.Queen;
                }
                else
                {
                    piece.Piece = ChessPieces.Knight;
                }
            }
        }

        public int GetMoveUniqueIdentifier(ChessMove move)
        {
            return move.GetMoveUniqueIdentifier();
        }

        public Chess Copy()
        {
            return new Chess(this);
        }

        public ITurnBasedGame<Chess, ChessMove> CopyInterface()
        {
            return new Chess(this);
        }

        public void Copy(Chess newBoard)
        {
            Copy(this, newBoard);
        }

        public BoardState PlayerMakeMove(GameMove<ChessMove> move)
        {
            if (IsLegalMove(move))
            {
                MakeMove(move);
                return CheckBoardState(move, false);
            }
            return BoardState.IllegalMove;
        }

        public bool IsLegalMove(GameMove<ChessMove> move)
        {
            if (board.InArray(move.Move.InitPosition) && board[move.Move.InitPosition].Player == move.Player && move.Player == currentPlayer)
            {
                ChessPieces piece = board[move.Move.InitPosition].Piece;
                ChessMove genericMove = move.Move;
                genericMove.InitPosition = new BoardPosition(-1, -1);
                if (pieceRanges[piece].Moves.ContainsKey(genericMove))
                {
                    var moveInfo = pieceRanges[piece].Moves[genericMove];
                    BoardPosition positionChange = moveInfo.PositionChange;
                    if (move.Player == Players.OpponentOrSecond)
                    {
                        positionChange = new BoardPosition(positionChange.X, -positionChange.Y);
                    }
                    BoardPosition newPos = move.Move.InitPosition + positionChange;
                    var canMakeMoveInfo = moveInfo.CanMakeAMove(move.Move.InitPosition, board[move.Move.InitPosition], newPos, board[newPos], move.Player, this, null);
                    if (canMakeMoveInfo.Item1)
                    {
                        if (board.InArray(newPos) && board[newPos].Player != move.Player)
                        {
                            if (!moveInfo.CanJump)
                            {
                                BoardPosition normalizedPositionChange = new BoardPosition(positionChange.X == 0 ? 0 : positionChange.X / Math.Abs(positionChange.X), positionChange.Y == 0 ? 0 : positionChange.Y / Math.Abs(positionChange.Y));
                                for (BoardPosition pos = normalizedPositionChange + move.Move.InitPosition; true; pos += normalizedPositionChange)
                                {
                                    if (pos == newPos)
                                    {
                                        break;
                                    }
                                    else if (board[pos].Player != Players.None)
                                    {
                                        return false;
                                    }
                                }
                            }
                            if (!InCheck(move.Player, canMakeMoveInfo.Item2))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public BoardState CheckBoardState(GameMove<ChessMove> lastMove, bool justCheckedAvilableMoves)
        {
            return CheckBoardState(lastMove.Player, justCheckedAvilableMoves);
        }

        Dictionary<BoardPosition, GameButton<BoardPosition>> displayButtons;
        BoardPosition? selectedPosition = null;
        public void DisplayGame(Panel panel)
        {
            panel.Controls.Clear();
            displayButtons = new Dictionary<BoardPosition, GameButton<BoardPosition>>(board.XLength * board.YLength);
            float bWidth = panel.Width / (float)board.XLength;
            float bHeight = panel.Height / (float)board.YLength;
            Size size = new Size((int)bWidth, (int)bHeight);
            selectedPosition = null;
            for (int x = 0; x < board.XLength; x++)
            {
                for (int y = 0; y < board.YLength; y++)
                {
                    GameButton<BoardPosition> newButton = new GameButton<BoardPosition>(new BoardPosition(x, y));
                    newButton.Size = size;
                    newButton.Location = new Point((int)(bWidth * x), (int)(bHeight * (board.YLength - y - 1)));
                    panel.Controls.Add(newButton);
                    displayButtons.Add(newButton.Info, newButton);
                    newButton.BackColor = GetChessPieceColor(board[x, y].Player, board[x, y].Position);
                    newButton.Image = GetChessPieceImage(board[x, y].Player, board[x, y].Piece, newButton.Size);
                    newButton.Click += NewButton_Click;
                }
            }
        }

        private async void NewButton_Click(object sender, GameButtonArgs<BoardPosition> e)
        {
            if (selectedPosition == null)
            {
                if (board[e.Info].Player != Players.None)
                {
                    selectedPosition = e.Info;
                    displayButtons[e.Info].BackColor = Color.Yellow;
                }
            }
            else
            {
                Players movePlayer = board[selectedPosition.Value].Player;
                ChessPieces piece = board[selectedPosition.Value].Piece;
                var moveInfo = pieceRanges[piece];
                BoardPosition positionChange = e.Info - selectedPosition.Value;
                if (movePlayer == Players.OpponentOrSecond)
                {
                    positionChange.Y *= -1;
                }
                if (moveInfo.ReverseMoves.ContainsKey(positionChange))
                {
                    var move = moveInfo.ReverseMoves[positionChange];
                    move.PositionChange = new ChessMove(selectedPosition.Value, move.PositionChange.Direction, move.PositionChange.MoveAmount, piece == ChessPieces.Pawn);
                    var gameMove = new GameMove<ChessMove>(move.PositionChange, movePlayer);
                    BoardState state = PlayerMakeMove(gameMove);
                    if(state == BoardState.IllegalMove)
                    {
                        gameMove.Move = new ChessMove(gameMove.Move.InitPosition, gameMove.Move.Direction, gameMove.Move.MoveAmount, false);
                        state = PlayerMakeMove(gameMove);
                    }
                    if (state != BoardState.IllegalMove)
                    {
                        for (int x = 0; x < board.XLength; x++)
                        {
                            for (int y = 0; y < board.YLength; y++)
                            {
                                BoardPosition pos = new BoardPosition(x, y);
                                displayButtons[pos].Image = GetChessPieceImage(board[pos].Player, board[pos].Piece, displayButtons[pos].Size);
                            }
                        }
                        displayButtons[selectedPosition.Value].BackColor = GetChessPieceColor(board[selectedPosition.Value].Player, selectedPosition.Value);
                        if (state == BoardState.Win)
                        {
                            displayButtons[e.Info].Text = "B";
                        }
                        else if (state == BoardState.Loss)
                        {
                            displayButtons[e.Info].Text = "R";
                        }
                        EventHandler<GameButtonArgs<(GameMove<ChessMove> move, bool done)>> handler = this.MoveMade;
                        if (handler != null)
                        {
                            await Task.Run(() => handler(this, new GameButtonArgs<(GameMove<ChessMove> move, bool done)>((gameMove, state != BoardState.Continue))));
                        }
                    }
                    else
                    {
                        displayButtons[selectedPosition.Value].BackColor = GetChessPieceColor(board[selectedPosition.Value].Player, selectedPosition.Value);
                    }
                }
                else
                {
                    displayButtons[selectedPosition.Value].BackColor = GetChessPieceColor(board[selectedPosition.Value].Player, selectedPosition.Value);
                }
                selectedPosition = null;
            }
        }

        Bitmap GetChessPieceImage(Players player, ChessPieces piece, Size buttonSize)
        {
            if (player == Players.None)
            {
                return noneImage;
            }
            return new Bitmap(pieceImages[player][piece], buttonSize);
        }

        Color GetChessPieceColor(Players player, BoardPosition position)
        {
            if (DarkPieceSquare(position.X, position.Y))
            {
                return Color.DarkGray;
            }
            return Color.LightGray;
        }

        public static bool DarkPieceSquare(int x, int y)
        {
            return !(x % 2 == 1 ^ y % 2 == 1);
        }

        public async void EnableDisplay(bool enable)
        {
            await Task.Run(() =>
            {
                foreach (var button in displayButtons.Values)
                {
                    button.Invoke(new MethodInvoker(() => button.Enabled = enable));
                }
            });
        }

        public void ComputerMakeMove(ChessMove move)
        {
            BoardState state = PlayerMakeMove(new GameMove<ChessMove>(move, board[move.InitPosition].Player));

            if (state != BoardState.IllegalMove)
            {
                if (displayButtons.ContainsKey(move.InitPosition))
                {
                    Button b = displayButtons[move.InitPosition];
                    b.Invoke(new MethodInvoker(() =>
                    {
                        for (int x = 0; x < board.XLength; x++)
                        {
                            for (int y = 0; y < board.YLength; y++)
                            {
                                BoardPosition pos = new BoardPosition(x, y);
                                displayButtons[pos].Image = GetChessPieceImage(board[pos].Player, board[pos].Piece, displayButtons[pos].Size);
                            }
                        }
                    }));
                }
            }
        }
        static int InputSize = -1;
        public double[] GetInputs(Players currentPlayer)
        {
            if (InputSize < 0)
            {
                InputSize = board.XLength * board.YLength;
                InputSize *= 3 + 2;//Piece Info
                InputSize++;//currentPlayer
            }
            double[] inputs = new double[InputSize];
            int currentIndex = 0;
            for (int y = 0; y < board.YLength; y++)
            {
                for (int x = 0; x < board.XLength; x++)
                {
                    SetPlayerInputs(board[x, y].Player, inputs, ref currentIndex);
                    SetPieceInputs(board[x, y].Piece, inputs, ref currentIndex);
                }
            }
            if (currentPlayer == Players.YouOrFirst)
            {
                inputs[currentIndex] = 1;
            }
            else
            {
                inputs[currentIndex] = 0;
            }
            return inputs;
        }
        void SetPlayerInputs(Players player, double[] inputs, ref int currentIndex)
        {
            int amountOfBinarryDigits = 2;
            switch (player)
            {
                case (Players.YouOrFirst):
                    inputs[currentIndex] = 1;
                    currentIndex += amountOfBinarryDigits;
                    break;
                case (Players.OpponentOrSecond):
                    currentIndex++;
                    inputs[currentIndex] = 1;
                    currentIndex += amountOfBinarryDigits - 1;
                    break;
                default:
                    currentIndex += amountOfBinarryDigits;
                    break;
            }
        }
        void SetPieceInputs(ChessPieces piece, double[] inputs, ref int currentIndex)
        {
            int amountOfBinaryDigits = 3;
            switch (piece)
            {
                case (ChessPieces.Knight):
                    inputs[currentIndex] = 1;
                    currentIndex += amountOfBinaryDigits;
                    break;
                case (ChessPieces.Bishop):
                    currentIndex++;
                    inputs[currentIndex] = 1;
                    currentIndex += amountOfBinaryDigits - 1;
                    break;
                case (ChessPieces.Rook):
                    currentIndex += 2;
                    inputs[currentIndex] = 1;
                    currentIndex += amountOfBinaryDigits - 2;
                    break;
                case (ChessPieces.Queen):
                    inputs[currentIndex] = 1;
                    currentIndex++;
                    inputs[currentIndex] = 1;
                    currentIndex += amountOfBinaryDigits - 1;
                    break;
                case (ChessPieces.King):
                    currentIndex++;
                    inputs[currentIndex] = 1;
                    currentIndex++;
                    inputs[currentIndex] = 1;
                    currentIndex += amountOfBinaryDigits - 2;
                    break;
                default:
                    currentIndex += amountOfBinaryDigits;
                    break;
            }
        }

        public void InitializeStaticVariables()
        {
            if (startBoard == null)
            {
                InitStatic();
            }
        }

        public void DeserializeInit()
        {
            firstPieces = new HashSet<ChessPiece>();
            secondPieces = new HashSet<ChessPiece>();
            for (int i = 0; i < board.Array.Length; i++)
            {
                if (board.Array[i].Player == Players.YouOrFirst)
                {
                    firstPieces.Add(board.Array[i]);
                    if (board.Array[i].Piece == ChessPieces.King)
                    {
                        firstKing = board.Array[i];
                    }
                }
                else if (board.Array[i].Player == Players.OpponentOrSecond)
                {
                    secondPieces.Add(board.Array[i]);
                    if (board.Array[i].Piece == ChessPieces.King)
                    {
                        secondKing = board.Array[i];
                    }
                }
            }
        }

        public bool BoardEquals(ITurnBasedGame<Chess, ChessMove> other)
        {
            Chess otherChess = (Chess)other;
            if (otherChess.currentPlayer == currentPlayer && otherChess.firstPieces.Count == firstPieces.Count &&
                otherChess.secondPieces.Count == secondPieces.Count && otherChess.firstKing.Position == firstKing.Position
                && otherChess.secondKing.Position == secondKing.Position && otherChess.unToggleList.Count == unToggleList.Count &&
                otherChess.newUnToggleList[Players.YouOrFirst].Count == newUnToggleList[Players.YouOrFirst].Count &&
                otherChess.newUnToggleList[Players.OpponentOrSecond].Count == newUnToggleList[Players.OpponentOrSecond].Count)
            {
                for (int i = 0; i < board.Array.Length; i++)
                {
                    ChessPiece piece = board.Array[i];
                    ChessPiece otherPiece = otherChess.board.Array[i];
                    if (!(piece.Player == otherPiece.Player && piece.Piece == otherPiece.Piece
                        && piece.TagedBool == otherPiece.TagedBool && piece.TagedBool2 == otherPiece.TagedBool2
                        && piece.HasMoved == otherPiece.HasMoved))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
        public override string ToString()
        {
            string str = "";
            for (int x = 0; x < board.XLength; x++)
            {
                for (int y = 0; y < board.YLength; y++)
                {
                    if (!(x == 0 && y == 0))
                    {
                        str += "-";
                    }
                    str += board[x, y].ToString();
                }
            }
            return str;
        }
    }

    public class ChessPiece
    {
        BoardPosition position;
        public BoardPosition Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                HasMoved = true;
            }
        }
        public bool TagedBool { get; set; }
        public bool TagedBool2 { get; private set; }
        public Players Player { get; set; }
        public ChessPieces Piece { get; set; }
        public bool HasMoved { get; private set; }
        public ChessPiece(BoardPosition pos, Players player, ChessPieces piece)
        {
            position = pos;
            Player = player;
            Piece = piece;
            HasMoved = false;
            TagedBool = false;
            TagedBool2 = false;
        }
        public ChessPiece Copy()
        {
            var p = new ChessPiece(Position, Player, Piece);
            p.HasMoved = HasMoved;
            p.TagedBool = TagedBool;
            p.TagedBool2 = TagedBool2;
            return p;
        }
        public void Tag2ndBool(bool val, Players player, Chess chess)
        {
            TagedBool2 = val;
            if (val && !chess.newUnToggleList[player].Contains(this))
            {
                chess.newUnToggleList[player].Add(this);
            }
        }
        public override string ToString()
        {
            return Position.X + "," + Position.Y + "," + (int)Piece + "," + (int)Player;
        }
    }

    public struct ChessMove
    {
        public BoardPosition InitPosition { get; set; }
        public Directions Direction { get; set; }
        public int MoveAmount { get; set; }
        public bool TurnIntoQueenNotKnight { get; set; }
        static int moveAmountDifference = 7;
        static int moveAmountOffset = 1;
        static int boardSize = 8;
        static int amountOfDirections = 8;
        public int GetMoveUniqueIdentifier()
        {
            int boardPos = InitPosition.Y * boardSize + InitPosition.X;
            boardPos *= moveAmountDifference * amountOfDirections * 2;
            if (TurnIntoQueenNotKnight)
            {
                boardPos += 1;
            }
            boardPos += (MoveAmount - moveAmountOffset) * 2;
            boardPos += ((int)Direction) * moveAmountDifference * 2;

            return boardPos;
        }
        public ChessMove(BoardPosition initPosition, Directions direction, int moveAmount, bool turnIntoQueenNotKngiht)
        {
            InitPosition = initPosition;
            Direction = direction;
            MoveAmount = moveAmount;
            TurnIntoQueenNotKnight = turnIntoQueenNotKngiht;
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
        public Dictionary<Directions, List<MoveChangeInfo<(BoardPosition pos, ChessMove move)>>> DirectionMoves { get; set; }
        public Dictionary<ChessMove, MoveChangeInfo<BoardPosition>> Moves { get; set; }
        public Dictionary<BoardPosition, MoveChangeInfo<ChessMove>> ReverseMoves { get; set; }
        public static Dictionary<Directions, (bool negX, bool negY, bool swap)> DirectionsRotations;
        public bool OneRange { get; }
        public MoveInfo(List<Directions> directions, BoardPosition boardChange, Range multiplayerRange, bool canJump, bool canUpgrade, Func<BoardPosition, ChessPiece, bool> upgradeCondition = null,
            Func<BoardPosition, ChessPiece, BoardPosition, ChessPiece, Players, Chess, Dictionary<ChessPiece, BoardPosition?>, (bool, Dictionary<ChessPiece, BoardPosition?>, BoardPosition?)> canMakeMove = null,
            Action<BoardPosition, ChessPiece, BoardPosition, ChessPiece, Players, Chess, Dictionary<ChessPiece, BoardPosition?>, bool> beforeMakeMove = null, bool? oneRange = null)
        {
            if (oneRange == null)
            {
                OneRange = multiplayerRange.Max - multiplayerRange.Min == 0;
            }
            else
            {
                OneRange = oneRange.Value;
            }
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
            DirectionMoves = new Dictionary<Directions, List<MoveChangeInfo<(BoardPosition, ChessMove)>>>();
            foreach (var d in directions)
            {
                var directionsMoveList = new List<MoveChangeInfo<(BoardPosition, ChessMove)>>();
                DirectionMoves.Add(d, directionsMoveList);
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
                    for (int j = 0; (canUpgrade && j < 2) || j < 1; j++)
                    {
                        bool upgradeV = j == 1;
                        ChessMove move = new ChessMove(new BoardPosition(-1, -1), d, i, upgradeV);
                        BoardPosition pos = directionPos * i;
                        var newCanMakeMove = canMakeMove;

                        if (upgradeV)
                        {
                            newCanMakeMove = (positon, piece, newPos, newPiece, player, board, movedPieces) =>
                            {
                                if (upgradeCondition == null || upgradeCondition.Invoke(newPos, piece))
                                {
                                    return canMakeMove == null ? (true, DefaultMovedPieces(positon, piece, newPos, newPiece, board), newPos) : canMakeMove.Invoke(positon, piece, newPos, newPiece, player, board, movedPieces);
                                }
                                return (false, null, null);
                            };
                        }

                        Moves.Add(move, new MoveChangeInfo<BoardPosition>(pos, canJump, newCanMakeMove, beforeMakeMove));
                        if (j == 0)
                        {
                            ReverseMoves.Add(pos, new MoveChangeInfo<ChessMove>(move, canJump, newCanMakeMove, beforeMakeMove));
                        }
                        directionsMoveList.Add(new MoveChangeInfo<(BoardPosition, ChessMove)>((pos, move), canJump, newCanMakeMove, beforeMakeMove));
                    }
                }
            }
        }

        public static Dictionary<ChessPiece, BoardPosition?> DefaultMovedPieces(BoardPosition piecePosition, ChessPiece currentPiece, BoardPosition newPiecePosition, ChessPiece newPosPiece, Chess board)
        {
            Dictionary<ChessPiece, BoardPosition?> dict = new Dictionary<ChessPiece, BoardPosition?>();
            dict.Add(currentPiece, newPiecePosition);
            dict.Add(newPosPiece, null);
            return dict;
        }
    }
    public struct MoveChangeInfo<T> where T : struct
    {
        public T PositionChange { get; set; }
        Func<BoardPosition, ChessPiece, BoardPosition, ChessPiece, Players, Chess, Dictionary<ChessPiece, BoardPosition?>, (bool, Dictionary<ChessPiece, BoardPosition?>, BoardPosition?)> CanMakeMove { get; }
        Action<BoardPosition, ChessPiece, BoardPosition, ChessPiece, Players, Chess, Dictionary<ChessPiece, BoardPosition?>, bool> BeforeMakeMove { get; }
        public bool CanJump { get; set; }
        public MoveChangeInfo(T positionChange, bool canJump,
            Func<BoardPosition, ChessPiece, BoardPosition, ChessPiece, Players, Chess, Dictionary<ChessPiece, BoardPosition?>, (bool, Dictionary<ChessPiece, BoardPosition?>, BoardPosition?)> canMakeMove,
            Action<BoardPosition, ChessPiece, BoardPosition, ChessPiece, Players, Chess, Dictionary<ChessPiece, BoardPosition?>, bool> afterMakeMove)
        {
            PositionChange = positionChange;
            CanJump = canJump;
            CanMakeMove = canMakeMove;
            BeforeMakeMove = afterMakeMove;
        }
        public (bool, Dictionary<ChessPiece, BoardPosition?>, BoardPosition?) CanMakeAMove(BoardPosition piecePosition, ChessPiece currentPiece, BoardPosition newPiecePosition, ChessPiece newPiece, Players player, Chess board, Dictionary<ChessPiece, BoardPosition?> movedPieces)
        {
            if (CanMakeMove == null)
            {
                return (true, MoveInfo.DefaultMovedPieces(piecePosition, currentPiece, newPiecePosition, newPiece, board), newPiecePosition);
            }
            return CanMakeMove.Invoke(piecePosition, currentPiece, newPiecePosition, newPiece, player, board, movedPieces);
        }
        public void BeforeMakeAMove(BoardPosition piecePosition, ChessPiece currentPiece, BoardPosition newPiecePosition, ChessPiece newPiece, Players player, Chess board, Dictionary<ChessPiece, BoardPosition?> movedPieces, bool turnIntoQueenNotKnight)
        {
            if (BeforeMakeMove != null)
            {
                BeforeMakeMove.Invoke(piecePosition, currentPiece, newPiecePosition, newPiece, player, board, movedPieces, turnIntoQueenNotKnight);
            }
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
