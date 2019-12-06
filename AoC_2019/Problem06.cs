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

            var parent = nodes.Single(n => n.ParentId == default);

            var result = parent.RelationshipCount();

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

            var inputs = new ParsedFile(FilePath).ToList<string>();

            inputs.ForEach(str =>
            {
                IEnumerable<string> items = str.Split(')');
                string centerId = items.First();
                string orbiterId = items.Last();

                Node existingOrbiter = AddOrUpdateOrbiter(nodes, centerId, orbiterId);
                AddOrUpdateCenter(nodes, centerId, existingOrbiter);
            });

            return nodes;
        }

        private static void AddOrUpdateCenter(HashSet<Node> nodes, string centerId, Node existingOrbiter)
        {
            if (nodes.TryGetValue(new Node(centerId), out Node existingCenter))
            {
                existingCenter.Children.Add(existingOrbiter);
            }
            else
            {
                nodes.Add(new Node(centerId, existingOrbiter));
            }
        }

        private static Node AddOrUpdateOrbiter(HashSet<Node> nodes, string centerId, string orbiterId)
        {
            if (nodes.TryGetValue(new Node(orbiterId), out Node existingOrbiter))
            {
                if (existingOrbiter.ParentId == default)
                {
                    existingOrbiter.ParentId = centerId;
                }
                else
                {
                    throw new SolvingException("Input doesn't look like a tree!!");
                }
            }
            else
            {
                existingOrbiter = new Node(orbiterId) { ParentId = centerId };
                nodes.Add(existingOrbiter);
            }

            return existingOrbiter;
        }
    }
}
