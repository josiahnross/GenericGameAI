using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeuralNetTreeStuffViewer
{
    public class ConnectFour : ITurnBasedGame<ConnectFour, int>
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        My2dArray<Players> board;
        int[] columnHeights;
        int placesRemaining;

        public event EventHandler<GameButtonArgs<(GameMove<int> move, bool done)>> MoveMade;

        private ConnectFour(ConnectFour other)
        {
            Copy(other, this);
        }

        public ConnectFour(int width, int height)
        {
            Width = width;
            Height = height;
            Restart();
        }

        public static void Copy(ConnectFour board, ConnectFour newBoard)
        {
            newBoard.Width = board.Width;
            newBoard.Height = board.Height;
            newBoard.board = board.board.Copy();
            newBoard.columnHeights = new int[board.Width];
            Array.Copy(board.columnHeights, newBoard.columnHeights, board.Width);
            newBoard.placesRemaining = board.placesRemaining;
        }

        public void Restart()
        {
            board = new My2dArray<Players>(Width, Height);
            columnHeights = new int[Width];
            placesRemaining = Width * Height;
        }

        public bool IsLegalMove(GameMove<int> move)
        {
            return placesRemaining > 0 && move.Move >= 0 && move.Move < Width && board[move.Move, Height - 1] == Players.None;
        }

        public void MakeMove(GameMove<int> move)
        {
            board[move.Move, columnHeights[move.Move]] = move.Player;
            columnHeights[move.Move]++;
            placesRemaining--;
        }

        public Dictionary<int, int> AvailableMoves(Players player)
        {
            Dictionary<int, int> moves = new Dictionary<int, int>();
            if (placesRemaining > 0)
            {
                for (int i = 0; i < board.XLength; i++)
                {
                    if (board[i, board.YLength - 1] == Players.None)
                    {
                        moves.Add(GetMoveUniqueIdentifier(i), i);
                    }
                }
            }
            return moves;
        }

        public BoardState CheckBoardState(GameMove<int> lastMove)
        {
            if(placesRemaining <= 0)
            {
                return BoardState.Draw;
            }

            BoardPosition pos = new BoardPosition(lastMove.Move, columnHeights[lastMove.Move] - 1);
            for (int xi = 1; xi >= -1; xi--)
            {
                for (int yi = 0; yi >= -1; yi--)
                {
                    if ((xi == 0 && yi == 0) || (xi == 1 && yi != -1))
                    {
                        continue;
                    }
                    int length = 1;
                    bool deadUp = false;
                    bool deadDown = false;
                    int x = pos.X;
                    int y = pos.Y;
                    int i = 1;
                    while (length < 4 && (!deadUp || !deadDown))
                    {
                        if (!deadUp)
                        {
                            int xTest = xi * i + x;
                            int yTest = yi * i + y;
                            if (board.InArray(xTest, yTest) && board[xTest, yTest] == lastMove.Player)
                            {
                                length++;
                            }
                            else
                            {
                                deadUp = true;
                            }
                        }
                        if (!deadDown)
                        {
                            int xTest = xi * -i + x;
                            int yTest = yi * -i + y;
                            if (board.InArray(xTest, yTest) && board[xTest, yTest] == lastMove.Player)
                            {
                                length++;
                            }
                            else
                            {
                                deadDown = true;
                            }
                        }
                        i++;
                    }
                    if(length >= 4)
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
                }
            }
            return BoardState.Continue;
        }


        public ConnectFour Copy()
        {
            return new ConnectFour(this);
        }

        public void Copy(ConnectFour newBoard)
        {
            Copy(this, newBoard);
        }

        public ITurnBasedGame<ConnectFour, int> CopyInterface()
        {
            return Copy();
        }

        public BoardState PlayerMakeMove(GameMove<int> move)
        {
            if (IsLegalMove(move))
            {
                MakeMove(move);
                return CheckBoardState(move);
            }
            return BoardState.IllegalMove;
        }

        public bool displayTurnRed = false;
        Dictionary<BoardPosition, GameButton<BoardPosition>> displayButtons;
        public void DisplayGame(Panel panel)
        {
            panel.Controls.Clear();
            displayButtons = new Dictionary<BoardPosition, GameButton<BoardPosition>>(Width * Height);
            float bWidth = panel.Width / (float)Width;
            float bHeight = panel.Height / (float)Height;
            Size size = new Size((int)bWidth, (int)bHeight);
            for(int x = 0; x < Width; x++)
            {
                for(int y = 0; y < Height; y++)
                {
                    GameButton<BoardPosition> newButton = new GameButton<BoardPosition>(new BoardPosition(x, y));
                    newButton.Size = size;
                    newButton.Location = new Point((int)(bWidth * x), (int)(bHeight * (Height - y - 1)));
                    panel.Controls.Add(newButton);
                    displayButtons.Add(newButton.Info, newButton);
                    newButton.Click += NewButton_Click;
                }
            }
        }

        private async void NewButton_Click(object sender, GameButtonArgs<BoardPosition> e)
        {
            var move = new GameMove<int>(e.Info.X, GetPlayerFromBool(displayTurnRed));
            int yPos = columnHeights[e.Info.X];
            BoardState state = PlayerMakeMove(move);

            if (state != BoardState.IllegalMove)
            {
                Button b = displayButtons[new BoardPosition(e.Info.X, yPos)];
                b.Text = ButtonText(state);
                b.BackColor = ButtonColor(displayTurnRed);

                displayTurnRed = !displayTurnRed;

                EventHandler<GameButtonArgs<(GameMove<int> move, bool done)>> handler = this.MoveMade;
                if (handler != null)
                {
                    await Task.Run(() => handler(this, new GameButtonArgs<(GameMove<int> move, bool done)>((move, state != BoardState.Continue))));
                }
            }
        }

        public void ComputerMakeMove(int move)
        {
            BoardState state = PlayerMakeMove(new GameMove<int>(move, GetPlayerFromBool(displayTurnRed)));
             
            if (state != BoardState.IllegalMove && displayButtons != null)
            {
                BoardPosition pos = new BoardPosition(move, columnHeights[move] - 1);
                if (displayButtons.ContainsKey(pos))
                {
                    Button b = displayButtons[pos];
                    b.Invoke(new MethodInvoker(() => {
                        b.Text = ButtonText(state);
                        b.BackColor = ButtonColor(displayTurnRed);
                    }));

                    displayTurnRed = !displayTurnRed;
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
                return "R";
            }
            else if (state == BoardState.Win)
            {
                return "B";
            }
            else if (state == BoardState.Draw)
            {
                return "D";
            }
            return "";
        }

        Color ButtonColor(bool b)
        {
            if(b)
            {
                return Color.Red;
            }
            else
            {
                return Color.Blue;
            }
        }

        public override string ToString()
        {
            string s = Width+ "," + Height + ",";
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

        public int GetMoveUniqueIdentifier(int move)
        {
            return move;
        }
    }
}
