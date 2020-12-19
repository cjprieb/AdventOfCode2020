using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2020.Day19
{
    [TestClass]
    public class Day19
    {
        #region Input

        public IEnumerable<string> GetInput() => Input.GetLines(Properties.Resources.Day19Input);

        public IEnumerable<string> GetTestInput() => new string[] 
        {
            "0: 6 5",
            "1: 2 3 | 3 2",
            "2: 4 4 | 5 5",
            "3: 4 5 | 5 4",
            "4: \"a\"",
            "5: \"b\"",
            "6: 4 1",
            "",
            "ababbb",
            "bababa",
            "abbbab",
            "aaabbb",
            "aaaabbb"
        };

        public IEnumerable<string> GetTestInput2() => new string[]
        {
            "42: 9 14 | 10 1",
            "9: 14 27 | 1 26",
            "10: 23 14 | 28 1",
            "1: \"a\"",
            "11: 42 31",
            "5: 1 14 | 15 1",
            "19: 14 1 | 14 14",
            "12: 24 14 | 19 1",
            "16: 15 1 | 14 14",
            "31: 14 17 | 1 13",
            "6: 14 14 | 1 14",
            "2: 1 24 | 14 4",
            "0: 8 11",
            "13: 14 3 | 1 12",
            "15: 1 | 14",
            "17: 14 2 | 1 7",
            "23: 25 1 | 22 14",
            "28: 16 1",
            "4: 1 1",
            "20: 14 14 | 1 15",
            "3: 5 14 | 16 1",
            "27: 1 6 | 14 18",
            "14: \"b\"",
            "21: 14 1 | 1 14",
            "25: 1 1 | 1 14",
            "22: 14 14",
            "8: 42",
            "26: 14 22 | 1 20",
            "18: 15 15",
            "7: 14 5 | 1 21",
            "24: 14 1",
            "",
            "abbbbbabbbaaaababbaabbbbabababbbabbbbbbabaaaa",
            "bbabbbbaabaabba",
            "babbbbaabbbbbabbbbbbaabaaabaaa",
            "aaabbbbbbaaaabaababaabababbabaaabbababababaaa",
            "bbbbbbbaaaabbbbaaabbabaaa",
            "bbbababbbbaaaaaaaabbababaaababaabab",
            "ababaaaaaabaaab",
            "ababaaaaabbbaba",
            "baabbaaaabbaaaababbaababb",
            "abbbbabbbbaaaababbbbbbaaaababb",
            "aaaaabbaabaaaaababaa",
            "aaaabbaaaabbaaa",
            "aaaabbaabbaaaaaaabbbabbbaaabbaabaaa",
            "babaaabbbaaabaababbaabababaaab",
            "aabbbbbaabbbaaaaaabbbbbababaaaaabbaaabba",
        };

        #endregion Input

        #region Code...

        #region CountMatchingMessages
        private int CountMatchingMessages(IEnumerable<string> input, bool part2 = false)
        {
            (RuleSet ruleset, IEnumerable<string> messages) = Parse(input, part2);
            return messages.Count(message => ruleset.DoesMessageMatchRules(message));
        }
        #endregion CountMatchingMessages

        #region DoesMessageMatchRules
        private bool DoesMessageMatchRules(IEnumerable<string> input, string message, bool part2 = false, int startingRule = 0)
        {
            (RuleSet ruleset, IEnumerable<string> _) = Parse(input, part2);
            return ruleset.DoesMessageMatchRules(message, startingRule);
        }
        #endregion DoesMessageMatchRules

        #region Parse
        private (RuleSet ruleset, IEnumerable<string> messages) Parse(IEnumerable<string> input, bool part2 = false)
        {
            RuleSet rules = new RuleSet();
            List<string> messages = new List<string>();
            bool doneWithRules = false;
            foreach (var line in input)
            {
                if (string.IsNullOrEmpty(line))
                {
                    doneWithRules = true;
                }
                else if (doneWithRules)
                {
                    messages.Add(line);
                }
                else
                {
                    rules.AddRuleFromString(line);
                }
            }
            if (part2)
            {
                rules.Fix();
            }
            else
            {
                rules.CalculatePossibleLengths();
            }
            rules.Reduce();
            return (rules, messages);
        }
        #endregion Parse

        #endregion Code...

        #region Tests...
        [TestMethod] public void MatchingMessage_ababbb() => Assert.IsTrue(DoesMessageMatchRules(GetTestInput(), "ababbb"));
        [TestMethod] public void MatchingMessage_abbbab() => Assert.IsTrue(DoesMessageMatchRules(GetTestInput(), "abbbab"));
        [TestMethod] public void MatchingMessage_bababa() => Assert.IsFalse(DoesMessageMatchRules(GetTestInput(), "bababa"));
        [TestMethod] public void MatchingMessage_aaabbb() => Assert.IsFalse(DoesMessageMatchRules(GetTestInput(), "aaabbb"));
        [TestMethod] public void MatchingMessage_aaaabbb() => Assert.IsFalse(DoesMessageMatchRules(GetTestInput(), "aaaabbb"));
        [TestMethod] public void Test1() => Assert.AreEqual(2, CountMatchingMessages(GetTestInput()));
        [TestMethod] public void Answer1() => Assert.AreEqual(129, CountMatchingMessages(GetInput()));

        [TestMethod] public void MatchingMessage_repeats() => Assert.IsTrue(DoesMessageMatchRules(GetTestInput2(), "babbbbaabbbbbabbbbbbaabaaabaaa", true));
        [TestMethod] public void MatchingMessage_repeats2() => Assert.IsTrue(DoesMessageMatchRules(GetTestInput2(), "bbbbbbbaaaabbbbaaabbabaaa", true));
        [TestMethod] public void Test2() => Assert.AreEqual(3, CountMatchingMessages(GetTestInput2(), false));
        [TestMethod] public void Test3() => Assert.AreEqual(12, CountMatchingMessages(GetTestInput2(), true));
        /// <summary>
        /// 283 is too high
        /// </summary>
        [TestMethod] public void Answer2() => Assert.AreEqual(243, CountMatchingMessages(GetInput(), true));
        #endregion Tests...

        #region Sub Tests...

        [TestMethod] public void SubTest1_42baabb() => Assert.IsTrue(DoesMessageMatchRules(GetTestInput2(), "baabb", true, 42));
        [TestMethod] public void SubTest1_42aaaab() => Assert.IsTrue(DoesMessageMatchRules(GetTestInput2(), "aaaab", true, 42));
        [TestMethod] public void SubTest1_42baaaa() => Assert.IsTrue(DoesMessageMatchRules(GetTestInput2(), "baaaa", true, 42));
        [TestMethod] public void SubTest1_42babba() => Assert.IsFalse(DoesMessageMatchRules(GetTestInput2(), "babba", true, 42));
        [TestMethod] public void SubTest1_31babba() => Assert.IsTrue(DoesMessageMatchRules(GetTestInput2(), "babba", true, 31));
        [TestMethod] public void SubTest1_31ababb() => Assert.IsTrue(DoesMessageMatchRules(GetTestInput2(), "ababb", true, 31));

        #endregion Sub Tests...
    }

    public abstract class Rule 
    {
        #region Properties...
        public int Id { get; set; }
        #endregion Properties...

        #region DoesMessageMatchRule
        public abstract bool DoesMessageMatchRule(RuleSet ruleset, string message, int start, int length);

        #endregion DoesMessageMatchRule

        #region GetLengths
        public abstract HashSet<int> GetLengths(RuleSet ruleSet);
        #endregion GetLengths

        #region Reduce
        public abstract CharacterRule Reduce(RuleSet ruleset);
        #endregion Reduce
    }

    public class CharacterRule : Rule
    {
        #region Properties...
        public string Value { get; set; }
        #endregion Properties...

        #region DoesMessageMatchRule
        public override bool DoesMessageMatchRule(RuleSet ruleset, string message, int start, int length)
        {
            if (length != Value.Length) return false;
            return message.Substring(start, length) == Value;
        }
        #endregion DoesMessageMatchRule

        #region GetLengths
        public override HashSet<int> GetLengths(RuleSet ruleSet) => new HashSet<int> { Value.Length };
        #endregion GetLengths

        #region Reduce
        public override CharacterRule Reduce(RuleSet ruleset) => null;
        #endregion Reduce

        #region ToString
        public override string ToString()
        {
            return Value;
        }
        #endregion ToString
    }

    public class ConcatRule : Rule
    {
        #region Properties...
        public List<int> SubRuleIds { get; set; } = new List<int>();
        #endregion Properties...

        #region DoesMessageMatchRule
        public override bool DoesMessageMatchRule(RuleSet ruleset, string message, int start, int length)
        {
            if (SubRuleIds.Count == 1)
            {
                return ruleset.Rules[SubRuleIds[0]].DoesMessageMatchRule(ruleset, message, start, length);
            }
            else if (SubRuleIds.Count == 2 && length >= 2)
            {
                bool isValid = false;
                for (int subLength = 1; subLength < length; subLength++)
                {
                    isValid = ruleset.Rules[SubRuleIds[0]].DoesMessageMatchRule(ruleset, message, start, subLength);
                    isValid = isValid && ruleset.Rules[SubRuleIds[1]].DoesMessageMatchRule(ruleset, message, start + subLength, length - subLength);
                    if (isValid) break;
                }
                return isValid;
            }
            else if (SubRuleIds.Count == 3 && length >= 3)
            {
                throw new NotImplementedException("Rule for count 3 not implemented");
            }
            else if (SubRuleIds.Count > 3)
            {
                throw new NotImplementedException("ConcatRule does not support more than 3 subrules");
            }
            return false;
        }
        #endregion DoesMessageMatchRule

        #region GetLengths
        public override HashSet<int> GetLengths(RuleSet ruleSet)
        {
            HashSet<int> result = new HashSet<int>();
            HashSet<int> set1 = ruleSet.Rules[SubRuleIds[0]].GetLengths(ruleSet);
            HashSet<int> set2 = null;
            if (SubRuleIds.Count >= 2) set2 = ruleSet.Rules[SubRuleIds[1]].GetLengths(ruleSet);
            foreach (var x in set1)
            {
                if (set2 == null) result.Add(x);
                else
                {
                    foreach (var y in set2)
                    {
                        result.Add(x + y);
                    }
                }
            }
            return result;
        }
        #endregion GetLengths

        #region Reduce
        public override CharacterRule Reduce(RuleSet ruleset)
        {
            CharacterRule newRule = null;
            if (SubRuleIds.All(id => (ruleset.Rules[id] is CharacterRule)))
            {
                StringBuilder builder = new StringBuilder();
                foreach (var id in SubRuleIds)
                {
                    if (ruleset.Rules[id] is CharacterRule charRule)
                    {
                        builder.Append(charRule.Value);
                    }
                }
                newRule = new CharacterRule()
                {
                    Id = Id,
                    Value = builder.ToString()
                };
            }
            return newRule;
        }
        #endregion Reduce

        #region ToString
        public override string ToString()
        {
            return string.Join(" ", SubRuleIds);
        }
        #endregion ToString
    }

    public class OrRule : Rule
    {
        #region Properties...
        public List<Rule> SubRules { get; private set; } = new List<Rule>();
        #endregion Properties...

        #region DoesMessageMatchRule
        public override bool DoesMessageMatchRule(RuleSet ruleset, string message, int start, int length)
        {
            return SubRules.Any(rule => rule.DoesMessageMatchRule(ruleset, message, start, length));
        }
        #endregion DoesMessageMatchRule

        #region GetLengths
        public override HashSet<int> GetLengths(RuleSet ruleSet)
        {
            HashSet<int> result = new HashSet<int>();
            foreach (var rule in SubRules)
            {
                foreach (var i in rule.GetLengths(ruleSet))
                {
                    result.Add(i);
                }
            }
            return result;
        }
        #endregion GetLengths

        #region Reduce
        public override CharacterRule Reduce(RuleSet ruleset)
        {
            var newSubRules = new List<Rule>();
            foreach (var subrule in SubRules)
            {
                var newrule = subrule.Reduce(ruleset);
                newSubRules.Add((newrule != null) ? newrule : subrule);
            }
            SubRules = newSubRules;
            return null;
        }
        #endregion Reduce

        #region ToString
        public override string ToString()
        {
            return string.Join(" | ", SubRules);
        }
        #endregion ToString
    }

    public class RepeatingRule : Rule
    {
        #region Member Variables...
        private int? _RuleLength = null;
        #endregion Member Variables...

        #region Properties...
        public Rule LeftRule { get; set; }
        #endregion Properties...

        #region DoesMessageMatchRule
        public override bool DoesMessageMatchRule(RuleSet ruleset, string message, int start, int length)
        {
            if (_RuleLength == null)
            {
                var lengths = LeftRule.GetLengths(ruleset);
                if (lengths.Count != 1) throw new Exception($"More than one length returned for rule {LeftRule}: {lengths.Count}");

                _RuleLength = lengths.First();
            }
            if ((length % _RuleLength.Value) != 0) return false;

            bool isMatch = true;
            for (int i = start; i < length; i += _RuleLength.Value)
            { 
                isMatch = isMatch && LeftRule.DoesMessageMatchRule(ruleset, message, i, _RuleLength.Value);
            }
            return isMatch;
        }
        #endregion DoesMessageMatchRule

        #region GetLengths
        public override HashSet<int> GetLengths(RuleSet ruleSet)
        {
            throw new NotImplementedException();
        }
        #endregion GetLengths

        #region Reduce
        public override CharacterRule Reduce(RuleSet ruleset) => null;
        #endregion Reduce

        #region ToString
        public override string ToString()
        {
            return $"{LeftRule.Id} | {LeftRule.Id} {Id}";
        }
        #endregion ToString
    }

    public class NestingRule : Rule
    {
        #region Member Variables...
        private int? _RuleLength = null;
        #endregion Member Variables...

        #region Properties...
        public Rule LeftRule { get; set; }
        public Rule RightRule { get; set; }
        #endregion Properties...

        #region DoesMessageMatchRule
        public override bool DoesMessageMatchRule(RuleSet ruleset, string message, int start, int length)
        {
            if (_RuleLength == null)
            {
                var lengths = LeftRule.GetLengths(ruleset);
                if (lengths.Count != 1) throw new Exception($"More than one length returned for rule {LeftRule}: {lengths.Count}");

                _RuleLength = lengths.First();

                lengths = RightRule.GetLengths(ruleset);
                if (lengths.Count != 1) throw new Exception($"More than one length returned for rule {RightRule}: {lengths.Count}");
                if (lengths.First() != _RuleLength.Value) throw new Exception($"Length returned for rule {RightRule} does not match {LeftRule}: {lengths.Count}");
            }

            if ((length % (2 * _RuleLength.Value)) != 0) return false;

            bool isMatch = true;
            int rightIndex = start + length - _RuleLength.Value;
            for (int leftIndex = start; leftIndex < rightIndex && isMatch; leftIndex += _RuleLength.Value)
            {
                isMatch = isMatch && LeftRule.DoesMessageMatchRule(ruleset, message, leftIndex, _RuleLength.Value);
                isMatch = isMatch && RightRule.DoesMessageMatchRule(ruleset, message, rightIndex, _RuleLength.Value);

                rightIndex -= _RuleLength.Value;
            }
            return isMatch;
        }
        #endregion DoesMessageMatchRule

        #region GetLengths
        public override HashSet<int> GetLengths(RuleSet ruleSet)
        {
            throw new NotImplementedException();
        }
        #endregion GetLengths

        #region Reduce
        public override CharacterRule Reduce(RuleSet ruleset) => null;
        #endregion Reduce

        #region ToString
        public override string ToString()
        {
            return $"{LeftRule.Id} {RightRule.Id} | {LeftRule.Id} {Id} {RightRule.Id}";
        }
        #endregion ToString
    }

    public class RuleSet
    {
        #region Member Variables...
        private int _Count = 0;
        private HashSet<int> _ValidLengths = new HashSet<int>();
        #endregion Member Variables...

        #region Properties...
        public Dictionary<int, Rule> Rules { get; private set; } = new Dictionary<int, Rule>();
        #endregion Properties...

        #region AddRuleFromString
        internal void AddRuleFromString(string line)
        {
            var tokens = line.Split(':');
            var id = int.Parse(tokens[0]);
            if (tokens[1].Contains('\"'))
            {
                Rules[id] = new CharacterRule()
                {
                    Id = id,
                    Value = tokens[1].Trim().Substring(1, 1)
                };
            }
            else
            {
                var subrules = tokens[1].Split('|');
                var concatRules = subrules.Select(x => ParseIds(id, x)).ToArray();
                if (concatRules.Length == 1)
                {
                    Rules[id] = concatRules[0];
                }
                else
                {
                    var rule = new OrRule()
                    {
                        Id = id,
                    };
                    rule.SubRules.AddRange(concatRules);
                    Rules[id] = rule;
                }
            }
        }
        #endregion AddRuleFromString

        #region CalculatePossibleLengths
        public void CalculatePossibleLengths(int start = 0)
        {
            _ValidLengths = Rules[0].GetLengths(this);
        }
        #endregion CalculatePossibleLengths

        #region DoesMessageMatchRules
        internal bool DoesMessageMatchRules(string message, int startingRule = 0)
        {
            if (_ValidLengths.Any() && !_ValidLengths.Contains(message.Length)) return false;

            if (Rules[startingRule].DoesMessageMatchRule(this, message, 0, message.Length))
            {
                _Count++;
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion DoesMessageMatchRules

        #region Fix
        public void Fix()
        {
            Rules[8] = new RepeatingRule()
            {
                Id = 8,
                LeftRule = Rules[42]
            };

            Rules[11] = new NestingRule()
            {
                Id = 11,
                LeftRule = Rules[42],
                RightRule = Rules[31]
            };
        }
        #endregion Fix

        #region ParseIds
        private ConcatRule ParseIds(int parentId, string line)
        {
            var rule = new ConcatRule()
            {
                Id = parentId,
            };
            foreach (var token in line.Split(' '))
            {
                if (token.Length > 0)
                {
                    rule.SubRuleIds.Add(int.Parse(token));
                }
            }
            return rule;
        }
        #endregion ParseIds

        #region Reduce
        public void Reduce()
        {
            List<CharacterRule> NewRules = new List<CharacterRule>();
            do
            {
                NewRules.Clear();
                foreach (var rule in Rules.Values)
                {
                    var newRule = rule.Reduce(this);
                    if (newRule != null)
                    {
                        NewRules.Add(newRule);
                    }
                }
                foreach (var rule in NewRules)
                {
                    Rules[rule.Id] = rule;
                }
            } while (NewRules.Count > 0);
        }
        #endregion Reduce
    }

}
