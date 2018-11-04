using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeuralNetTreeStuffViewer
{
    public class Checkers : ITurnBasedGame<Checkers, CheckersMove>
    {
        public int AmountOfFirstPlayerCheckers { get { return firstCheckers.Count; } }
        public int AmountOfSecondPlayerCheckers { get { return secondCheckers.Count; } }
        static My2dArray<CheckersPiece> startBoard;
        static Dictionary<bool, BoardPosition> frontDirections;
        My2dArray<CheckersPiece> board;
        HashSet<CheckersPiece> firstCheckers;
        HashSet<CheckersPiece> secondCheckers;
        Dictionary<Players, (BoardPosition? pos, Dictionary<int, CheckersMove> moves)> forceMultiJump;
        BoardState? finishedGame;

        public event EventHandler<GameButtonArgs<(GameMove<CheckersMove> move, bool done)>> MoveMade;

        public static void Copy(Checkers board, Checkers newBoard)
        {
            newBoard.board = new My2dArray<CheckersPiece>(board.board.XLength, board.board.YLength);
            for(int i = 0; i < newBoard.board.Array.Length; i++)
            {
                newBoard.board.Array[i] = new CheckersPiece(board.board.Array[i].Player, board.board.Array[i].Position);
                newBoard.board.Array[i].IsKing = board.board.Array[i].IsKing;
            }
            newBoard.firstCheckers = new HashSet<CheckersPiece>();
            foreach (var c in board.firstCheckers)
            {
                newBoard.firstCheckers.Add(newBoard.board[c.Position]);
            }
            newBoard.secondCheckers = new HashSet<CheckersPiece>();
            foreach (var c in board.secondCheckers)
            {
                newBoard.secondCheckers.Add(newBoard.board[c.Position]);
            }
            newBoard.forceMultiJump = new Dictionary<Players, (BoardPosition? pos, Dictionary<int, CheckersMove> moves)>();
            foreach(var i in board.forceMultiJump)
            {
                BoardPosition? pos = null;
                if (i.Value.pos != null)
                {
                    pos = i.Value.pos.Value;
                }
                Dictionary<int, CheckersMove> moves = null;
                if(i.Value.moves != null)
                {
                    moves = new Dictionary<int, CheckersMove>();
                    foreach(var j in i.Value.moves)
                    {
                        moves.Add(j.Key, j.Value);
                    }
                }
                newBoard.forceMultiJump.Add(i.Key, (pos, moves));
            }
            newBoard.finishedGame = board.finishedGame;
        }

        private Checkers(Checkers other)
        {
            Copy(other, this);
        }

        public Checkers()
        {
            if (startBoard == null)
            {
                SetStartBoard();
            }
            board = new My2dArray<CheckersPiece>(startBoard.XLength, startBoard.YLength);
            firstCheckers = new HashSet<CheckersPiece>();
            secondCheckers = new HashSet<CheckersPiece>();
            forceMultiJump = new Dictionary<Players, (BoardPosition? pos, Dictionary<int, CheckersMove> moves)>();
            forceMultiJump.Add(Players.YouOrFirst, (null, null));
            forceMultiJump.Add(Players.OpponentOrSecond, (null, null));
            Restart();
        }

        public void Restart()
        {
            firstCheckers.Clear();
            secondCheckers.Clear();
            for (int x = 0; x < board.XLength; x++)
            {
                for (int y = 0; y < board.YLength; y++)
                {
                    board[x, y] = startBoard[x, y];
                    if (board[x, y].Player == Players.YouOrFirst)
                    {
                        firstCheckers.Add(board[x, y]);
                    }
                    else if (board[x, y].Player == Players.OpponentOrSecond)
                    {
                        secondCheckers.Add(board[x, y]);
                    }
                }
            }
            finishedGame = null;
            forceMultiJump[Players.YouOrFirst] = (null, null);
            forceMultiJump[Players.OpponentOrSecond] = (null, null);
        }

        public void MakeMove(GameMove<CheckersMove> move)
        {
            if(move.Move.Pass)
            {
                return;
            }
            BoardPosition newPos = GetNewPos(move.Move);

            board[newPos] = board[move.Move.Position];
            board[newPos].Position = newPos;
            board[move.Move.Position] = new CheckersPiece(Players.None, move.Move.Position);
            if ((newPos.Y == board.YLength - 1 && move.Player == Players.YouOrFirst) || (newPos.Y == 0 && move.Player == Players.OpponentOrSecond))
            {
                board[newPos].IsKing = true;
            }
            if (move.Move.Jump)
            {
                BoardPosition deadPos = GetNewPos(new CheckersMove(move.Move.Position, move.Move.Left, false, move.Move.Back, move.Player));
                if (board[deadPos].Player == Players.YouOrFirst)
                {
                    firstCheckers.Remove(board[deadPos]);
                }
                else
                {
                    secondCheckers.Remove(board[deadPos]);
                }
                board[deadPos] = new CheckersPiece(Players.None, move.Move.Position);
                Dictionary<int, CheckersMove> moves = new Dictionary<int, CheckersMove>();
                Dictionary<int, CheckersMove> forcedMoves = new Dictionary<int, CheckersMove>();
                GetMoves(board[newPos], moves, forcedMoves);
                if (forcedMoves.Count > 0)
                {
                    forceMultiJump[move.Player] = (newPos, forcedMoves);
                }
                else
                {
                    forceMultiJump[move.Player] = (null, null);
                }
            }
            else
            {
                forceMultiJump[move.Player] = (null, null);
            }
        }

        public BoardPosition GetNewPos(CheckersMove move)
        {
            BoardPosition change = frontDirections[move.Left];
            if (move.Player == Players.OpponentOrSecond)
            {
                change = new BoardPosition(change.X, -change.Y);
            }
            if (move.Back)
            {
                change = new BoardPosition(change.X, -change.Y);
            }
            if (move.Jump)
            {
                change *= 2;
            }
            return move.Position + change;
        }

        public Dictionary<int, CheckersMove> AvailableMoves(Players player)
        {
            if (player == Players.None)
            {
                throw new IndexOutOfRangeException();
            }
            Dictionary<int, CheckersMove> moves = new Dictionary<int, CheckersMove>();

            if (finishedGame != null)
            {
                return moves;
            }
            Dictionary<int, CheckersMove> forcedMoves = new Dictionary<int, CheckersMove>();
            if (forceMultiJump[player].pos == null && forceMultiJump[GetOpposiePlayer(player)].pos == null)
            {
                HashSet<CheckersPiece> checkersList;
                if (player == Players.YouOrFirst)
                {
                    checkersList = firstCheckers;
                }
                else
                {
                    checkersList = secondCheckers;
                }

                foreach (var c in checkersList)
                {
                    GetMoves(c, moves, forcedMoves);
                }
            }
            else if (forceMultiJump[GetOpposiePlayer(player)].pos != null)
            {
                var move = new CheckersMove(true, player);
                moves.Add(GetMoveUniqueIdentifier(move), move);
            }
            else
            {
                forcedMoves = forceMultiJump[player].moves;
            }

            if (forcedMoves.Count > 0)
            {
                return forcedMoves;
            }
            else
            {
                if(moves.Count <= 0)
                {
                    finishedGame = BoardState.Continue;
                }
                return moves;
            }
        }

        public void GetMoves(CheckersPiece piece, Dictionary<int, CheckersMove> moves, Dictionary<int, CheckersMove> forcedMoves)
        {
            Players currentP = piece.Player;
            Players oppositeP = GetOpposiePlayer(currentP);

            foreach (var d in frontDirections)
            {
                BoardPosition direction = d.Value;
                if (currentP == Players.OpponentOrSecond)
                {
                    direction = new BoardPosition(direction.X, -direction.Y);
                }
                for (int i = 0; i < 2; i++)
                {
                    if (i > 0)
                    {
                        if (!piece.IsKing)
                        {
                            break;
                        }
                        else
                        {
                            direction = new BoardPosition(direction.X, -direction.Y);
                        }
                    }
                    BoardPosition adjasentTile = piece.Position + direction;
                    if (board.InArray(adjasentTile))
                    {
                        if (forcedMoves.Count <= 0 && board[adjasentTile].Player == Players.None)
                        {
                            CheckersMove move = new CheckersMove(piece.Position, d.Key, false, i > 0, currentP);
                            moves.Add(GetMoveUniqueIdentifier(move), move);
                        }
                        BoardPosition jumpTile = piece.Position + (direction * 2);
                        if (board.InArray(jumpTile) && board[adjasentTile].Player == oppositeP && board[jumpTile].Player == Players.None)
                        {
                            CheckersMove move = new CheckersMove(piece.Position, d.Key, true, i > 0, currentP);
                            forcedMoves.Add(GetMoveUniqueIdentifier(move), move);
                        }
                    }
                }

            }
        }

        public Players GetOpposiePlayer(Players player)
        {
            if (player == Players.YouOrFirst)
            {
                return Players.OpponentOrSecond;
            }
            else if (player == Players.OpponentOrSecond)
            {
                return Players.YouOrFirst;
            }
            else
            {
                return Players.None;
            }
        }

        public void SetStartBoard()
        {
            startBoard = new My2dArray<CheckersPiece>(8, 8);
            firstCheckers = new HashSet<CheckersPiece>();
            secondCheckers = new HashSet<CheckersPiece>();
            for (int y = 0; y < startBoard.YLength; y++)
            {
                for (int x = 0; x < startBoard.XLength; x++)
                {
                    if ((y < 3 || startBoard.YLength - y <= 3) && y % 2 == 1 ^ x % 2 == 1)
                    {
                        if (y < 3)
                        {
                            startBoard[x, y] = new CheckersPiece(Players.YouOrFirst, new BoardPosition(x, y));
                            firstCheckers.Add(startBoard[x, y]);
                        }
                        else if (startBoard.YLength - y <= 3)
                        {
                            startBoard[x, y] = new CheckersPiece(Players.OpponentOrSecond, new BoardPosition(x, y));
                            secondCheckers.Add(startBoard[x, y]);
                        }
                    }
                    else if (startBoard[x, y] == null)
                    {
                        startBoard[x, y] = new CheckersPiece(Players.None, new BoardPosition(x, y));
                    }
                }
            }
            frontDirections = new Dictionary<bool, BoardPosition>();
            frontDirections.Add(true, new BoardPosition(-1, 1));
            frontDirections.Add(false, new BoardPosition(1, 1));
        }

        public Checkers Copy()
        {
            return new Checkers(this);
        }

        public ITurnBasedGame<Checkers, CheckersMove> CopyInterface()
        {
            return Copy();
        }

        public void Copy(Checkers newBoard)
        {
            Copy(this, newBoard);
        }

        public BoardState PlayerMakeMove(GameMove<CheckersMove> move)
        {
            if (IsLegalMove(move))
            {
                MakeMove(move);
                return CheckBoardState(move);
            }
            return BoardState.IllegalMove;
        }

        public bool IsLegalMove(GameMove<CheckersMove> move)
        {
            if(move.Move.Pass)
            {
                return true;
            }
            if (forceMultiJump[move.Player].pos != null && forceMultiJump[move.Player].pos != move.Move.Position)
            {
                return false;
            }
            if (board[move.Move.Position].Player == Players.None)
            {
                return false;
            }
            if (!board.InArray(move.Move.Position) || board[move.Move.Position].Player != move.Player)
            {
                return false;
            }
            if (move.Move.Back && !board[move.Move.Position].IsKing)
            {
                return false;
            }
            BoardPosition newPos = GetNewPos(move.Move);
            if (!board.InArray(newPos) || board[newPos].Player != Players.None)
            {
                return false;
            }
            if (move.Move.Jump)
            {
                BoardPosition jmpPos = GetNewPos(new CheckersMove(move.Move.Position, move.Move.Left, false, move.Move.Back, move.Player));
                if (board[jmpPos].Player != GetOpposiePlayer(board[move.Move.Position].Player))
                {
                    return false;
                }
            }
            return true;
        }

        public BoardState CheckBoardState(GameMove<CheckersMove> lastMove)
        {
            if(finishedGame != null)
            {
                return finishedGame.Value;
            }
            if (firstCheckers.Count > 0 && secondCheckers.Count > 0)
            {
                return BoardState.Continue;
            }
            else if (firstCheckers.Count > 0)
            {
                finishedGame = BoardState.Win;
                return BoardState.Win;
            }
            else
            {
                finishedGame = BoardState.Loss;
                return BoardState.Loss;
            }
        }

        Dictionary<BoardPosition, GameButton<BoardPosition>> displayButtons;
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
                    newButton.BackColor = GetCheckerColor(board[x, y].Player, board[x, y].Position);
                    newButton.Text = GetButtonTxt(board[x, y]);
                    newButton.Click += NewButton_Click;
                }
            }
        }
        public string GetButtonTxt(CheckersPiece piece)
        {
            return piece.IsKing ? "K" : "";
        }
        public Color GetCheckerColor(Players player, BoardPosition location)
        {
            if (player == Players.None)
            {
                if (location.X % 2 == 1 ^ location.Y % 2 == 1)
                {
                    return Color.DarkGray;
                }
                else
                {
                    return Color.LightGray;
                }
            }
            else if (player == Players.YouOrFirst)
            {
                return Color.Blue;
            }
            else
            {
                return Color.Red;
            }
        }

        public Players GetPlayerFromColor(Color color)
        {
            if (color == Color.White)
            {
                return Players.None;
            }
            else if (color == Color.Blue)
            {
                return Players.YouOrFirst;
            }
            else
            {
                return Players.OpponentOrSecond;
            }
        }


        BoardPosition? selectedPosition = null;

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
                var move = GetCheckersMove(selectedPosition.Value, e.Info, movePlayer);
                if (move != null)
                {
                    var gameMove = new GameMove<CheckersMove>(move.Value, movePlayer);
                    BoardState state = PlayerMakeMove(gameMove);
                    if (state != BoardState.IllegalMove)
                    {
                        if (move.Value.Jump)
                        {
                            BoardPosition jumpedPos = GetNewPos(new CheckersMove(move.Value.Position, move.Value.Left, false, move.Value.Back, movePlayer));
                            displayButtons[jumpedPos].BackColor = GetCheckerColor(board[jumpedPos].Player, jumpedPos);
                            displayButtons[jumpedPos].Text = GetButtonTxt(board[jumpedPos]);
                        }
                        displayButtons[selectedPosition.Value].BackColor = GetCheckerColor(board[selectedPosition.Value].Player, selectedPosition.Value);
                        displayButtons[selectedPosition.Value].Text = GetButtonTxt(board[selectedPosition.Value]);
                        displayButtons[e.Info].BackColor = GetCheckerColor(board[e.Info].Player, e.Info);
                        displayButtons[e.Info].Text = GetButtonTxt(board[e.Info]);
                        if (state == BoardState.Win)
                        {
                            displayButtons[e.Info].Text = "B";
                        }
                        else if (state == BoardState.Loss)
                        {
                            displayButtons[e.Info].Text = "R";
                        }
                        EventHandler<GameButtonArgs<(GameMove<CheckersMove> move, bool done)>> handler = this.MoveMade;
                        if (handler != null)
                        {
                            await Task.Run(() => handler(this, new GameButtonArgs<(GameMove<CheckersMove> move, bool done)>((gameMove, state != BoardState.Continue))));
                        }
                    }
                    else
                    {
                        displayButtons[selectedPosition.Value].BackColor = GetCheckerColor(board[selectedPosition.Value].Player, selectedPosition.Value);
                    }
                }
                else
                {
                    displayButtons[selectedPosition.Value].BackColor = GetCheckerColor(board[selectedPosition.Value].Player, selectedPosition.Value);
                }
                selectedPosition = null;
            }
        }

        public CheckersMove? GetCheckersMove(BoardPosition startPos, BoardPosition endPos, Players player)
        {
            CheckersMove move = new CheckersMove(startPos, false, false, false, player);
            bool setMove = false;
            foreach (var d in frontDirections)
            {
                var backDirection = new BoardPosition(d.Value.X, -d.Value.Y);
                for (int i = 0; i < 2; i++)
                {
                    move.Jump = i == 1;
                    for (int j = 0; j < 2; j++)
                    {
                        BoardPosition direction = d.Value;
                        move.Back = j == 1;
                        if ((j == 1 && player == Players.YouOrFirst) || (j == 0 && player == Players.OpponentOrSecond))
                        {
                            direction = backDirection;
                        }
                        if (endPos - startPos == direction)
                        {
                            move.Left = d.Key;
                            setMove = true;
                            break;
                        }
                        if (endPos - startPos == direction * 2)
                        {
                            move.Left = d.Key;
                            move.Jump = true;
                            setMove = true;
                            break;
                        }
                    }
                    if (setMove)
                    {
                        break;
                    }
                }
                if (setMove)
                {
                    break;
                }
            }
            if (setMove)
            {
                return move;
            }
            else
            {
                return null;
            }
        }

        public void ComputerMakeMove(CheckersMove move)
        {
            BoardState state = PlayerMakeMove(new GameMove<CheckersMove>(move, move.Player));

            if (state != BoardState.IllegalMove)
            {
                if (displayButtons.ContainsKey(move.Position))
                {
                    Button b = displayButtons[move.Position];
                    b.Invoke(new MethodInvoker(() =>
                    {
                        for (int x = 0; x < board.XLength; x++)
                        {
                            for (int y = 0; y < board.YLength; y++)
                            {
                                BoardPosition pos = new BoardPosition(x, y);
                                displayButtons[pos].BackColor = GetCheckerColor(board[x, y].Player, board[x, y].Position);
                                displayButtons[pos].Text = GetButtonTxt(board[x, y]);
                            }
                        }
                    }));

                }
            }
        }

        public int GetMoveUniqueIdentifier(CheckersMove move)
        {
            return move.GetUniqueIdentifier();
        }
    }
    public class CheckersPiece
    {
        public BoardPosition Position { get; set; }
        public bool IsKing { get; set; }
        public Players Player { get; set; }
        public CheckersPiece(Players player, BoardPosition position)
        {
            Position = position;
            IsKing = false;
            Player = player;
        }
    }
    public struct CheckersMove
    {
        public bool Pass { get; set; }
        public BoardPosition Position { get; set; }
        public bool Left { get; set; }
        public bool Jump { get; set; }
        public bool Back { get; set; }
        public Players Player { get; set; }
        public CheckersMove(bool pass, Players player)
        {
            if (!pass) { throw new NullReferenceException(); }
            Pass = pass;
            Position = new BoardPosition(-1, -1);
            Left = false;
            Jump = false;
            Back = false;
            Player = player;
        }
        public CheckersMove(BoardPosition position, bool left, bool jump, bool back, Players player)
        {
            Pass = false;
            Position = position;
            Left = left;
            Jump = jump;
            Back = back;
            Player = player;
        }
        public int GetUniqueIdentifier()
        {
            if (Pass)
            {
                return -1;
            }
            else
            {
                int boardSize = 8;
                int boolsVariation = 8;
                int returnInt = (Position.Y * boardSize + Position.X) * boolsVariation;
                if (Left)
                {
                    returnInt += 1;
                }
                if (Jump)
                {
                    returnInt += 2;
                }
                if (Back)
                {
                    returnInt += 4;
                }
                return returnInt;
            }
        }
    }
}

