using System;
using AoCHelper;
using FileParser;
using System.Collections.Generic;
using System.Linq;
using AoC_2019.IntCode;
using AoCHelper.Model;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace AoC_2019
{
    public class Problem13 : BaseProblem
    {
        private static readonly Dictionary<int, Piece> _pieceTileId = new Dictionary<int, Piece>
        {
            [1] = new Wall(),
            [2] = new Block(),
            [3] = new HorizontalPaddle(),
            [4] = new Ball()
        };

        public override string Solve_1()
        {
            var intCode = ParseInput().ToList();

            var pieces = ExtractPieces(intCode).ToList();

            int numberOfBlocks = pieces.Count(p => p is Block);

            return numberOfBlocks.ToString();
        }

        public override string Solve_2()
        {
            var intCode = ParseInput().ToList();

            long score = PlayArcade(intCode).Result;

            return score.ToString();
        }

        private static IEnumerable<Piece> ExtractPieces(IEnumerable<long> intCode)
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
            if (_pieceTileId.TryGetValue((int)id, out Piece piece))
            {
                piece.Init(id, x, y);
                return piece;
            }
            else
            {
                return null;
            }
        }

        private async Task<long> PlayArcade(List<long> intCode)
        {
            // Game rules
            intCode[0] = 2;
            const long xWhenScore = -1;
            const long yWhenScore = 0;

            Channel<long> channel = Channel.CreateUnbounded<long>();
            IntCodeComputer computer = new IntCodeComputer(channel);

            long score = 0;
            List<long> output = new List<long>(3);
            HashSet<Piece> pieces = new HashSet<Piece>();
            int outputIndex = 0;
            await foreach (var partialOutput in computer.RunIntCodeProgram(intCode.ToList()))
            {
                output.Add(partialOutput);
                outputIndex++;

                outputIndex %= 3;
                if (outputIndex == 0)
                {
                    if (output[0] == xWhenScore && output[1] == yWhenScore)
                    {
                        score = output[2];
                    }
                    else
                    {
                        Piece updatedPiece = GeneratePiece(output[0], output[1], output[2]);
                        if (!pieces.TryGetValue(updatedPiece, out Piece existingPiece))
                        {
                            pieces.Add(updatedPiece);
                        }
                        else
                        {
                            existingPiece.Position.X = updatedPiece.Position.X;
                            existingPiece.Position.Y = updatedPiece.Position.Y;
                        }
                    }

                    output = new List<long>(3);
                }
            }

            return score;
        }

        private IEnumerable<long> ParseInput()
        {
            return new ParsedFile(FilePath)
                .ToSingleString()
                .Split(',')
                .Select(long.Parse);
        }
    }

    public abstract class Piece : IEquatable<Piece>
    {
        public string Id { get; private set; }

        public Point Position { get; private set; }

        public void Init(long id, long x, long y)
        {
            Id = id.ToString();
            Position = new Point((int)x, (int)y);
        }

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

        public bool Equals(Piece other)
        {
            if (other == null)
            {
                return false;
            }

            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        #endregion
    }

    public class Wall : Piece { }

    public class Block : Piece { }

    public class HorizontalPaddle : Piece { }

    public class Ball : Piece { }
}
