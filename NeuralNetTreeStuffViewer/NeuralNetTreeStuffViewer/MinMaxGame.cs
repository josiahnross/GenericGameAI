using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeuralNetTreeStuffViewer
{
    public class MinMaxGame : ITurnBasedGame<MinMaxGame, bool>
    {
        public MinMaxGame Game { get { return this; } }
        public int MinIndex { get; private set;}
        public int MaxIndex { get; private set; }
        public int Depth { get; private set; }
        public int CurrentDepth { get; private set; }
        public double[] Nums { get; protected set; }
        public Func<double> RandomFunc { get; private set; }

        public int TotalAmountOfMoves => throw new NotImplementedException();

        public MinMaxGame(MinMaxGame game)
        {
            MinIndex = game.MinIndex;
            MaxIndex = game.MaxIndex;
            Depth = game.Depth;
            CurrentDepth = game.CurrentDepth;
            RandomFunc = game.RandomFunc;
            Nums = new double[game.Nums.Length];
            for (int i = 0; i < Nums.Length; i++)
            {
                Nums[i] = game.Nums[i];
            }
        }
        public MinMaxGame()
        {
            throw new NotImplementedException();
        }
        public MinMaxGame(int depth, Func<double> randomFunc, double[] values = null)
        {
            this.Depth = depth;
            this.RandomFunc = randomFunc;
            Nums = new double[(int)Math.Pow(2, depth)];
            Restart();
            if(values != null && values.Length == Nums.Length)
            {
                Nums = values;
            }
        }

        public event EventHandler<GameButtonArgs<(GameMove<bool> move, bool done)>> MoveMade;

        public void Restart()
        {
            MinIndex = 0;
            MaxIndex = Nums.Length;
            CurrentDepth = 0;
            for (int i = 0; i < Nums.Length; i++)
            {
                Nums[i] = RandomFunc.Invoke();
            }
        }


        public MinMaxGame Copy()
        {
            return new MinMaxGame(this);
        }

        public ITurnBasedGame<MinMaxGame, bool> CopyInterface()
        {
            return Copy();
        }

        public void Copy(MinMaxGame newBoard)
        {
            newBoard.MinIndex = MinIndex;
            newBoard.MaxIndex = MaxIndex;
            newBoard.Depth = Depth;
            newBoard.CurrentDepth = CurrentDepth;
            newBoard.RandomFunc = RandomFunc;
            newBoard.Nums = new double[Nums.Length];
            for (int i = 0; i < Nums.Length; i++)
            {
                newBoard.Nums[i] = Nums[i];
            }
        }


        public Dictionary<int, bool> AvailableMoves(Players player)
        {
            if (CurrentDepth < Depth)
            {
                Dictionary<int, bool> moves = new Dictionary<int, bool>();
                moves.Add(GetMoveUniqueIdentifier(false), false);
                moves.Add(GetMoveUniqueIdentifier(true), true);
                return moves;
            }
            else
            {
                return null;
            }
        }

        public BoardState CheckBoardState(GameMove<bool> lastMove, bool justCheckedAvilableMoves)
        {
            if (Depth - CurrentDepth <= 0)
            {
                return BoardState.Draw;
            }
            else
            {
                return BoardState.Continue;
            }
        }

        public bool IsLegalMove(GameMove<bool> move)
        {
            return true;
        }

        public void MakeMove(GameMove<bool> move)
        {
            int halfPoint = (MaxIndex - MinIndex) / 2 + MinIndex;
            if (move.Move)
            {
                MinIndex = halfPoint;
            }
            else
            {
                MaxIndex = halfPoint;
            }
            CurrentDepth++;
        }

        public BoardState PlayerMakeMove(GameMove<bool> move)
        {
            if (IsLegalMove(move))
            {
                MakeMove(move);
                return CheckBoardState(move, false);
            }
            return BoardState.IllegalMove;
        }

        public void DisplayGame(Panel panel)
        {
            TreeView treeView;
            if (panel.Controls.Count != 1 || panel.Controls[0].Name != GetHashCode().ToString())
            {
                panel.Controls.Clear();
                panel.Controls.Add(new TreeView());
                treeView = (TreeView)panel.Controls[0];
                treeView.Size = panel.Size;
                treeView.Location = new Point(0,0);
                treeView.Name = GetHashCode().ToString();
                treeView.Nodes.Add("Root");
                GenDisplayR(treeView, treeView.Nodes[0], 0, 0, Nums.Length);
            }
            else
            {
                treeView = (TreeView)panel.Controls[0];
            }
            DisplayCurrentPosR(treeView, treeView.Nodes[0], 0, 0, Nums.Length);
        }
        void GenDisplayR(TreeView treeView, TreeNode node, int displayDepth, int minIndex, int maxIndex)
        {
            //return;
            node.Text = (displayDepth % 2 == 0 ? "Max, " : "Min, ") + node.Text;
            
            if (displayDepth < Depth)
            {
                if (displayDepth + 1 >= Depth)
                {
                    node.Nodes.Add(Nums[maxIndex -1].ToString());
                    node.Nodes.Add(Nums[minIndex].ToString());
                }
                else
                {
                    node.Nodes.Add("True");
                    node.Nodes.Add("False");
                }
                int halfPoint = ((maxIndex - minIndex) / 2) + minIndex;
                GenDisplayR(treeView, node.Nodes[0], displayDepth + 1, halfPoint, maxIndex);
                GenDisplayR(treeView, node.Nodes[1], displayDepth + 1, minIndex, halfPoint);
            }
            
            node.Expand();
        }
        void DisplayCurrentPosR(TreeView treeView, TreeNode node, int displayDepth, int minIndex, int maxIndex)
        {
            //return;
            if (this.MinIndex == minIndex && this.MaxIndex == maxIndex)
            {
                node.BackColor = Color.Yellow;
            }
            else
            {
                node.BackColor = treeView.BackColor;
            }
            if (displayDepth < Depth)
            {
                int halfPoint = (maxIndex - minIndex) / 2 + minIndex;
                DisplayCurrentPosR(treeView, node.Nodes[0], displayDepth + 1, halfPoint, maxIndex);
                DisplayCurrentPosR(treeView, node.Nodes[1], displayDepth + 1, minIndex, halfPoint);
            }
        }

        public void HumanControlls(Panel panel, Players player)
        {
            Button falseButton = new Button();
            Button trueButton = new Button();

            panel.Controls.Add(falseButton);
            panel.Controls.Add(trueButton);

            falseButton.Text = "false";
            falseButton.Click += FalseButton_Click;

            trueButton.Text = "true";
            trueButton.Click += TrueButton_Click;

            falseButton.Size = new Size(panel.Size.Width / 2, panel.Height);
            falseButton.Location = new Point(panel.Size.Width / 2, 0);

            trueButton.Size = new Size(panel.Size.Width / 2, panel.Height);
            trueButton.Location = new Point(0, 0);
        }

        private void TrueButton_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void FalseButton_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void ComputerMakeMove(bool move)
        {
            throw new NotImplementedException();
        }

        public int GetMoveUniqueIdentifier(bool move)
        {
            if(move)
            {
                return 1;
            }
            return 0;
        }

        public void EnableDisplay(bool enable)
        {
            throw new NotImplementedException();
        }

        public double[] GetInputs(Players currentPlayer)
        {
            throw new NotImplementedException();
        }

        public void InitializeStaticVariables()
        {
            throw new NotImplementedException();
        }

        public void DeserializeInit()
        {
            throw new NotImplementedException();
        }

        public BoardState CheckBoardState()
        {
            throw new NotImplementedException();
        }

        public bool BoardEquals(ITurnBasedGame<MinMaxGame, bool> other)
        {
            throw new NotImplementedException();
        }

        public BoardState CheckBoardState(Players currentPlayer, bool justCheckedAvilableMoves)
        {
            throw new NotImplementedException();
        }

        ITurnBasedGame ITurnBasedGame.Copy()
        {
            throw new NotImplementedException();
        }
    }
}
