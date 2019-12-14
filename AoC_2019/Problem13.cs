using System;
using AoCHelper;
using FileParser;
using System.Collections.Generic;
using System.Linq;
using AoC_2019.IntCode;
using AoCHelper.Model;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Text;
using System.Threading;

namespace AoC_2019
{
    public class Problem13 : BaseProblem
    {
        public override string Solve_1()
        {
            var intCode = ParseInput().ToList();

            var pieces = ArcadeMachine.ExtractPieces(intCode).ToList();

            int numberOfBlocks = pieces.Count(p => p is Block);

            return numberOfBlocks.ToString();
        }

        public override string Solve_2()
        {
            var intCode = ParseInput().ToList();

            ArcadeMachine arcadeMachine = new ArcadeMachine(intCode);

            long score = arcadeMachine.Play().Result;

            return score.ToString();
        }

        private IEnumerable<long> ParseInput()
        {
            return new ParsedFile(FilePath)
                .ToSingleString()
                .Split(',')
                .Select(long.Parse);
        }
    }

    #region Pieces

    public abstract class Piece : IEquatable<Piece>
    {
        public string Type => GetType().Name;

        public Point Position { get; set; }

        public void Init(long x, long y)
        {
            Position = new Point((int)x, (int)y);
        }

        public abstract char Render();

        #region Equals override

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is Piece))
            {
                return false;
            }

            return Equals((Piece)obj);
        }

        public virtual bool Equals(Piece other)
        {
            if (other == null)
            {
                return false;
            }

            return Position.Equals(other.Position);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Position);
        }

        #endregion
    }

    public abstract class UniquePiece : Piece
    {
        #region Equals override

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is Piece))
            {
                return false;
            }

            return Equals((Piece)obj);
        }

        public override bool Equals(Piece other)
        {
            if (other == null)
            {
                return false;
            }

            return Type.Equals(other.Type);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type);  // Always unique
        }

        #endregion
    }

    public class Wall : Piece
    {
        public static int Id => 1;

        public override char Render() => Position.Y == 0 ? '—' : '|';
    }

    public class Block : Piece
    {
        public static int Id => 2;
        public override char Render() => '#';
    }

    public class HorizontalPaddle : UniquePiece
    {
        public static int Id => 3;

        public override char Render() => '_';
    }

    public class Ball : UniquePiece
    {
        public static int Id => 4;

        public override char Render() => 'o';
    }

    public class EmptyPiece : Piece
    {
        public override char Render() => ' ';

        public EmptyPiece(Piece piece)
        {
            Position = piece.Position;
        }
    }

    #endregion

    public class ArcadeMachine
    {
        private readonly List<long> _sourceCode;
        private readonly IntCodeComputer _intCodeComputer;
        private readonly Channel<long> _userInputChannel;
        private readonly Channel<Piece> _computerOutputChannel;
        private string _grid;

        public ArcadeMachine(List<long> intCode)
        {
            _grid = CreateBaseGrid(intCode);
            _sourceCode = intCode;
            _sourceCode[0] = 2;

            _userInputChannel = Channel.CreateUnbounded<long>();
            _computerOutputChannel = Channel.CreateUnbounded<Piece>();
            _intCodeComputer = new IntCodeComputer(_userInputChannel);

        }

        public async Task<long> Play()
        {
            const long xWhenScore = -1;
            const long yWhenScore = 0;

            long score = 0;
            List<long> output = new List<long>(3);
            HashSet<Piece> pieces = new HashSet<Piece>();
            int outputIndex = 0;

            Task.Run(ComputerOutputThread);
            Task.Run(UserInputThread);

            await foreach (var partialOutput in _intCodeComputer.RunIntCodeProgram(_sourceCode))
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

        private async Task ComputerOutputThread()
        {
            HashSet<Piece> existingPieces = new HashSet<Piece>();

            while (true)
            {
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
        }

        private async Task UserInputThread()
        {
            while (true)
            {
                int userInput = await GetInputAsync().ConfigureAwait(false);
                await _userInputChannel.Writer.WriteAsync(userInput).ConfigureAwait(false);
            }
        }

        private async Task<int> GetInputAsync()
        {
            CancellationTokenSource src = new CancellationTokenSource(millisecondsDelay: 10000);
            try
            {
                var input = await Task.Run(Console.ReadKey, src.Token).ConfigureAwait(false);

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
