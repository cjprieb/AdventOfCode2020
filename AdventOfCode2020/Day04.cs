using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2020.Day04
{
    [TestClass]
    public class Day04
    {
        #region Input

        public static string[] GetInputLines() => Properties.Resources.Day04Input.Split('\n');

        public static string[] GetTestInputLines() => new string[]
        {
            "ecl:gry pid:860033327 eyr:2020 hcl:#fffffd",
            "byr:1937 iyr:2017 cid:147 hgt:183cm",
            "",
            "iyr:2013 ecl:amb cid:350 eyr:2023 pid:028048884",
            "hcl:#cfa07d byr:1929",
            "",
            "hcl:#ae17e1 iyr:2013",
            "eyr:2024",
            "ecl:brn pid:760753108 byr:1931",
            "hgt:179cm",
            "",
            "hcl:#cfa07d eyr:2025 pid:166559648",
            "iyr:2011 ecl:brn hgt:59in"
        };

        public static string[] GetTestInputLines2() => new string[]
        {
            "eyr:1972 cid:100",
            "hcl:#18171d ecl:amb hgt:170 pid:186cm iyr:2018 byr:1926",
            "",
            "iyr:2019",
            "hcl:#602927 eyr:1967 hgt:170cm",
            "ecl:grn pid:012533040 byr:1946",
            "",
            "hcl:dab227 iyr:2012",
            "ecl:brn hgt:182cm pid:021572410 eyr:2020 byr:1992 cid:277",
            "",
            "hgt:59cm ecl:zzz",
            "eyr:2038 hcl:74454a iyr:2023",
            "pid:3556412378 byr:2007",
            "",
            "pid:087499704 hgt:74in ecl:grn iyr:2012 eyr:2030 byr:1980",
            "hcl:#623a2f",
            "",
            "eyr:2029 ecl:blu cid:129 byr:1989",
            "iyr:2014 pid:896056539 hcl:#a97842 hgt:165cm",
            "",
            "hcl:#888785",
            "hgt:164cm byr:2001 iyr:2015 cid:88",
            "pid:545766238 ecl:hzl",
            "eyr:2022",
            "",
            "iyr:2010 hgt:158cm hcl:#b6652a ecl:blu byr:1944 eyr:2021 pid:093154719",
        };

        #endregion Input

        #region Code...

        public int CountValidPassports(string[] input, bool useExtendedValidation = false)
        {
            var passports = Passport.ParseLines(input);
            return passports.Count(passport => passport.IsValid(useExtendedValidation));
        }

        #endregion Code...

        #region Tests...

        [TestMethod] public void Test1() => Assert.AreEqual(2, CountValidPassports(GetTestInputLines()));

        [TestMethod] public void Answer1() => Assert.AreEqual(219, CountValidPassports(GetInputLines()));

        [TestMethod] public void IsValidBirthYear_2002() => Assert.IsTrue(Passport.IsValidYear("2002", 1920, 2002));
        [TestMethod] public void IsValidBirthYear_2003() => Assert.IsFalse(Passport.IsValidYear("2003", 1920, 2002));

        [TestMethod] public void IsValidHeight_60in() => Assert.IsTrue(Passport.IsValidHeight("60in"));
        [TestMethod] public void IsValidHeight_190cm() => Assert.IsTrue(Passport.IsValidHeight("190cm"));
        [TestMethod] public void IsValidHeight_190in() => Assert.IsFalse(Passport.IsValidHeight("190in"));
        [TestMethod] public void IsValidHeight_190() => Assert.IsFalse(Passport.IsValidHeight("190"));

        [TestMethod] public void IsValidHairColor_n123abc() => Assert.IsTrue(Passport.IsValidHairColor("#123abc"));
        [TestMethod] public void IsValidHairColor_n123abz() => Assert.IsFalse(Passport.IsValidHairColor("#123abz"));
        [TestMethod] public void IsValidHairColor_123abc() => Assert.IsFalse(Passport.IsValidHairColor("123abc"));

        [TestMethod] public void IsValidEyeColor_brn() => Assert.IsTrue(Passport.IsValidEyeColor("brn"));
        [TestMethod] public void IsValidEyeColor_wat() => Assert.IsFalse(Passport.IsValidEyeColor("wat"));

        [TestMethod] public void IsValidPassportId_000000001() => Assert.IsTrue(Passport.IsValidPassportId("000000001"));
        [TestMethod] public void IsValidPassportId_0123456789() => Assert.IsFalse(Passport.IsValidPassportId("0123456789"));
        [TestMethod] public void IsValidPassportId_56789() => Assert.IsFalse(Passport.IsValidPassportId("56789"));

        [TestMethod] public void Test2() => Assert.AreEqual(4, CountValidPassports(GetTestInputLines2(), true));
        [TestMethod] public void Answer2() => Assert.AreEqual(127, CountValidPassports(GetInputLines(), true));
        #endregion Tests...
    }

    class Passport
    {
        private static readonly string[] _RequiredFields = new string[]
        {
            "byr","iyr","eyr","hgt","hcl","ecl","pid" //,"cid"
        };

        private static Regex _HeightPattern = new Regex(@"^(\d+)(in|cm)$");

        private static Regex _HairColorPattern = new Regex(@"^\#[0-9a-f]{6}$");

        private static string[] _ValidEyeColors = new string[]
        {
            "amb","blu","brn","gry","grn","hzl","oth"
        };

        private static Regex _PassportIdPattern = new Regex(@"^[0-9]{9}$");

        private Dictionary<string, string> _FieldValues = new Dictionary<string, string>();

        private string ToDebugString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var fld in _RequiredFields)
            {
                builder.AppendLine($"{fld} = {_FieldValues[fld]}");
            }
            foreach (var kvp in _FieldValues)
            {
                if (!_RequiredFields.Contains(kvp.Key) && kvp.Key != "cid")
                {
                    builder.AppendLine($"OTHER  {kvp.Key} = {kvp.Value}");
                }
            }
            return builder.ToString();
        }

        public static List<Passport> ParseLines(string [] lines)
        {
            List<Passport> result = new List<Passport>();
            Passport currentPassport = null;
            foreach (var line in lines)
            {
                string trimmed = line.Trim();
                if (trimmed.Length == 0)
                {
                    currentPassport = null;
                }
                else
                {
                    if (currentPassport == null)
                    {
                        currentPassport = new Passport();
                        result.Add(currentPassport);
                    }
                    currentPassport.AddDataFromLine(trimmed);
                }
            }
            return result;
        }

        public void AddDataFromLine(string line)
        {
            foreach (var field in line.Split(' '))
            {
                string[] tokens = field.Split(':');
                if (tokens.Length == 2)
                {
                    _FieldValues.Add(tokens[0], tokens[1]);
                }
            }
        }

        public bool IsValid(bool useExtendedValidation)
        {
            bool hasAllRequiredFields = _RequiredFields.All(field => _FieldValues.ContainsKey(field));
            if (!hasAllRequiredFields) return false;

            if (_FieldValues.Count == 8 && !_FieldValues.ContainsKey("cid")) return false;


            bool isValid = true;

            if (useExtendedValidation)
            {
                if (isValid && !IsValidYear(_FieldValues["byr"], 1920, 2002)) isValid = false;
                if (isValid && !IsValidYear(_FieldValues["iyr"], 2010, 2020)) isValid = false;
                if (isValid && !IsValidYear(_FieldValues["eyr"], 2020, 2030)) isValid = false;

                if (isValid && !IsValidHeight(_FieldValues["hgt"])) isValid = false;
                if (isValid && !IsValidHairColor(_FieldValues["hcl"])) isValid = false;
                if (isValid && !IsValidEyeColor(_FieldValues["ecl"])) isValid = false;
                if (isValid && !IsValidPassportId(_FieldValues["pid"])) isValid = false;

                //Console.WriteLine(ToDebugString());
            }

            return isValid;
        }

        public static bool IsValidHeight(string value)
        {
            bool isValid = false;
            Match match = _HeightPattern.Match(value);
            if (match.Success)
            {
                var height = int.Parse(match.Groups[1].Value);
                var unit = match.Groups[2].Value;
                if (unit == "cm")
                {
                    isValid = (height >= 150) && (height <= 193);
                }
                else if (unit == "in")
                {
                    isValid = (height >= 59) && (height <= 76);
                }
            }
            return isValid;
        }

        public static bool IsValidHairColor(string value) => _HairColorPattern.IsMatch(value);

        public static bool IsValidEyeColor(string value) => _ValidEyeColors.Contains(value);

        public static bool IsValidPassportId(string value) => _PassportIdPattern.IsMatch(value);

        public static bool IsValidYear(string value, int min, int max)
        {
            bool isValid = false;
            if (int.TryParse(value, out int year))
            {
                isValid = year >= min && year <= max;
            }
            return isValid;
        }
    }
}
