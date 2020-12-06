using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2020.Day06
{
    [TestClass]
    public class Day06
    {
        #region Input

        public string[] GetInput() => Properties.Resources.Day06.Split('\n');

        #endregion Input

        #region Code...

        private int SumOfAllAnswersPart1(string[] allResponses)
        {
            int sum = 0;
            HashSet<char> groupAnswers = new HashSet<char>();
            foreach (var answers in allResponses)
            {
                string trimmed = answers.Trim();
                if (trimmed.Length == 0)
                {
                    sum += groupAnswers.Count;
                    groupAnswers = new HashSet<char>();
                }
                else
                {
                    foreach (var c in trimmed)
                    {
                        groupAnswers.Add(c);
                    }
                }
            }
            sum += groupAnswers.Count;
            return sum;
        }

        private int SumOfAllAnswersPart2(string[] allResponses)
        {
            int sum = 0;
            int numberOfPeopleInGroup = 0;
            Dictionary<char, int> groupAnswers = new Dictionary<char, int>();
            foreach (var answers in allResponses)
            {
                string trimmed = answers.Trim();
                if (trimmed.Length == 0)
                {
                    sum += CountValidAnswers(groupAnswers, numberOfPeopleInGroup);
                    numberOfPeopleInGroup = 0;
                    groupAnswers = new Dictionary<char, int>();
                }
                else
                {
                    numberOfPeopleInGroup++;
                    foreach (var c in trimmed)
                    {
                        if (!groupAnswers.ContainsKey(c)) groupAnswers[c] = 0;
                        groupAnswers[c] += 1;
                    }
                }
            }
            sum += CountValidAnswers(groupAnswers, numberOfPeopleInGroup);
            return sum;
        }

        private int CountValidAnswers(Dictionary<char, int> groupAnswers, int numberOfPeopleInGroup)
        {
            return groupAnswers.Where(kvp => kvp.Value == numberOfPeopleInGroup).Count();
        }

        #endregion Code...

        #region Tests...
        [TestMethod] public void Answer1() => Assert.AreEqual(6549, SumOfAllAnswersPart1(GetInput()));

        [TestMethod] public void Answer2() => Assert.AreEqual(3466, SumOfAllAnswersPart2(GetInput()));
        #endregion Tests...
    }
}
