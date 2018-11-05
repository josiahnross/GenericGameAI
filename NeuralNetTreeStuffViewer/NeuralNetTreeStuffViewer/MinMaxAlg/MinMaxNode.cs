using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer.MinMaxAlg
{
    public class MinMaxNode<T, T1>
    {
        public IEvaluateableTurnBasedGame<T, T1> CurrentState { get; private set; }
        public bool MaxTurn { get; private set; }
        public double MinimumOrMaximumValue { get; set; }
        public double Value { get; set; }
        public List<MinMaxNode<T, T1>> Children { get; set; }
        public MinMaxNode<T, T1> Parent { get; set; }
        public Dictionary<int, T1> AvailableMoves { get; }
        public int ExploredChildren { get; set; }
        public double MinimumParentMinMaxValue { get; set; }
        public double MaximumParentMinMaxValue { get; set; }
        bool exploredNode = false;
        public MoveIndex<T1>? MoveIndex { get; }
        public bool ExploredNode
        {
            get
            {
                return ExploredChildren == AvailableMoves.Count || exploredNode;
            }
            set
            {
                exploredNode = value;
            }
        }
        public MinMaxNode(bool maxTurn, IEvaluateableTurnBasedGame<T, T1> currentState, MinMaxNode<T, T1> parent, MoveIndex<T1>? moveIndex)
        {
            MaxTurn = maxTurn;
            MoveIndex = moveIndex;
            AvailableMoves = currentState.Game.AvailableMoves(MaxTurn ? Players.YouOrFirst : Players.OpponentOrSecond);
            MinimumOrMaximumValue = double.NaN;
            Value = double.NaN;
            CurrentState = currentState;
            Parent = parent;
            Children = new List<MinMaxNode<T, T1>>();
            ExploredChildren = 0;

            if (parent == null)
            {
                MinimumParentMinMaxValue = double.NaN;
                MaximumParentMinMaxValue = double.NaN;
            }
            else
            {
                MinimumParentMinMaxValue = parent.MinimumParentMinMaxValue;
                if (MaxTurn && BetterValue(MinimumParentMinMaxValue, parent.MinimumOrMaximumValue, false, true))
                {
                    MinimumParentMinMaxValue = parent.MinimumOrMaximumValue;
                }
                MaximumParentMinMaxValue = parent.MaximumParentMinMaxValue;
                if (!MaxTurn && BetterValue(MaximumParentMinMaxValue, parent.MinimumOrMaximumValue, true, true))
                {
                    MaximumParentMinMaxValue = parent.MinimumOrMaximumValue;
                }
            }
        }
        public bool AlphaBetaExplored()
        {
            double compareValue = MinimumParentMinMaxValue;
            if (!MaxTurn)
            {
                compareValue = MaximumParentMinMaxValue;
            }
            return BetterValue(MinimumOrMaximumValue, compareValue, !MaxTurn, false);
        }
        public bool BetterMinMaxValue(double value)
        {
            return BetterValue(MinimumOrMaximumValue, value, MaxTurn, true);
        }
        public bool BetterValue(double value)
        {
            return BetterValue(Value, value, MaxTurn, true);
        }
        public static bool BetterValue(double currentValue, double value, bool greaterThan, bool withNan)
        {
            if (greaterThan)
            {
                if (value > currentValue)
                {
                    return true;
                }
            }
            else
            {
                if (value < currentValue)
                {
                    return true;
                }
            }

            if (withNan && double.IsNaN(currentValue) && !double.IsNaN(value))
            {
                return true;
            }

            return false;
        }

        public string DebugString
        {
            get
            {
                return JsonConvert.SerializeObject(GetDebugInfo());
            }
        }
        DebugInfo GetDebugInfo()
        {
            DebugInfo info = new DebugInfo();
            info.MaxTurn = MaxTurn;
            info.MinimumOrMaximumValue = MinimumOrMaximumValue;
            info.Value = Value;
            info.Children = new List<DebugInfo>(Children.Count);
            info.MinimumParentMinMaxValue = MinimumParentMinMaxValue;
            info.MaximumParentMinMaxValue = MaximumParentMinMaxValue;
            info.Board = CurrentState.Game.ToString();
            info.BoardName = typeof(T).Name;
            foreach (var c in Children)
            {
                info.Children.Add(c.GetDebugInfo());
            }
            return info;
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
    }
    public struct MoveIndex<T>
    {
        public int Index { get; set; }
        public T Move { get; set; }
        public MoveIndex(int index, T move)
        {
            Index = index;
            Move = move;
        }
    }
}
