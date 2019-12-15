﻿using AoC_2019.Arcade.Pieces;
using AoC_2019.IntCode;
using AoCHelper;
using AoCHelper.Model;
using EasyAsyncCancel;
using MersenneTwister;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace AoC_2019.Arcade
{
    public class ArcadeMachine
    {
        private readonly List<long> _originalCode;
        private readonly IntCodeComputer _intCodeComputer;
        private readonly Channel<long> _userInputChannel;
        private Channel<Piece> _computerOutputChannel;
        private string _grid;

        private static readonly Random _rnd = Randoms.Create(RandomType.FastestInt32);

        public ArcadeMachine(List<long> intCode)
        {
            _grid = CreateBaseGrid((long[])intCode.ToArray().Clone());
            _originalCode = intCode;
            _originalCode[0] = 2;

            _userInputChannel = Channel.CreateUnbounded<long>();
            _computerOutputChannel = Channel.CreateUnbounded<Piece>();
            _intCodeComputer = new IntCodeComputer(_userInputChannel);
        }

        private List<long> GetSourceCode() => ((long[])_originalCode.ToArray().Clone()).ToList();

        public async Task<long> Play(DifficultyLevel difficultyLevel)
        {
            const long xWhenScore = -1;
            const long yWhenScore = 0;

            long score = 0;
            List<long> output = new List<long>(3);
            HashSet<Piece> pieces = new HashSet<Piece>();
            int outputIndex = 0;

            Task.Run(ComputerOutputThread);
            Task.Run(() => UserInputThread(difficultyLevel));

            await foreach (var partialOutput in _intCodeComputer.RunIntCodeProgram(GetSourceCode()))
            {
                output.Add(partialOutput);
                outputIndex++;

                outputIndex %= 3;
                if (outputIndex == 0)
                {
                    if (output[0] == xWhenScore && output[1] == yWhenScore)
                    {
                        score = output[2];
                        Console.WriteLine($"Current score: {score}");
                    }
                    else
                    {
                        Piece piece = GeneratePiece(output[0], output[1], output[2]);
                        if (piece != null)
                        {
                            pieces.Add(piece);
                            _computerOutputChannel.Writer.TryWrite(piece);
                        }
                    }

                    output.Clear();
                }
            }

            Console.WriteLine($"Final score: {score}");

            if (!pieces.Any(p => p is Block))
            {
                Console.WriteLine("Congrats, you won!!");
            }

            return score;
        }

        public async Task<long> PlayCheating()
        {
            const long xWhenScore = -1;
            const long yWhenScore = 0;
            const int inputNumber = 10000;

            int attemptIndex = 1;
            List<int> winningSequence = new List<int>();
            int previousSequenceSize = -1;
            int itemsToSkipFromBestSolution = 0;
            long topScore = 0;
            while (true)
            {
                //Task.Run(ComputerOutputThread);

                List<int> usedSequence = winningSequence.SkipLast(itemsToSkipFromBestSolution).ToList();

                foreach (int n in usedSequence)
                {
                    _userInputChannel.Writer.TryWrite(n);
                }

                for (int i = usedSequence.Count; i < 10000; ++i)
                {
                    int randomInput = GenerateRandomInput();
                    if (_userInputChannel.Writer.TryWrite(randomInput))
                    {
                        usedSequence.Add(randomInput);
                    }
                }

                long score = 0;
                List<long> output = new List<long>(3);
                HashSet<Piece> pieces = new HashSet<Piece>();

                int outputIndex = 0;
                long highestScore = 0;
                await foreach (var partialOutput in _intCodeComputer.RunIntCodeProgram(GetSourceCode()))
                {
                    output.Add(partialOutput);
                    outputIndex++;

                    outputIndex %= 3;
                    if (outputIndex == 0)
                    {
                        if (output[0] == xWhenScore && output[1] == yWhenScore)
                        {
                            score = output[2];
                            highestScore = score > highestScore ? score : highestScore;
                            //Console.WriteLine($"Current score: {score}");
                        }
                        else
                        {
                            Piece piece = GeneratePiece(output[0], output[1], output[2]);
                            if (piece != null)
                            {
                                pieces.Add(piece);
                                _computerOutputChannel.Writer.TryWrite(piece);
                            }
                        }

                        output.Clear();
                    }
                }

                if (highestScore > topScore)
                {
                    topScore = highestScore;
                    Console.WriteLine($"New score: {topScore} (attempt #{attemptIndex})");
                }

                if (!pieces.Any(p => p is Block))
                {
                    return score;
                }

                int unusedInputs = 0;
                while (_userInputChannel.Reader.TryRead(out var _))
                {
                    ++unusedInputs;
                }

                int processedInputs = inputNumber - unusedInputs;

                //if (processedInputs > winningSequence.Count)
                //{
                //    winningSequence = usedSequence.Take(processedInputs).ToList();
                //}

                //if (previousWinningSequenceSize <= winningSequence.Count)
                //{
                //    winningSequence = winningSequence.SkipLast(1).ToList();
                //}
                //previousWinningSequenceSize = winningSequence.Count;

                //Console.WriteLine($"Attempting [{string.Join(",", usedSequence.Take(processedInputs))}]");

                if (processedInputs > winningSequence.Count && processedInputs > 4)
                {
                    winningSequence = usedSequence.Take(processedInputs).ToList();
                }

                if (processedInputs > previousSequenceSize)
                {
                    itemsToSkipFromBestSolution = 0;
                }
                else if (processedInputs == previousSequenceSize)
                {
                    ++itemsToSkipFromBestSolution;
                }
                else
                {
                    //winningSequence = winningSequence.SkipLast(1).ToList();
                    //--itemsToSkipFromBestSolution;
                }


                previousSequenceSize = processedInputs;

                _computerOutputChannel.Writer.Complete();
                _computerOutputChannel = Channel.CreateUnbounded<Piece>();

                Console.WriteLine($"Score (attempt #{attemptIndex}, pieces left: {pieces.Count(p => p is Block)}): {score}");
                ++attemptIndex;
            }
        }

        private static int GenerateRandomInput()
        {
            return _rnd.Next(-1, 2);
        }

        private async Task ComputerOutputThread()
        {
            HashSet<Piece> existingPieces = new HashSet<Piece>();

            await _computerOutputChannel.Reader.WaitToReadAsync().ConfigureAwait(false);
            await foreach (Piece piece in _computerOutputChannel.Reader.ReadAllAsync())
            {
                ICollection<Piece> piecesToUpdate = new List<Piece>(3) { piece };
                if (existingPieces.TryGetValue(piece, out Piece existingPiece))
                {
                    existingPieces.Remove(existingPiece);
                    piecesToUpdate.Add(new EmptyPiece(existingPiece));

                    if (piece is Ball)
                    {
                        Point bouncePosition = existingPiece.Position.X > piece.Position.X
                            ? new Point(piece.Position.X - 1, piece.Position.Y - 1)
                            : new Point(piece.Position.X + 1, piece.Position.Y - 1);

                        Piece block = GeneratePiece(bouncePosition.X, bouncePosition.Y, 2);
                        if (existingPieces.Contains(block))
                        {
                            existingPieces.Remove(block);
                            piecesToUpdate.Add(new EmptyPiece(block));
                        }
                    }
                }

                existingPieces.Add(piece);

                UpdatePieceInGrid(piecesToUpdate);

                Console.Out.Flush();
                Console.WriteLine(_grid);
            }
        }

        private async Task UserInputThread(DifficultyLevel difficultyLevel)
        {
            Dictionary<DifficultyLevel, int> difficultyAutomaticInput = new Dictionary<DifficultyLevel, int>
            {
                [DifficultyLevel.Beginner] = int.MaxValue,
                [DifficultyLevel.Easy] = 5000,
                [DifficultyLevel.Medium] = 2000,
                [DifficultyLevel.Hard] = 1000,
                [DifficultyLevel.Master] = 500
            };

            int userInput = await GetInputAsync(int.MaxValue).ConfigureAwait(false);
            await _userInputChannel.Writer.WriteAsync(userInput).ConfigureAwait(false);

            while (true)
            {
                userInput = await GetInputAsync(difficultyAutomaticInput[difficultyLevel]).ConfigureAwait(false);
                await _userInputChannel.Writer.WriteAsync(userInput).ConfigureAwait(false);
            }
        }

        private async Task<int> GetInputAsync(int milliseconds)
        {
            try
            {
                var input = await Task.Run(Console.ReadKey).CancelAfter(milliseconds).ConfigureAwait(false);

                return input.Key switch
                {
                    ConsoleKey.LeftArrow => -1,
                    ConsoleKey.A => -1,
                    ConsoleKey.RightArrow => +1,
                    ConsoleKey.D => +1,
                    _ => 0
                };

            }
            catch (OperationCanceledException)
            {
                return 0;
            }
        }

        private void UpdatePieceInGrid(IEnumerable<Piece> piecesToUpdate)
        {
            int width = _grid.IndexOf(Environment.NewLine) + Environment.NewLine.Length;
            StringBuilder gridBuilder = new StringBuilder(_grid);

            foreach (Piece piece in piecesToUpdate.Where(p => p != null))
            {
                int position = width * (piece.Position.Y) + piece.Position.X;
                char content = piece.Render();

                if (position >= 0 && position < gridBuilder.Length)
                {
                    gridBuilder[position] = content;
                }
                else
                {
                    throw new SolvingException($"Piece out of grid range: {piece.Position} -> {position}");
                }
            }

            _grid = gridBuilder.ToString();
        }

        private string CreateBaseGrid(IEnumerable<long> intCode)
        {
            var pieces = ExtractPieces(intCode);

            int minX = pieces.Min(p => p.Position.X);
            int maxX = pieces.Max(p => p.Position.X);

            int minY = pieces.Min(p => p.Position.Y);
            int maxY = pieces.Max(p => p.Position.Y);

            if (minX < 0 || minY < 0)
            {
                throw new SolvingException("The position of your pieces looks suspicious");
            }

            StringBuilder sb = new StringBuilder();

            for (int x = 0; x < maxX; ++x)
            {
                for (int y = 0; y <= 2 * maxY; ++y)
                {
                    sb.Append(" ");
                }
                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        public static IEnumerable<Piece> ExtractPieces(IEnumerable<long> intCode)
        {
            var output = IntCodeHelpers.RunIntCodeProgram(intCode.ToList(), Array.Empty<long>()).Result;

            for (int i = 0; i < output.Count; i += 3)
            {
                var piece = GeneratePiece(output.ElementAt(i), output.ElementAt(i + 1), output.ElementAt(i + 2));
                if (piece != null)
                {
                    yield return piece;
                }
            }
        }

        private static Piece GeneratePiece(long x, long y, long id)
        {
            if (_pieceTileId.TryGetValue((int)id, out var pieceFunc))
            {
                Piece piece = pieceFunc.Invoke();
                piece.Init(x, y);

                return piece;
            }
            else
            {
                return null;
            }
        }

        private static readonly Dictionary<int, Func<Piece>> _pieceTileId = new Dictionary<int, Func<Piece>>
        {
            [Wall.Id] = () => new Wall(),
            [Block.Id] = () => new Block(),
            [HorizontalPaddle.Id] = () => new HorizontalPaddle(),
            [Ball.Id] = () => new Ball()
        };
    }
}
