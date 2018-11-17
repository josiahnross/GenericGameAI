using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer.MinMaxAlg
{
    public static class MinMaxAlgorithm<T, T1>
    {
        public static MinMaxNode<T, T1> EvaluateMoves(uint depth, IEvaluateableTurnBasedGame<T, T1> game, bool maximisingPlayer = true)
        {
            var node = new MinMaxNode<T, T1>(maximisingPlayer, game, null, null);
            EvaluateMovesR(depth, 0, node);
            return node;
        }
        private static void EvaluateMovesR(uint depth, int currentDepth, MinMaxNode<T, T1> currentNode)
        {
            if (currentDepth >= depth || currentNode.AvailableMoves == null || currentNode.AvailableMoves.Count == 0)
            {
                BoardState boardState = BoardState.Continue;
                if (currentNode.MoveIndex != null)
                {
                    boardState = currentNode.CurrentState.Game.CheckBoardState(new GameMove<T1>(currentNode.MoveIndex.Value.Move, Funcs.GetPlayerFromBool(!currentNode.MaxTurn)), true);
                }
                if (boardState == BoardState.Continue)
                {
                    currentNode.Value = currentNode.CurrentState.EvaluateCurrentState(Funcs.GetPlayerFromBool(currentNode.MaxTurn));
                }
                else if (boardState == BoardState.Draw)
                {
                    currentNode.Value = 0;
                }
                else if (boardState == BoardState.Loss)
                {
                    currentNode.Value = double.MinValue;
                }
                else if (boardState == BoardState.Win)
                {
                    currentNode.Value = double.MaxValue;
                }
                if (currentNode.Parent != null)
                {
                    currentNode.Parent.ExploredChildren++;
                    UpdateParentNodes(currentNode.Parent, currentNode);
                }
            }
            else
            {
                foreach (var move in currentNode.AvailableMoves)
                {
                    if (!currentNode.ExploredNode)
                    {
                        var childState = currentNode.CurrentState.CopyEInterface();
                        bool childMaxTurn = !currentNode.MaxTurn;
                        childState.MakeMove(new GameMove<T1>(move.Value, Funcs.GetPlayerFromBool(currentNode.MaxTurn)), move.Key, true);
                        MinMaxNode<T, T1> childNode = new MinMaxNode<T, T1>(childMaxTurn, childState, currentNode, new MoveIndex<T1>(move.Key, move.Value));

                        currentNode.Children.Add(childNode);
                        //nodesToEvaluate.Enqueue((childNode, currentDepth +1));
                        int newDepth = currentDepth;
                        if (currentNode.AvailableMoves.Count > 1)
                        {
                            newDepth += 1;
                        }
                        EvaluateMovesR(depth, newDepth, childNode);
                    }
                }
            }
            //if (nodesToEvaluate.Count > 0)
            //{
            //    var nextNode = nodesToEvaluate.Dequeue();
            //    EvaluateMovesR(depth, nextNode.currentDepth, nextNode.node);
            //}
        }
        private static void UpdateParentNodes(MinMaxNode<T, T1> currentNode, MinMaxNode<T, T1> childNode)
        {
            if (currentNode.ExploredNode)
            {
                //for (int i = 0; i < currentNode.Children.Count; i++)
                //{
                //    if(currentNode.BetterValue(currentNode.Children[i].Value))
                //    {
                //        currentNode.Value = currentNode.Children[i].Value;
                //    }
                //}

                currentNode.Value = currentNode.MinimumOrMaximumValue;
                if (currentNode.BetterValue(childNode.Value))
                {
                    currentNode.Value = childNode.Value;
                }
                if (currentNode.Parent != null)
                {
                    currentNode.Parent.ExploredChildren++;
                    UpdateParentNodes(currentNode.Parent, currentNode);
                }
            }
            else
            {
                if (currentNode.BetterMinMaxValue(childNode.Value))
                {
                    currentNode.MinimumOrMaximumValue = childNode.Value;
                    currentNode.ExploredNode = currentNode.AlphaBetaExplored();
                    if (currentNode.ExploredNode)
                    {
                        currentNode.Value = currentNode.MinimumOrMaximumValue;
                        currentNode.Parent.ExploredChildren++;
                        UpdateParentNodes(currentNode.Parent, currentNode);
                    }
                }
            }
        }
    }
}
