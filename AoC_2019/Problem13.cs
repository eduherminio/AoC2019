using System;
using AoCHelper;
using FileParser;
using System.Collections.Generic;
using System.Linq;
using AoC_2019.IntCode;
using AoCHelper.Model;

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

        public override string Solve_2()
        {
            var intCode = ParseInput().ToList();

            return "";
        }

        private IEnumerable<long> ParseInput()
        {
            return new ParsedFile(FilePath)
                .ToSingleString()
                .Split(',')
                .Select(long.Parse);
        }
    }

    public abstract class Piece
    {
        public string Id { get; private set; }

        public Point Position { get; private set; }

        public void Init(long id, long x, long y)
        {
            Id = id.ToString();
            Position = new Point((int)x, (int)y);
        }
    }

    public class Wall : Piece { }

    public class Block : Piece { }

    public class HorizontalPaddle : Piece { }

    public class Ball : Piece { }
}
