using AoCHelper;
using FileParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoC_2019
{
    public class Problem08 : BaseProblem
    {
        private const int Width = 25;
        private const int Length = 6;

        public override string Solve_1()
        {
            var layers = ParseInput().ToList();

            var layerWithFewest0 = layers.Single(lay => lay.Count(n => n == 0) == layers.Min(l => l.Count(n => n == 0)));

            int result = layerWithFewest0.Count(n => n == 1) * layerWithFewest0.Count(n => n == 2);

            return result.ToString();
        }

        public override string Solve_2()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<List<int>> ParseInput()
        {
            IEnumerable<int> rawInput = new ParsedFile(FilePath)
                .ToSingleString()
                .Select(str => int.Parse(str.ToString()));

            int pixelsPerLayer = Width * Length;

            for (int i = 0; i < rawInput.Count(); i += pixelsPerLayer)
            {
                yield return rawInput.Skip(i).Take(pixelsPerLayer).ToList();
            }
        }
    }
}
