using AoC_2019.IntCode;
using AoC_2019.Model;
using AoCHelper;
using FileParser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC_2019
{
    public class Problem14 : BaseProblem
    {
        private const string Fuel = "FUEL";
        private const string Ore = "ORE";

        private ICollection<Formula> _formulas;

        public override string Solve_1()
        {
            _formulas = ParseInput().ToList();

            if (_formulas.Select(f => _formulas.Count(form => form.Result == f.Result)).ToHashSet().Count != 1)
            {
                throw new SolvingException("Multiple formulas have the same component as result, aka, you're fucked");
            }

            Dictionary<Element, int> excedent = new Dictionary<Element, int>(
                _formulas
                .SelectMany(f => f.Reactives.Select(r => r.Element))
                .ToHashSet()
                .Select(element => new KeyValuePair<Element, int>(element, 0)));

            var fuelFormula = _formulas.Single(f => f.Result.Element.Id == Fuel);

            excedent.Add(fuelFormula.Result.Element, 0);

            int result = CalculateRealOre(fuelFormula.Result, excedent);

            return result.ToString();
        }

        private int CalculateIdealOre(Component component)
        {
            return component.Element.Id == Ore
                ? component.Quantity
                : component.Quantity *
                    _formulas
                        .Where(f => f.Result.Element == component.Element)
                        .Min(f => f.Reactives.Sum(CalculateIdealOre) / f.Result.Quantity);
        }

        private int CalculateRealOre(Component component, Dictionary<Element, int> excedent, int numberofComponents = 1)
        {
            int elementsToCreate = component.Quantity * numberofComponents;

            if (component.Element.Id == Ore)
            {
                return elementsToCreate;
            }

            if (excedent.TryGetValue(component.Element, out int existing))
            {
                if (existing >= elementsToCreate)
                {
                    excedent[component.Element] = existing - elementsToCreate;
                    elementsToCreate = 0;
                }
                else
                {
                    excedent[component.Element] = 0;
                    elementsToCreate -= existing;
                }
            }

            if (elementsToCreate == 0)
            {
                return 0;
            }

            Formula formula = _formulas.Single(f => f.Result.Element.Equals(component.Element));

            int reactionsToStart = 1;
            if (formula.Result.Quantity < elementsToCreate)
            {
                reactionsToStart =
                    elementsToCreate % formula.Result.Quantity == 0
                    ? elementsToCreate / formula.Result.Quantity
                    : (elementsToCreate / formula.Result.Quantity) + 1;
            }
            int elementExcedent = (reactionsToStart * formula.Result.Quantity) - elementsToCreate;

            excedent[component.Element] += elementExcedent;

            return formula.Reactives.Sum(r => CalculateRealOre(r, excedent, reactionsToStart));
        }

        public override string Solve_2()
        {
            _formulas = ParseInput().ToList();

            return "";
        }

        private IEnumerable<Formula> ParseInput()
        {
            var file = new ParsedFile(FilePath);

            while (!file.Empty)
            {
                var line = file.NextLine();
                string str = line.ToSingleString();
                var parts = str.Split("=>");

                var reactives = parts.First().Split(",").Select(r => new Component(r));
                var result = new Component(parts.Last());

                yield return new Formula(reactives.ToList(), result);
            }
        }
    }

    public class Formula
    {
        public ICollection<Component> Reactives { get; set; }

        public Component Result { get; set; }

        public Formula(ICollection<Component> reactives, Component result)
        {
            Reactives = reactives;
            Result = result;
        }
    }

    public class Component
    {
        public Element Element { get; set; }

        public int Quantity { get; set; }

        public Component(string representation)
        {
            string formattedRepresentation = representation.Trim().Replace(" ", string.Empty);

            string number = string.Empty;
            for (int i = 0; i < formattedRepresentation.Length; ++i)
            {
                if (int.TryParse(formattedRepresentation[i].ToString(), out int n))
                {
                    number += n.ToString();
                    continue;
                }

                break;
            }
            if (string.IsNullOrWhiteSpace(number))
            {
                throw new SolvingException($"Error parsing component [{representation}]");
            }

            Quantity = int.Parse(number);
            Element = new Element(formattedRepresentation.Substring(number.Length));
        }

        /// <summary>
        /// 'Clone' ctor
        /// </summary>
        /// <param name="component"></param>
        /// <param name="quantity"></param>
        public Component(Component component, int quantity)
        {
            Element = component.Element;
            Quantity = quantity;
        }
    }

    public class Element : IEquatable<Element>
    {
        public string Id { get; set; }

        public Element(string id)
        {
            Id = id;
        }

        #region Equals override
        // https://docs.microsoft.com/en-us/visualstudio/code-quality/ca1815-override-equals-and-operator-equals-on-value-types?view=vs-2017

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is Point))
            {
                return false;
            }

            return Equals((Point)obj);
        }

        public bool Equals(Element other)
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
}
