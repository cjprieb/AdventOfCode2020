using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2020.Day16
{
    [TestClass]
    public class Day16
    {
        #region Input

        public IEnumerable<string> GetInput() => Input.GetLines(Properties.Resources.Day16Input);

        public string[] GetTestInput() => new string[]
        {
            "class: 1-3 or 5-7",
            "row: 6-11 or 33-44",
            "seat: 13-40 or 45-50",
            "",
            "your ticket:",
            "7,1,14",
            "",
            "nearby tickets:",
            "7,3,47",
            "40,4,50",
            "55,2,20",
            "38,6,12"
        };

        public string[] GetTestInput2() => new string[]
        {
            "class: 0-1 or 4-19",
            "row: 0-5 or 8-19",
            "seat: 0-13 or 16-19",
            "",
            "your ticket:",
            "11,12,13",
            "",
            "nearby tickets:",
            "3,9,18",
            "15,1,5",
            "5,14,9"
        };

        #endregion Input

        #region Code...

        #region CalculateTicketScanningErrorRate
        private int CalculateTicketScanningErrorRate(IEnumerable<string> input)
        {
            var translation = Translation.Parse(input);
            return translation.InvalidSum;
        }
        #endregion CalculateTicketScanningErrorRate

        #region CalculateProductOfFields
        private long CalculateProductOfFields(IEnumerable<string> input, string fieldSearch = "")
        {
            var translation = Translation.Parse(input);
            translation.FindFieldIndexes();
            return translation.GetProduct(fieldSearch);
        }
        #endregion CalculateProductOfFields

        #endregion Code...

        #region Tests...

        [TestMethod] public void Test1() => Assert.AreEqual(71, CalculateTicketScanningErrorRate(GetTestInput()));

        [TestMethod] public void Answer1() => Assert.AreEqual(21980, CalculateTicketScanningErrorRate(GetInput()));

        [TestMethod] public void Test2a() => Assert.AreEqual(98, CalculateProductOfFields(GetTestInput()));

        [TestMethod] public void Test2b() => Assert.AreEqual(1716, CalculateProductOfFields(GetTestInput2()));

        [TestMethod] public void Answer2() => Assert.AreEqual(1439429522627L, CalculateProductOfFields(GetInput(), "departure"));
        #endregion Tests...
    }

    public enum ParseState
    {
        Rule, MyTicket, NearbyTicket, Done
    }

    public class Translation
    {
        private Dictionary<string, int> _FieldIndexes = new Dictionary<string, int>();

        public List<Rule> Rules { get; private set; } = new List<Rule>();        
        public List<Ticket> ValidTickets { get; private set; } = new List<Ticket>();
        public Ticket MyTicket { get; private set; } = null;
        public int InvalidSum { get; private set; } = 0;

        internal long GetProduct(string fieldSearch)
        {
            int[] indexes = _FieldIndexes.Values.ToArray();
            if (!string.IsNullOrEmpty(fieldSearch))
            {
                indexes = _FieldIndexes.Where(kvp => kvp.Key.Contains(fieldSearch)).Select(kvp=> kvp.Value).ToArray();
            }
            long product = 1L;
            foreach (var i in indexes)
            {
                product *= MyTicket.Values[i];
            }
            return product;
        }

        public void FindFieldIndexes()
        {
           Dictionary<string, HashSet<int>> possibleIndexes = new Dictionary<string, HashSet<int>>();
            foreach (var rule in Rules)
            {
                var fieldName = rule.Name;
                possibleIndexes[fieldName] = new HashSet<int>();
                for (int i = 0; i < Rules.Count; i++)
                {
                    if (ValidTickets.All(ticket => rule.IsValidValue(ticket.Values[i])))
                    {
                        possibleIndexes[fieldName].Add(i);
                    }
                    else
                    {
                        if (fieldName == "type"  || fieldName == "train")
                        {
                            var invalid = ValidTickets.First(ticket => !rule.IsValidValue(ticket.Values[i]));
                            Console.WriteLine($"First mismatch for {fieldName} at {i}: {invalid.Values[i]}");
                        }
                    }
                }
            }

            bool stop = false;
            while (_FieldIndexes.Count < Rules.Count && !stop)
            {
                foreach (var kvp in possibleIndexes)
                {
                    foreach (var key in _FieldIndexes.Keys)
                    {
                        kvp.Value.Remove(_FieldIndexes[key]);
                    }

                    if (kvp.Value.Count == 1)
                    {
                        _FieldIndexes[kvp.Key] = kvp.Value.First();
                    }
                    else if (kvp.Value.Count == 0)
                    {
                        stop = true;
                    }
                }

                foreach (var key in _FieldIndexes.Keys)
                {
                    possibleIndexes.Remove(key);
                }
            }
        }

        public static Translation Parse(IEnumerable<string> input)
        {
            var translation = new Translation();
            ParseState state = ParseState.Rule;
            foreach (var line in input)
            {
                if (line.StartsWith("your ticket"))
                {
                    state = ParseState.MyTicket;
                }
                else if (line.StartsWith("nearby tickets"))
                {
                    state = ParseState.NearbyTicket;
                }
                else if (line.Length > 0)
                {
                    if (state == ParseState.Rule)
                    {
                        translation.Rules.Add(Rule.Parse(line));
                    }
                    else if (state == ParseState.NearbyTicket)
                    {
                        var ticket = Ticket.Parse(line);
                        var invalidValues = ticket.GetInvalidValues(translation.Rules);
                        if (!invalidValues.Any())
                        {
                            translation.ValidTickets.Add(ticket);
                        }
                        else
                        {
                            translation.InvalidSum += invalidValues.Sum();
                        }
                    }
                    else if (state == ParseState.MyTicket)
                    {
                        translation.MyTicket = Ticket.Parse(line);
                    }
                }
            }

            return translation;
        }
    }


    public class Ticket
    {
        public List<int> Values { get; set; } = new List<int>();

        public static Ticket Parse(string line)
        {
            var tokens = line.Split(',');
            var ticket = new Ticket();
            foreach (var t in tokens)
            {
                ticket.Values.Add(int.Parse(t));
            }
            return ticket;
        }

        public IEnumerable<int> GetInvalidValues(IEnumerable<Rule> rules)
        {
            foreach (var value in Values)
            {
                if (!rules.Any(rule => rule.IsValidValue(value)))
                {
                    yield return value;
                }
            }
        }
    }

    public class Rule
    {
        public string Name { get; private set; }
        public List<MyRange> ValidRanges { get; private set; } = new List<MyRange>();

        public Rule(string name)
        {
            Name = name;
        }

        public static Rule Parse(string line)
        {
            var tokens = line.Split(':');
            var rule = new Rule(tokens[0]);
            var regex = new Regex(@"(\d+)\-(\d+)");
            foreach (var rangeString in tokens[1].Split("or"))
            {
                Match match = regex.Match(rangeString);
                if (match.Success)
                {
                    var min = int.Parse(match.Groups[1].Value);
                    var max = int.Parse(match.Groups[2].Value);
                    rule.ValidRanges.Add(new MyRange(min, max));
                }
            }
            return rule;
        }

        public bool IsValidValue(int value)
        {
            return ValidRanges.Any(range => value >= range.Start && value <= range.End);
        }

        public override string ToString()
        {
            return $"{Name} {string.Join(",", ValidRanges.Select(range => range.ToString()))}";
        }
    }

    public struct MyRange
    {
        public int Start;
        public int End;
        public MyRange(int start, int end)
        {
            Start = start;
            End = end;
        }

        public override string ToString()
        {
            return $"{Start}-{End}";
        }
    }
}
