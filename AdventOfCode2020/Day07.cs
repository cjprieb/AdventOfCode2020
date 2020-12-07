using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2020.Day07
{
    [TestClass]
    public class Day07
    {
        #region Input

        IEnumerable<string> GetInput() => Input.GetLines(Properties.Resources.Day07Input);

        IEnumerable<string> GetTestInput() => new string[]
        {
            "light red bags contain 1 bright white bag, 2 muted yellow bags.",
            "dark orange bags contain 3 bright white bags, 4 muted yellow bags.",
            "bright white bags contain 1 shiny gold bag.",
            "muted yellow bags contain 2 shiny gold bags, 9 faded blue bags.",
            "shiny gold bags contain 1 dark olive bag, 2 vibrant plum bags.",
            "dark olive bags contain 3 faded blue bags, 4 dotted black bags.",
            "vibrant plum bags contain 5 faded blue bags, 6 dotted black bags.",
            "faded blue bags contain no other bags.",
            "dotted black bags contain no other bags.",
        };

        IEnumerable<string> GetTestInput2() => new string[]
        {
            "shiny gold bags contain 2 dark red bags.",
            "dark red bags contain 2 dark orange bags.",
            "dark orange bags contain 2 dark yellow bags.",
            "dark yellow bags contain 2 dark green bags.",
            "dark green bags contain 2 dark blue bags.",
            "dark blue bags contain 2 dark violet bags.",
            "dark violet bags contain no other bags.",
        };

        #endregion Input

        #region Code...

        Dictionary<string, Bag> ParseBags(IEnumerable<string> stringRules)
        {
            var result = new Dictionary<string, Bag>();
            foreach (var rule in stringRules)
            {
                var bag = Bag.Parse(rule);
                if (bag != null) result[bag.Color] = bag;
            }
            return result;
        }

        int NumberOfBagsThatContain(IEnumerable<string> rules, string color)
        {
            var bags = ParseBags(rules);
            var bagsThatCanContainColor = new HashSet<string>();

            var colorsProcessed = new HashSet<string>();
            var colorsToSearchFor = new Queue<string>();
            colorsToSearchFor.Enqueue(color);

            while (colorsToSearchFor.Count > 0)
            {
                string nextColor = colorsToSearchFor.Dequeue();
                foreach (var bag in bags.Values)
                {
                    if (bag.AllowableInnerBags.ContainsKey(nextColor) && !colorsProcessed.Contains(bag.Color))
                    {
                        bagsThatCanContainColor.Add(bag.Color);
                        colorsToSearchFor.Enqueue(bag.Color);
                    }
                }
                colorsProcessed.Add(nextColor);
            }

            return bagsThatCanContainColor.Count;
        }

        int NumberOfBagsInside(IEnumerable<string> rules, string color)
        {
            var bags = ParseBags(rules);
            return NumberOfBagsInside(bags, color, 1) - 1; // -1 since the current color shouldn't be included in the count
        }

        int NumberOfBagsInside(Dictionary<string, Bag> bags, string color, int outerBagCount)
        {
            var innerBagCount = 1; // for the current bag

            var bag = bags[color];
            foreach (var kvp in bag.AllowableInnerBags)
            {
                innerBagCount += NumberOfBagsInside(bags, kvp.Key, kvp.Value);
            }

            return outerBagCount * innerBagCount;
        }

        #endregion Code...

        #region Tests...
        [TestMethod] public void Test1() => Assert.AreEqual(4, NumberOfBagsThatContain(GetTestInput(), "shiny gold"));
        [TestMethod] public void Answer1() => Assert.AreEqual(302, NumberOfBagsThatContain(GetInput(), "shiny gold"));


        [TestMethod] public void Test2_a() => Assert.AreEqual(32, NumberOfBagsInside(GetTestInput(), "shiny gold"));
        [TestMethod] public void Test2_b() => Assert.AreEqual(126, NumberOfBagsInside(GetTestInput2(), "shiny gold"));
        [TestMethod] public void Answer2() => Assert.AreEqual(4165, NumberOfBagsInside(GetInput(), "shiny gold"));
        #endregion Tests...
    }

    class Bag
    {
        private static readonly Regex _OuterBagPattern = new Regex("(.+) bags contain (.+).");
        private static readonly Regex _InnerBagPattern = new Regex(@"(\d+) (.+) bags?");

        public string Color { get; set; }
        public Dictionary<string, int> AllowableInnerBags { get; private set; } = new Dictionary<string, int>();
        public static Bag Parse(string rule)
        {
            if (string.IsNullOrEmpty(rule)) return null;

            Match match = _OuterBagPattern.Match(rule);
            if (!match.Success) throw new Exception($"outer bag string \"{rule}\" does not match pattern.");

            Bag bag = new Bag() 
            {
                Color = match.Groups[1].Value
            };
            string innerBags = match.Groups[2].Value;
            if (innerBags != "no other bags")
            {
                foreach (var inner in innerBags.Split(','))
                {
                    Match innerMatch = _InnerBagPattern.Match(inner);
                    if (!innerMatch.Success) throw new Exception($"inner bag string \"{inner}\" does not match pattern.");

                    int innerCount = int.Parse(innerMatch.Groups[1].Value);
                    string innerColor = innerMatch.Groups[2].Value;
                    bag.AllowableInnerBags[innerColor] = innerCount;
                }
            }
            return bag;
        }
    }
}
