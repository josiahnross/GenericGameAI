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
    public partial class Form1 : Form
    {
        Dictionary<string, (Action<DebugInfo> init, Action<DebugInfo, object> set)> debugInfoDisplayActions;
        object actionInfo;
        public Form1()
        {
            InitializeComponent();
            debugInfoDisplayActions = new Dictionary<string, (Action<DebugInfo> init, Action<DebugInfo, object> set)>();
            debugInfoDisplayActions.Add("TickTacToe", (InitTickTacToeDisplay, TickTacToeDisplayAction));
            debugInfoDisplayActions.Add("ConnectFour", (InitConnectFourDisplay, ConnectFourDisplayAction));
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

            treeView1.Nodes.Clear();
            var n = treeView1.Nodes.Add(info.ToString());
            infos.Add(n, info);
            foreach (var c in info.Children)
            {
                AddNodesToTree(n, c);
            }
            if(debugInfoDisplayActions.ContainsKey(info.BoardName))
            {
                debugInfoDisplayActions[info.BoardName].init?.Invoke(info);
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
            treeView1.AfterSelect += TreeView1_AfterSelect;
        }

        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var node = treeView1.SelectedNode;
            var info = infos[node];
            if (debugInfoDisplayActions.ContainsKey(info.BoardName))
            {
                debugInfoDisplayActions[info.BoardName].set?.Invoke(info, actionInfo);
            }
        }

        #region TickTacToe
        void InitTickTacToeDisplay(DebugInfo info)
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

        void TickTacToeDisplayAction(DebugInfo info, object extraStuff)
        {
            string[] board = info.Board.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
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
        void InitConnectFourDisplay(DebugInfo info)
        {
            boardPanel.Controls.Clear();
            List<Button> buttons = new List<Button>();

            string[] board = info.Board.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
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

        void ConnectFourDisplayAction(DebugInfo info, object extraStuff)
        {
            List<Button> buttons = (List<Button>)extraStuff;
            string[] board = info.Board.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
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
                buttons[i-2].BackColor = color;
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
}
