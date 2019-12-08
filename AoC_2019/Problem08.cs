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
        private const int PixelsPerLayer = Width * Length;

        public override string Solve_1()
        {
            var layers = ParseInput().ToList();

            List<int> layerWithFewest0 = layers.Single(lay => lay.Count(n => n == 0) == layers.Min(l => l.Count(n => n == 0)));

            int result = layerWithFewest0.Count(n => n == 1) * layerWithFewest0.Count(n => n == 2);

            return result.ToString();
        }

        public override string Solve_2()
        {
            var layers = ParseInput().ToList();

            List<int> visiblePixels = ExtractVisiblePixels(layers).ToList();

            return PrintVisiblePixels(visiblePixels);
        }

        private static IEnumerable<int> ExtractVisiblePixels(ICollection<List<int>> layers)
        {
            for (int pixelIndex = 0; pixelIndex < PixelsPerLayer; ++pixelIndex)
            {
                yield return layers.FirstOrDefault(layer => layer[pixelIndex] != 2)?.ElementAt(pixelIndex) ?? 2;
            }
        }

        private static string PrintVisiblePixels(IEnumerable<int> layer)
        {
            if (layer.Count() != PixelsPerLayer)
            {
                throw new SolvingException(
                    $"Error calculating visible pixels layer: length can't be {layer.Count()}, but {PixelsPerLayer}");
            }

            if (layer.Contains(2))
            {
                throw new SolvingException("Some pixels are transparent in all layers!?");
            }

            StringBuilder sb = new StringBuilder(Environment.NewLine);
            for (int pixelIndex = 0; pixelIndex < layer.Count(); ++pixelIndex)
            {
                if (pixelIndex % Width == 0)
                {
                    sb.Append(Environment.NewLine);
                }

                sb.Append(layer.ElementAt(pixelIndex) == 1
                    ? "o"
                    : " ");
            }

            return sb.ToString();
        }

        private IEnumerable<List<int>> ParseInput()
        {
            IEnumerable<int> rawInput = new ParsedFile(FilePath)
                .ToSingleString()
                .Select(str => int.Parse(str.ToString()));

            for (int i = 0; i < rawInput.Count(); i += PixelsPerLayer)
            {
                yield return rawInput.Skip(i).Take(PixelsPerLayer).ToList();
            }
        }
    }
}
