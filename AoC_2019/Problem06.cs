using AoCHelper;
using SheepTools.Model;
using FileParser;
using System.Collections.Generic;
using System.Linq;

namespace AoC_2019
{
    public class Problem06 : BaseProblem
    {
        public override string Solve_1()
        {
            var nodes = ParseInput();

            var parent = nodes.Single(n => n.ParentId == default);

            var result = parent.RelationshipsCount();

            return result.ToString();
        }

        public override string Solve_2()
        {
            var nodes = ParseInput().ToList();

            const string me = "YOU";
            const string santa = "SAN";

            Node myLocation = nodes.Single(node => node.Id == me);
            Node santaLocation = nodes.Single(node => node.Id == santa);

            Node commonAncestorBetweenMeAndSanta = GetCommonAncestor(nodes, myLocation, santaLocation);

            int distanceToMe = commonAncestorBetweenMeAndSanta.DistanceTo(myLocation, 0);
            int distanceToSanta = commonAncestorBetweenMeAndSanta.DistanceTo(santaLocation, 0);

            int result = distanceToMe + distanceToSanta - 2;

            return result.ToString();
        }

        private Node GetCommonAncestor(List<Node> nodes, Node myLocation, Node santaLocation)
        {
            HashSet<string> identifiers = new HashSet<string>();

            Node transverseBackwards(List<Node> nodes, ref HashSet<string> identifiers, Node currentNode)
            {
                while (currentNode.ParentId != default)
                {
                    if (!identifiers.Add(currentNode.Id))
                    {
                        break;
                    }

                    currentNode = nodes.Single(node => node.Id == currentNode.ParentId);
                }

                return currentNode;
            }

            transverseBackwards(nodes, ref identifiers, myLocation);

            Node commonAncestor = transverseBackwards(nodes, ref identifiers, santaLocation);

            return commonAncestor;
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
