using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReadMinMaxNodeDebugInfo
{
    public partial class currentIndexTxtBox : Form
    {
        Dictionary<string, (Action<string> init, Action<string, object> set)> debugInfoDisplayActions;
        object actionInfo;
        public currentIndexTxtBox()
        {
            InitializeComponent();
            debugInfoDisplayActions = new Dictionary<string, (Action<string> init, Action<string, object> set)>();
            debugInfoDisplayActions.Add("TickTacToe", (InitTickTacToeDisplay, TickTacToeDisplayAction));
            debugInfoDisplayActions.Add("ConnectFour", (InitConnectFourDisplay, ConnectFourDisplayAction));
            debugInfoDisplayActions.Add("Checkers", (InitCheckersDisplay, CheckersDisplayAction));
        }

        Dictionary<TreeNode, DebugInfo> infos;
        private void button1_Click(object sender, EventArgs e)
        {
            LoadFromTxt(textBox1.Text);
        }
        void LoadFromTxt(string txt)
        {
            DebugInfo info = JsonConvert.DeserializeObject<DebugInfo>(txt);

            infos = new Dictionary<TreeNode, DebugInfo>();

            openDebugInputOutputButton.Nodes.Clear();
            var n = openDebugInputOutputButton.Nodes.Add(info.ToString());
            infos.Add(n, info);
            foreach (var c in info.Children)
            {
                AddNodesToTree(n, c);
            }
            if (debugInfoDisplayActions.ContainsKey(info.BoardName))
            {
                debugInfoDisplayActions[info.BoardName].init?.Invoke(info.Board);
            }
        }

        void AddNodesToTree(TreeNode node, DebugInfo info)
        {
            var n = node.Nodes.Add(info.ToString());

            infos.Add(n, info);
            foreach (var c in info.Children)
            {
                AddNodesToTree(n, c);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            openDebugInputOutputButton.AfterSelect += TreeView1_AfterSelect;
        }

        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var node = openDebugInputOutputButton.SelectedNode;
            var info = infos[node];
            if (debugInfoDisplayActions.ContainsKey(info.BoardName))
            {
                debugInfoDisplayActions[info.BoardName].set?.Invoke(info.Board, actionInfo);
            }
        }

        #region TickTacToe
        void InitTickTacToeDisplay(string strBoard)
        {
            boardPanel.Controls.Clear();
            List<Button> buttons = new List<Button>();
            int size = Math.Min(boardPanel.Size.Width, boardPanel.Size.Height) / 3;
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    Button b = new Button();
                    b.Size = new Size(size, size);
                    b.Location = new Point(x * size, y * size);
                    buttons.Add(b);
                    boardPanel.Controls.Add(b);
                }
            }

            actionInfo = buttons;
        }

        void TickTacToeDisplayAction(string strBoard, object extraStuff)
        {
            string[] board = strBoard.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<Button> buttons = (List<Button>)extraStuff;
            for (int i = 0; i < board.Length; i++)
            {
                string txt = "";
                if (board[i] == "1")
                {
                    txt = "O";
                }
                else if (board[i] == "2")
                {
                    txt = "X";
                }
                buttons[i].Text = txt;
            }
        }
        #endregion

        #region ConnectFour
        void InitConnectFourDisplay(string strBoard)
        {
            boardPanel.Controls.Clear();
            List<Button> buttons = new List<Button>();

            string[] board = strBoard.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            int width = int.Parse(board[0]);
            int height = int.Parse(board[1]);
            float sWidth = boardPanel.Size.Width / (float)width;
            float sHeight = boardPanel.Size.Height / (float)height;
            Size size = new Size((int)sWidth, (int)sHeight);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Button b = new Button();
                    b.Size = size;
                    b.Location = new Point((int)(x * sWidth), (int)((height - y - 1) * sHeight));
                    buttons.Add(b);
                    boardPanel.Controls.Add(b);
                }
            }
            actionInfo = buttons;
        }

        void ConnectFourDisplayAction(string strBoard, object extraStuff)
        {
            List<Button> buttons = (List<Button>)extraStuff;
            string[] board = strBoard.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 2; i < board.Length; i++)
            {
                Color color = BackColor;
                if (board[i] == "1")
                {
                    color = Color.Blue;
                }
                else if (board[i] == "2")
                {
                    color = Color.Red;
                }
                buttons[i - 2].BackColor = color;
            }
        }
        #endregion

        #region Checkers
        void InitCheckersDisplay(string strBoard)
        {
            boardPanel.Controls.Clear();
            List<Button> buttons = new List<Button>();
            int size = Math.Min(boardPanel.Size.Width, boardPanel.Size.Height) / 8;
            for (int x = 0; x < 8; x++)
            {
                for (int y = 7; y >= 0; y--)
                {
                    Button b = new Button();
                    b.Size = new Size(size, size);
                    b.Location = new Point(x * size, y * size);
                    buttons.Add(b);
                    boardPanel.Controls.Add(b);
                    if (!(x % 2 == 1 ^ y % 2 == 1))
                    {
                        b.BackColor = Color.LightGray;
                    }
                    else
                    {
                        b.BackColor = Color.DarkGray;
                    }
                }
            }

            actionInfo = buttons;
        }

        void CheckersDisplayAction(string strBoard, object extraStuff)
        {
            string[] board = strBoard.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            List<Button> buttons = (List<Button>)extraStuff;
            for (int i = 0; i < board.Length; i++)
            {
                string[] piece = board[i].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string txt = "";
                Color color;
                if (board[2] == "1")
                {
                    txt = "K";
                }
                if (piece[3] == "0")
                {
                    if (!((i % 8) % 2 == 1 ^ (i / 8) % 2 == 1))
                    {
                        color = Color.DarkGray;
                    }
                    else
                    {
                        color = Color.LightGray;
                    }
                }
                else if (piece[3] == "1")
                {
                    color = Color.Blue;
                }
                else
                {
                    color = Color.Red;
                }
                buttons[i].BackColor = color;
                buttons[i].Text = txt;
            }
        }
        #endregion

        private void fileButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            string info = File.ReadAllText(openFileDialog1.FileName);
            LoadFromTxt(info);
        }

        private void justBoardButton_Click(object sender, EventArgs e)
        {
            LoadBoardFromTxt(textBox1.Text);
        }

        void LoadBoardFromTxt(string txt)
        {
            infos = new Dictionary<TreeNode, DebugInfo>();
            openDebugInputOutputButton.Nodes.Clear();

            BoardInfo info = JsonConvert.DeserializeObject<BoardInfo>(txt);
            if (debugInfoDisplayActions.ContainsKey(info.BoardName))
            {
                debugInfoDisplayActions[info.BoardName].init?.Invoke(info.Board);
                debugInfoDisplayActions[info.BoardName].set?.Invoke(info.Board, actionInfo);
            }
        }

        private void openJustBoardFile_Click(object sender, EventArgs e)
        {
            openFileDialog2.ShowDialog();
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            string info = File.ReadAllText(openFileDialog2.FileName);
            LoadBoardFromTxt(info);
        }

        private void openDebugInOut_Click(object sender, EventArgs e)
        {
            openFileDialog3.ShowDialog();
        }

        private void openFileDialog3_FileOk(object sender, CancelEventArgs e)
        {
            string info = File.ReadAllText(openFileDialog3.FileName);
            LoadDebugInOutInfo(info);
        }
        List<InputOutputDebugInfo> inOutDebugInfo = null;
        void LoadDebugInOutInfo(string txt)
        {
            inOutDebugInfo = JsonConvert.DeserializeObject<List<InputOutputDebugInfo>>(txt);
            if (inOutDebugInfo.Count > 0)
            {
                LoadDebugInOutInfo(inOutDebugInfo[0]);
                indexUpDown.Value = 0;
            }
        }
        void LoadDebugInOutInfo(InputOutputDebugInfo info)
        {
            if (debugInfoDisplayActions.ContainsKey(info.BoardInfo.BoardName))
            {
                debugInfoDisplayActions[info.BoardInfo.BoardName].init?.Invoke(info.BoardInfo.Board);
                debugInfoDisplayActions[info.BoardInfo.BoardName].set?.Invoke(info.BoardInfo.Board, actionInfo);
            }

            DebugLabel.Text = "Value: ";
            if (info.Output == null)
            {
                DebugLabel.Text += "null";
            }
            else
            {
                DebugLabel.Text += info.Output.ToString();
            }
            DebugLabel.Text += " Player: " + Enum.GetName(typeof(Players), info.Player);
        }
        private void indexUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (inOutDebugInfo != null && inOutDebugInfo.Count > 0)
            {
                indexUpDown.Value = indexUpDown.Value % inOutDebugInfo.Count;
                LoadDebugInOutInfo(inOutDebugInfo[(int)indexUpDown.Value]);
            }
        }
    }
    public struct DebugInfo
    {
        public bool MaxTurn { get; set; }
        public double MinimumOrMaximumValue { get; set; }
        public double Value { get; set; }
        public List<DebugInfo> Children { get; set; }
        public string Board { get; set; }
        public double MinimumParentMinMaxValue { get; set; }
        public double MaximumParentMinMaxValue { get; set; }
        public string BoardName { get; set; }
        public override string ToString()
        {
            string s = "Turn: ";
            if (MaxTurn)
            {
                s += "Max";
            }
            else
            {
                s += "Min";
            }
            s += "    Value: " + Value + "    MinMax: " + MinimumOrMaximumValue;
            if (MaxTurn)
            {
                s += ">=";
            }
            else
            {
                s += "<=";
            }
            s += "    MaxParent: " + MaximumParentMinMaxValue + "    MinParent: " + MinimumParentMinMaxValue + "    Children: " + Children.Count;
            return s;
        }
    }
    public struct BoardInfo
    {
        public string Board { get; set; }
        public string BoardName { get; set; }
    }
    public struct InputOutputDebugInfo
    {
        public double? Output { get; set; }
        public BoardInfo BoardInfo { get; set; }
        public Players Player { get; set; }
        public InputOutputDebugInfo(BoardInfo boardInfo, double? output, Players player)
        {
            Output = output;
            BoardInfo = boardInfo;
            Player = player;
        }
    }
    public enum Players
    {
        None,
        YouOrFirst,
        OpponentOrSecond
    }
}
