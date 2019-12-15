using AoC_2019.Arcade;
using AoC_2019.Arcade.Pieces;
using AoCHelper;
using FileParser;
using System.Collections.Generic;
using System.Linq;

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

            long score = arcadeMachine.PlayCheating().Result;

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
}
