using NeuralNetTreeStuffViewer.MinMaxAlg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer
{
    public class MinMaxTurnBasedGameInterface<T, T1> : IEvaluateableTurnBasedGame<T, T1>
        where T : ITurnBasedGame<T, T1>
        where T1 : struct
    {
        public bool AiFirst { get; private set; }
        public string DebugStringPath { get; set; }
        public ITurnBasedGame<T, T1> Game { get; }
        public uint MakeMoveMinMaxDepth { get; set; }
        public IEvaulator<T, T1> Evaulator { get; private set; }


        private MinMaxTurnBasedGameInterface(MinMaxTurnBasedGameInterface<T, T1> gameInterface)
        {
            Evaulator = gameInterface.Evaulator.Copy();

            AiFirst = gameInterface.AiFirst;
            DebugStringPath = gameInterface.DebugStringPath;
            Game = gameInterface.Game.Copy();
            MakeMoveMinMaxDepth = gameInterface.MakeMoveMinMaxDepth;
        }

        public MinMaxTurnBasedGameInterface(T game, bool aiFirst, IEvaulator<T, T1> evaulator, uint makeMoveMinMaxDepth = 5, string debugStringPath = null)
        {
            Evaulator = evaulator;
            Evaulator?.Init(game, aiFirst);
            if (game == null)
            { throw new NullReferenceException(); }
            AiFirst = aiFirst;
            DebugStringPath = debugStringPath;
            Game = game;
            MakeMoveMinMaxDepth = makeMoveMinMaxDepth;

            game.MoveMade += MoveMadeAndResponse;
            if (AiFirst)
            {
                AIMakeMove();
            }
        }


        public Players GetAiPlayer()
        {
            return AiFirst ? Players.YouOrFirst : Players.OpponentOrSecond;
        }

        private async void MoveMadeAndResponse(object sender, GameButtonArgs<(GameMove<T1> move, bool done)> e)
        {
            await Task.Run(() =>
            {
                if (!e.Info.done)
                {
                    int moveIndex = Game.GetMoveUniqueIdentifier(e.Info.move.Move);
                    MakeMove(e.Info.move, moveIndex);
                    AIMakeMove();
                }
            });
        }

        T1? AIMakeMove()
        {
            if (Game != null)
            {
                var node = MinMaxAlgorithm<T, T1>.EvaluateMoves(MakeMoveMinMaxDepth, this, AiFirst);
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
                        File.WriteAllText(DebugStringPath, nextMoveChild.Parent.DebugString);
                    }

                    if (nextMoveChild.MoveIndex == null)
                    { throw new NullReferenceException(); }

                    MakeMoveOnGame(new GameMove<T1>(nextMoveChild.MoveIndex.Value.Move, GetAiPlayer()), nextMoveChild.MoveIndex.Value.Index);
                    Players humanPlayer = AiFirst ? Players.OpponentOrSecond : Players.YouOrFirst;
                    var moves = Game.AvailableMoves(humanPlayer);
                    if (moves.Count == 1 && moves.ContainsKey(-1))
                    {
                        T1 move = moves[-1];
                        MakeMoveOnGame(new GameMove<T1>(move, humanPlayer), -1);
                        return AIMakeMove();
                    }
                    return nextMoveChild.MoveIndex.Value.Move;
                }
            }
            return null;
        }

        public IEvaluateableTurnBasedGame<T, T1> CopyEInterface()
        {
            return new MinMaxTurnBasedGameInterface<T, T1>(this);
        }

        public void Restart()
        {
            Evaulator.Restart();
        }

        public double EvaluateCurrentState()
        {
            return Evaulator.Evaluate(Game);
        }

        public void MakeMoveOnGame(GameMove<T1> move, int moveIndex)
        {
            MakeMove(move, moveIndex);
            Game.ComputerMakeMove(move.Move);
        }
        public void MakeMove(GameMove<T1> move, int moveIndex)
        {
            Evaulator?.MakeMove(move, moveIndex);
            //Game.ComputerMakeMove(move.Move);
        }
    }

}
