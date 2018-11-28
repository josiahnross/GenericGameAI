﻿using NeuralNetTreeStuffViewer.MinMaxAlg;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer
{
    public class MinMaxTurnBasedGameInterface
    {
        public ITurnBasedGame Game;
        public ITurnBasedGame DisplayGame;
        public bool AiFirst;
        public uint MakeMoveMinMaxDepth;
        public string DebugStringPath;
        public IEvaluateableTurnBasedGame Evaluator;
        public MinMaxTurnBasedGameInterface(ITurnBasedGame game, ITurnBasedGame displayGame, bool aiFirst, uint makeMoveMinMaxDepth = 5, string debugStringPath = null)
        {
            Game = game;
            DisplayGame = displayGame;
            AiFirst = aiFirst;
            MakeMoveMinMaxDepth = makeMoveMinMaxDepth;
            DebugStringPath = debugStringPath;
        }
    }
    public class MinMaxTurnBasedGameInterface<T, T1> : MinMaxEvaluator<T, T1>
        where T : ITurnBasedGame<T, T1>,new()
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
        public MinMaxTurnBasedGameInterface(MinMaxTurnBasedGameInterface gameInterface)
            :this((T)gameInterface.Game, (T)gameInterface.DisplayGame, gameInterface.AiFirst, gameInterface.MakeMoveMinMaxDepth, gameInterface.DebugStringPath)
        {
            Evaluator = gameInterface.Evaluator.Cast<T,T1>();
            Evaluator.ParentEval = this;
            SetEvaluator(Evaluator);
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
                    MakeMove(e.Info.move, moveIndex, false);
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
                            File.WriteAllText(DebugStringPath, nextMoveChild.Parent.DebugString);
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
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Restart();
                    while (stopwatch.ElapsedMilliseconds < 500) { }
                    stopwatch.Stop();
                }

                if (moveIndex != null)
                {
                    MakeMoveOnGame(new GameMove<T1>(moveIndex.Value.Move, GetAiPlayer()), moveIndex.Value.Index);
                    Players humanPlayer = AiFirst ? Players.OpponentOrSecond : Players.YouOrFirst;
                    var moves = Game.AvailableMoves(humanPlayer);
                    if (moves.Count == 1 && moves.ContainsKey(0))
                    {
                        T1 move = moves[0];
                        MakeMoveOnGame(new GameMove<T1>(move, humanPlayer), 0);
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
            MakeMove(move, moveIndex, false);
            DisplayGame.ComputerMakeMove(move.Move);
        }
        
    }

}
