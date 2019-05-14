using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer
{
    public interface IMonteCarloNode
    {
        NodeGameInfo GameInfo { get; set; }
        Players Player { get; }
        bool FullyExplored { get; set; }
        int TotalAvialableMovesCount { get;  }
        bool EndOfGame { get; set; }
        int Depth { get; set; }
        IMonteCarloNode UnGenParent { get; }
    }
    public class MonteCarloNode<T, T1> :IMonteCarloNode
        where T : ITurnBasedGame<T, T1>,new()
    {
        public NodeGameInfo GameInfo { get; set; }
        public Players Player { get; private set; }
        public bool FullyExplored { get; set; }
        public int TotalAvialableMovesCount { get; private set; }
        public bool EndOfGame { get; set; }
        public int Depth { get; set; }
        public IMonteCarloNode UnGenParent { get { return Parent; } }
        public Dictionary<int, MonteCarloNode<T, T1>> Children { get; }
        public MonteCarloNode<T, T1> Parent { get; private set; }
        public ITurnBasedGame<T, T1> CurrentState { get; }
        public Dictionary<int, T1> AvailableMoves { get; }
        public (int index, T1 move) MoveIndex { get; }
        public MonteCarloNode(MonteCarloNode<T, T1> parent, ITurnBasedGame<T, T1> currentState, (int index, T1 move) moveIndex, Players player, int depth)
        {
            EndOfGame = false;
            FullyExplored = false;
            Player = player;
            Parent = parent;
            GameInfo = new NodeGameInfo(0, 0, 0,0,0,0);
            Children = new Dictionary<int, MonteCarloNode<T, T1>>();
            CurrentState = currentState;
            AvailableMoves = currentState.AvailableMoves(Player);
            TotalAvialableMovesCount = AvailableMoves.Count;
            MoveIndex = moveIndex;
            Depth = depth;
        }
        public void ClearMoves()
        {
            TotalAvialableMovesCount = 0;
            AvailableMoves.Clear();
            Children.Clear();
        }
        public void KillParent()
        {
            Parent = null;
        }
    }
    public struct MonteCarloNodeValue<T, T1> : IComparable<MonteCarloNodeValue<T, T1>> 
        where T : ITurnBasedGame<T, T1>,new()
    {
        public MonteCarloNode<T, T1> Node { get; set; }
        public double Value { get; set; }
        public MonteCarloNodeValue(MonteCarloNode<T,T1> node, double value)
        {
            Node = node;
            Value = value;
        }
        public int CompareTo(MonteCarloNodeValue<T, T1> other)
        {
            return Value.CompareTo(other);
        }
    }

    public struct NodeGameInfo
    {
        public int Player1Wins { get; set; }
        public int Player2Wins { get; set; }
        public int AmountOfGames { get { return Player1AmountOfGames + Player2AmountOfGames; } }
        public int Player1AmountOfGames { get; set; }
        public int Player2AmountOfGames { get; set; }
        public int Player1TotalDepth { get; set; }
        public int Player2TotalDepth { get; set; }
        public NodeGameInfo(int player1Wins, int player2Wins, int player1AmountOfGames, int player2AmountOfGames, int player1TotalDepth, int player2TotalDepth)
        {
            Player1Wins = player1Wins;
            Player2Wins = player2Wins;
            Player1AmountOfGames = player1AmountOfGames;
            Player2AmountOfGames = player2AmountOfGames;
            Player1TotalDepth = player1TotalDepth;
            Player2TotalDepth = player2TotalDepth;
        }
        public static NodeGameInfo operator +(NodeGameInfo left, NodeGameInfo right)
        {
            return new NodeGameInfo(left.Player1Wins + right.Player1Wins, left.Player2Wins + right.Player2Wins, left.Player1AmountOfGames + right.Player1AmountOfGames, left.Player2AmountOfGames + right.Player2AmountOfGames, left.Player1TotalDepth + right.Player1TotalDepth, left.Player2TotalDepth + right.Player2TotalDepth);
        }
    }
}
