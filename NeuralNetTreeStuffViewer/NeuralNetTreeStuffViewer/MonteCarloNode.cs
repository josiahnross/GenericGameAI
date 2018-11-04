using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer
{
    public class MonteCarloNode<T, T1> where T : ITurnBasedGame<T, T1>
    {
        public NodeGameInfo GameInfo { get; set; }
        public Dictionary<int, MonteCarloNode<T, T1>> Children { get; }
        public MonteCarloNode<T, T1> Parent { get; private set; }
        public ITurnBasedGame<T, T1> CurrentState { get; }
        public Players Player { get; }
        public Dictionary<int, T1> AvailableMoves { get; }
        public bool FullyExplored { get; set; }
        public (int index, T1 move) MoveIndex { get; }
        public int TotalAvialableMovesCount { get; }
        public bool EndOfGame { get; set; }
        public MonteCarloNode(MonteCarloNode<T, T1> parent, ITurnBasedGame<T, T1> currentState, (int index, T1 move) moveIndex, Players player)
        {
            EndOfGame = false;
            FullyExplored = false;
            Player = player;
            Parent = parent;
            GameInfo = new NodeGameInfo(0, 0, 0,0);
            Children = new Dictionary<int, MonteCarloNode<T, T1>>();
            CurrentState = currentState;
            AvailableMoves = currentState.AvailableMoves(Player);
            TotalAvialableMovesCount = AvailableMoves.Count;
            MoveIndex = moveIndex;
        }
        public void KillParent()
        {
            Parent = null;
        }
    }
    public struct MonteCarloNodeValue<T, T1> : IComparable<MonteCarloNodeValue<T, T1>> 
        where T : ITurnBasedGame<T, T1>
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
        public NodeGameInfo(int player1Wins, int player2Wins, int player1AmountOfGames, int player2AmountOfGames)
        {
            Player1Wins = player1Wins;
            Player2Wins = player2Wins;
            Player1AmountOfGames = player1AmountOfGames;
            Player2AmountOfGames = player2AmountOfGames;
        }
        public static NodeGameInfo operator +(NodeGameInfo left, NodeGameInfo right)
        {
            return new NodeGameInfo(left.Player1Wins + right.Player1Wins, left.Player2Wins + right.Player2Wins, left.Player1AmountOfGames + right.Player1AmountOfGames, left.Player2AmountOfGames + right.Player2AmountOfGames);
        }
    }
}
