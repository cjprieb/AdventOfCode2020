using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2020.Day02
{
    [TestClass]
    public class Day02
    {
        #region Input

        private static IEnumerable<PasswordRecord> GetInput()
        {
            List<PasswordRecord> records = new List<PasswordRecord>();
            foreach (var line in Properties.Resources.Day02Input.Split('\n'))
            {
                var tokens = line.Split(':');
                if (tokens.Length == 2)
                {
                    records.Add(new PasswordRecord()
                    {
                        Policy = tokens[0].Trim(),
                        Password = tokens[1].Trim()
                    });
                }
            }
            return records;
        }

        #endregion Input

        #region Code...

        static Regex _PolicyPattern = new Regex(@"(\d+)-(\d+) (\w)");

        private bool IsValidPassword(PasswordRecord record) => IsValidPassword(record.Policy, record.Password);

        private bool IsValidPassword(string policy, string password)
        {
            Match match = _PolicyPattern.Match(policy);
            if (!match.Success) throw new FormatException($"The policy string \"{policy}\" is not in the correct format.");

            var min = int.Parse(match.Groups[1].Value);
            var max = int.Parse(match.Groups[2].Value);
            var character = match.Groups[3].Value[0];

            int count = password.Count(c => c == character);
            return count >= min && count <= max;
        }
        private bool IsValidPasswordVersion2(PasswordRecord record) => IsValidPasswordVersion2(record.Policy, record.Password);

        private bool IsValidPasswordVersion2(string policy, string password)
        {
            Match match = _PolicyPattern.Match(policy);
            if (!match.Success) throw new FormatException($"The policy string \"{policy}\" is not in the correct format.");

            var position1 = int.Parse(match.Groups[1].Value) - 1;
            var position2 = int.Parse(match.Groups[2].Value) - 1;
            var character = match.Groups[3].Value[0];

            if (position2 >= password.Length) throw new FormatException($"Expected at least {position2} characters in \"{password}\"");

            bool atPosition1 = (password[position1] == character);
            bool atPosition2 = (password[position2] == character);
            return (atPosition1 || atPosition2) && (atPosition1 != atPosition2);
        }

        #endregion Code...

        #region Tests...
        [TestMethod]
        public void IsValid_a() => Assert.IsTrue(IsValidPassword("1-3 a", "abcde"));

        [TestMethod]
        public void IsNotValid_b() => Assert.IsFalse(IsValidPassword("1-3 b", "cdefg"));
        [TestMethod]
        public void IsValid_c() => Assert.IsTrue(IsValidPassword("2-9 c", "ccccccccc"));

        [TestMethod]
        public void IsNotValid_c() => Assert.IsFalse(IsValidPassword("2-4 c", "ccccccccc"));
        [TestMethod]
        public void IsValidVersion2_a() => Assert.IsTrue(IsValidPasswordVersion2("1-3 a", "abcde"));

        [TestMethod]
        public void IsNotValidVersion2_b() => Assert.IsFalse(IsValidPasswordVersion2("1-3 b", "cdefg"));
        [TestMethod]
        public void IsNotValidVersion2_c() => Assert.IsFalse(IsValidPasswordVersion2("2-9 c", "ccccccccc"));

        [TestMethod]
        public void IsValidVersion2_c1() => Assert.IsTrue(IsValidPasswordVersion2("2-4 c", "cccdccccc"));

        [TestMethod]
        public void IsValidVersion2_c2() => Assert.IsTrue(IsValidPasswordVersion2("2-4 c", "cdccccc"));

        [TestMethod]
        public void Answer1()
        {
            Assert.AreEqual(660, GetInput().Count(IsValidPassword));
        }

        [TestMethod]
        public void Answer2()
        {
            Assert.AreEqual(530, GetInput().Count(IsValidPasswordVersion2));
        }
        #endregion Tests...
    }

    class PasswordRecord
    {
        public string Policy { get; set; }
        public string Password { get; set; }
    }
}
