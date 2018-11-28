using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeuralNetTreeStuffViewer
{
    public class TickTacToe : ITurnBasedGame<TickTacToe, BoardPosition>
    {
        int boardSize;
        public My2dArray<Players> board { get; private set; }

        public TickTacToe Game { get { return this; } }

        public int TotalAmountOfMoves => 9;

        int placesRemaining;
        public event EventHandler<GameButtonArgs<(GameMove<BoardPosition> move, bool done)>> MoveMade;
        protected TickTacToe(TickTacToe game)
        {
            Copy(game, this);
        }
        public TickTacToe() : this(3) { }
        public TickTacToe(int boardSize)
        {
            this.boardSize = boardSize;
            Restart();
        }
        public TickTacToe Copy()
        {
            return new TickTacToe(this);
        }
        public static void Copy(TickTacToe board, TickTacToe newBoard)
        {
            newBoard.boardSize = board.boardSize;
            newBoard.board = board.board.Copy();
            newBoard.placesRemaining = board.placesRemaining;
        }
        public void Copy(TickTacToe newBoard)
        {
            Copy(this, newBoard);
        }

        public ITurnBasedGame<TickTacToe, BoardPosition> CopyInterface()
        {
            return Copy();
        }

        public void Restart()
        {
            board = new My2dArray<Players>(boardSize, boardSize);
            placesRemaining = 9;
        }
        public BoardState PlayerMakeMove(GameMove<BoardPosition> move)
        {
            if (IsLegalMove(move))
            {
                MakeMove(move);
                return CheckBoardState(move, false);
            }
            return BoardState.IllegalMove;
        }

        public bool IsLegalMove(GameMove<BoardPosition> move)
        {
            if (move.Move.X >= 0 && move.Move.X < boardSize && move.Move.Y >= 0 && move.Move.Y < boardSize)
            {
                if (move.Player != Players.None)
                {
                    if (board[move.Move.X, move.Move.Y] == Players.None)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public void MakeMove(GameMove<BoardPosition> move)
        {
            board[move.Move.X, move.Move.Y] = move.Player;
            placesRemaining--;
        }

        public BoardState CheckBoardState(GameMove<BoardPosition> lastMove, bool justCheckedAvilableMoves)
        {
            bool colPossibleWin = true;
            bool rowPossibleWin = true;
            bool diag1PossibleWin = false;
            bool diag2PossibleWin = false;

            if (lastMove.Move.X == lastMove.Move.Y)
            {
                diag1PossibleWin = true;
            }
            if (boardSize - lastMove.Move.X - 1 == lastMove.Move.Y)
            {
                diag2PossibleWin = true;
            }

            for (int i = 0; i < boardSize; i++)
            {
                if (colPossibleWin)
                {
                    if (board[lastMove.Move.X, i] != lastMove.Player)
                    {
                        colPossibleWin = false;
                    }
                }
                if (rowPossibleWin)
                {
                    if (board[i, lastMove.Move.Y] != lastMove.Player)
                    {
                        rowPossibleWin = false;
                    }
                }
                if (diag1PossibleWin)
                {
                    if (board[i, i] != lastMove.Player)
                    {
                        diag1PossibleWin = false;
                    }
                }
                if (diag2PossibleWin)
                {
                    if (board[boardSize - i - 1, i] != lastMove.Player)
                    {
                        diag2PossibleWin = false;
                    }
                }
            }
            if (colPossibleWin || rowPossibleWin || diag1PossibleWin || diag2PossibleWin)
            {
                placesRemaining = 0;
                if (lastMove.Player == Players.YouOrFirst)
                {
                    return BoardState.Win;
                }
                else
                {
                    return BoardState.Loss;
                }
            }

            if (placesRemaining <= 0)
            {
                return BoardState.Draw;
            }

            return BoardState.Continue;
        }

        public BoardState CheckBoardState(Players currentPlayer, bool justCheckedAvilableMoves)
        {
            BoardState boardState = BoardState.Continue;
            for(int x = 0; x < board.XLength; x++)
            {
                for (int y = 0; y < board.YLength; y++)
                {
                    if (board[x,y] != Players.None)
                    {
                        boardState = CheckBoardState(new GameMove<BoardPosition>(new BoardPosition(x, y), board[x, y]), justCheckedAvilableMoves);
                        if(boardState != BoardState.Continue)
                        {
                            return boardState;
                        }
                    }
                }
            }
            return boardState;
        }

        public Dictionary<int, BoardPosition> AvailableMoves(Players player)
        {
            Dictionary<int, BoardPosition> availableMoves;
            if (placesRemaining > 0)
            {
                availableMoves = new Dictionary<int, BoardPosition>(placesRemaining);

                for (int x = 0; x < boardSize; x++)
                {
                    for (int y = 0; y < boardSize; y++)
                    {
                        if (board[x, y] == Players.None)
                        {
                            var pos = new BoardPosition(x, y);
                            availableMoves.Add(GetMoveUniqueIdentifier(pos), pos);
                            if (availableMoves.Count >= placesRemaining)
                            {
                                return availableMoves;
                            }
                        }
                    }
                }
            }
            else
            {
                availableMoves = new Dictionary<int, BoardPosition>(0);
            }
            return availableMoves;
        }

        public bool displayTurnX = false;
        public void ComputerMakeMove(BoardPosition move)
        {
            BoardState state = PlayerMakeMove(new GameMove<BoardPosition>(move, GetPlayerFromBool(displayTurnX)));
            if (state != BoardState.IllegalMove && displayButtons.ContainsKey(move))
            {
                Button b = displayButtons[move];
                b.Invoke(new MethodInvoker(() => b.Text = ButtonText(state)));

                displayTurnX = !displayTurnX;
            }
        }

        Dictionary<BoardPosition, GameButton<BoardPosition>> displayButtons;
        public void DisplayGame(Panel panel)
        {
            panel.Controls.Clear();
            displayButtons = new Dictionary<BoardPosition, GameButton<BoardPosition>>(boardSize * boardSize);
            float size = (Math.Min(panel.Size.Width, panel.Height)) / 3f;
            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    GameButton<BoardPosition> newButton = new GameButton<BoardPosition>(new BoardPosition(x, y));
                    newButton.Size = new System.Drawing.Size((int)size, (int)size);
                    newButton.Location = new System.Drawing.Point((int)(size * x), (int)(size * y));
                    panel.Controls.Add(newButton);
                    displayButtons.Add(newButton.Info, newButton);
                    newButton.Click += NewButton_Click;
                }
            }
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

        private void NewButton_Click(object sender, GameButtonArgs<BoardPosition> e)
        {
            Button button = (Button)sender;
            var move = new GameMove<BoardPosition>(e.Info, GetPlayerFromBool(displayTurnX));
            BoardState state = PlayerMakeMove(move);

            if (state != BoardState.IllegalMove)
            {
                button.Text = ButtonText(state);

                displayTurnX = !displayTurnX;

                EventHandler<GameButtonArgs<(GameMove<BoardPosition> move, bool done)>> handler = this.MoveMade;
                if (handler != null)
                {
                    handler(this, new GameButtonArgs<(GameMove<BoardPosition> move, bool done)>((move, state != BoardState.Continue)));
                }
            }
        }
        public Players GetPlayerFromBool(bool b)
        {
            return b ? Players.OpponentOrSecond : Players.YouOrFirst;
        }

        string ButtonText(BoardState state)
        {
            if (state == BoardState.Loss)
            {
                return "X Wins";
            }
            else if (state == BoardState.Win)
            {
                return "O Wins";
            }
            else if (state == BoardState.Draw)
            {
                return "Draw";
            }
            else if (displayTurnX)
            {
                return "X";
            }
            else
            {
                return "O";
            }
        }

        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < board.Array.Length; i++)
            {
                if (i != 0)
                {
                    s += ",";
                }
                s += ((int)board.Array[i]).ToString();
            }
            return s;
        }

        public int GetMoveUniqueIdentifier(BoardPosition move)
        {
            return move.Y * boardSize + move.X;
        }

        public double[] GetInputs(Players currentPlayer)
        {
            double[] arr = new double[board.XLength * board.YLength * 2 + 1];

            int arrIndex = 0;
            for (int y = 0; y < board.YLength; y++)
            {
                for (int x = 0; x < board.XLength; x++)
                {
                    if (board[x, y] == Players.YouOrFirst)
                    {
                        arr[arrIndex] = 1;
                        arrIndex += 2;
                    }
                    else if (board[x, y] == Players.OpponentOrSecond)
                    {
                        arrIndex++;
                        arr[arrIndex] = 1;
                        arrIndex++;
                    }
                    else
                    {
                        arrIndex += 2;
                    }
                }
            }
            if (currentPlayer == Players.YouOrFirst)
            {
                arr[arrIndex] = 1;
            }
            else
            {
                arr[arrIndex] = 0;
            }

            return arr;
        }

        public void InitializeStaticVariables()
        {
        }

        public void DeserializeInit()
        {
        }

        public bool BoardEquals(ITurnBasedGame<TickTacToe, BoardPosition> other)
        {
            TickTacToe otherTTT = (TickTacToe)other;
            if(placesRemaining == otherTTT.placesRemaining && boardSize == otherTTT.boardSize)
            {
                for(int x = 0; x < board.XLength; x++)
                {
                    for(int y = 0; y < board.YLength; y++)
                    {
                        if(board[x,y] != otherTTT.board[x,y])
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            return false;
        }
        ITurnBasedGame ITurnBasedGame.Copy()
        {
            return Copy();
        }
    }
}
