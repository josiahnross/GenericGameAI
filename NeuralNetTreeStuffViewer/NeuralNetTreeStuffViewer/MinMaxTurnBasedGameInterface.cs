using NeuralNetTreeStuffViewer.MinMaxAlg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer
{
    public class MinMaxTurnBasedGameInterface<T, T1> : MinMaxEvaluator<T, T1>
        where T : ITurnBasedGame<T, T1>
        where T1 : struct
    {
        public ITurnBasedGame<T, T1> DisplayGame { get; }
        public string DebugStringPath { get; set; }

        public bool AiFirst { get; private set; }

        private MinMaxTurnBasedGameInterface(MinMaxTurnBasedGameInterface<T, T1> gameInterface)
            : base(gameInterface, gameInterface.Game.CopyInterface())
        {
            AiFirst = gameInterface.AiFirst;
            DebugStringPath = gameInterface.DebugStringPath;
        }

        public MinMaxTurnBasedGameInterface(T game, T displayGame, bool aiFirst, uint makeMoveMinMaxDepth = 5, string debugStringPath = null)
            : base(game, null, makeMoveMinMaxDepth, debugStringPath)
        {
            AiFirst = aiFirst;
            DebugStringPath = debugStringPath;
            DisplayGame = displayGame;
        }

        public void SetEvaluator(IEvaluateableTurnBasedGame<T, T1> evaulator)
        {
            Evaluator = evaulator;
            if (DisplayGame != null)
            {
                DisplayGame.MoveMade += MoveMadeAndResponse;
            }
            if (AiFirst)
            {
                AIMakeMove();
            }
        }

        private async void MoveMadeAndResponse(object sender, GameButtonArgs<(GameMove<T1> move, bool done)> e)
        {
            await Task.Run(() =>
            {
                if (!e.Info.done)
                {
                    int moveIndex = Game.GetMoveUniqueIdentifier(e.Info.move.Move);
                    MakeMove(e.Info.move, moveIndex);
                    DisplayGame.EnableDisplay(false);
                    AIMakeMove();
                    DisplayGame.EnableDisplay(true);
                }
            });
        }

        T1? AIMakeMove()
        {
            if (DisplayGame != null)
            {
                var availableMoves = Game.AvailableMoves(GetAiPlayer());
                MoveIndex<T1>? moveIndex = null;
                if (availableMoves.Count > 1)
                {
                    MinMaxNode<T, T1> node;
                    node = MinMaxAlgorithm<T, T1>.EvaluateMoves(MakeMoveMinMaxDepth, Evaluator, AiFirst);
                    MinMaxNode<T, T1> nextMoveChild = null;
                    foreach (var n in node.Children)
                    {
                        if (n.Value == node.Value)
                        {
                            nextMoveChild = n;
                            break;
                        }
                    }
                    if (nextMoveChild != null)
                    {
                        if (DebugStringPath != null)
                        {
                            //File.WriteAllText(DebugStringPath, nextMoveChild.Parent.DebugString);
                        }

                        if (nextMoveChild.MoveIndex == null)
                        { throw new NullReferenceException(); }
                        moveIndex = nextMoveChild.MoveIndex;
                    }
                }
                else
                {
                    foreach(var m in availableMoves)
                    {
                        moveIndex = new MoveIndex<T1>(m.Key, m.Value);
                    }
                }

                if (moveIndex != null)
                {
                    MakeMoveOnGame(new GameMove<T1>(moveIndex.Value.Move, GetAiPlayer()), moveIndex.Value.Index);
                    Players humanPlayer = AiFirst ? Players.OpponentOrSecond : Players.YouOrFirst;
                    var moves = Game.AvailableMoves(humanPlayer);
                    if (moves.Count == 1 && moves.ContainsKey(-1))
                    {
                        T1 move = moves[-1];
                        MakeMoveOnGame(new GameMove<T1>(move, humanPlayer), -1);
                        return AIMakeMove();
                    }
                    return moveIndex.Value.Move;
                }

            }
            return null;
        }

        public Players GetAiPlayer()
        {
            return AiFirst ? Players.YouOrFirst : Players.OpponentOrSecond;
        }

        public void MakeMoveOnGame(GameMove<T1> move, int moveIndex)
        {
            MakeMove(move, moveIndex);
            DisplayGame.ComputerMakeMove(move.Move);
        }
    }

}
