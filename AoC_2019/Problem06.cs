using AoCHelper;
using FileParser;
using System.Collections.Generic;
using System.Linq;
using AoC_2019.Model;

namespace AoC_2019
{
    public class Problem06 : BaseProblem
    {
        public override string Solve_1()
        {
            var nodes = ParseInput();

            var parent = nodes.Single(n => !nodes.Any(otherN => otherN.Children.Contains(n)));

            var result = parent.RelationShipCount();

            return result.ToString();
        }

        public override string Solve_2()
        {
            var input = ParseInput().ToList();

            return "";
        }

        private HashSet<Node> ParseInput()
        {
            HashSet<Node> nodes = new HashSet<Node>();
            HashSet<string> existingNodes = new HashSet<string>();

            var inputs = new ParsedFile(FilePath).ToList<string>();

            inputs.ForEach(str =>
            {
                IEnumerable<string> items = str.Split(')');

                if (existingNodes.Add(items.Last()))
                {
                    nodes.Add(new Node(items.Last()));
                }

                var orbiter = nodes.Single(n => n.Id == items.Last());

                if (existingNodes.Add(items.First()))
                {
                    nodes.Add(new Node(items.First(), orbiter));
                }
                else
                {
                    nodes.Single(n => n.Id == items.First()).Children.Add(orbiter);
                }
            });

            if (existingNodes.Count != nodes.Count)
            {
                throw new SolvingException("Error parsing input");
            }

            return nodes;
        }
    }
}
