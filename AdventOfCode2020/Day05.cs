using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;

namespace AdventOfCode2020.Day05
{
    [TestClass]
    public class Day05
    {
        #region Input

        private string[] GetInput()
        {
            return Properties.Resources.Day05Input.Split('\n');
        }

        #endregion Input

        #region Code...

        private int CalculateMySeat()
        {
            var seatMap = new bool[128 * 8];
            foreach (var code in GetInput())
            {
                CalculateSeatId(code, seatMap);
            }
            int mySeat = 0;
            int max = seatMap.Length - 1;
            bool prevSeat = false, currSeat = false, nextSeat = false;
            for (int i = 1; i < max; i++)
            {
                prevSeat = seatMap[i - 1];
                currSeat = seatMap[i];
                nextSeat = seatMap[i + 1];
                if (prevSeat && !currSeat && nextSeat)
                {
                    mySeat = i;
                    break;
                }
            }

            return mySeat;
        }

        private int CalculateSeatId(string code, bool[] seatMap = null)
        {
            code = code.Trim();
            if (code.Length != 10) throw new Exception($"Expected code length of 10, but code was \"{code}\"");

            int row = ParseRow(code);
            int column = ParseColumn(code);
            int seatId = row * 8 + column;
            if (seatMap != null)
            {
                seatMap[seatId] = true;
            }
            return seatId;
        }

        private (int, int) SelectRange(char c, int min, int max)
        {
            int midPoint = min + ((max - min) / 2);
            if ((c == 'B') || (c == 'R')) // take upper half
            {
                min = midPoint + 1;
            }
            else
            {
                max = midPoint;
            }
            return (min, max);
        }

        private int ParseResult(IEnumerable<char> code, int max)
        {
            int? result = null;
            int min = 0;

            foreach (var c in code)
            {
                (min, max) = SelectRange(c, min, max);
                if (min == max) result = min;
            }
            return result ?? -1;
        }

        private int ParseColumn(string code) => ParseResult(code.Skip(7), 7);

        private int ParseRow(string code) => ParseResult(code.Take(7), 127);

        #endregion Code...

        #region Tests...

        [TestMethod] public void ParseRow_44() => Assert.AreEqual(44, ParseRow("FBFBBFFRLR"));
        [TestMethod] public void ParseRow_70() => Assert.AreEqual(70, ParseRow("BFFFBBFRRR"));
        [TestMethod] public void ParseRow_14() => Assert.AreEqual(14, ParseRow("FFFBBBFRRR"));
        [TestMethod] public void ParseRow_102() => Assert.AreEqual(102, ParseRow("BBFFBBFRLL"));

        [TestMethod] public void ParseColumn_5() => Assert.AreEqual(5, ParseColumn("FBFBBFFRLR"));
        [TestMethod] public void ParseColumn_7a() => Assert.AreEqual(7, ParseColumn("BFFFBBFRRR"));
        [TestMethod] public void ParseColumn_7b() => Assert.AreEqual(7, ParseColumn("FFFBBBFRRR"));
        [TestMethod] public void ParseColumn_4() => Assert.AreEqual(4, ParseColumn("BBFFBBFRLL"));

        [TestMethod] public void CalculateSeatId_357() => Assert.AreEqual(357, CalculateSeatId("FBFBBFFRLR"));
        [TestMethod] public void CalculateSeatId_567() => Assert.AreEqual(567, CalculateSeatId("BFFFBBFRRR"));
        [TestMethod] public void CalculateSeatId_119() => Assert.AreEqual(119, CalculateSeatId("FFFBBBFRRR"));
        [TestMethod] public void CalculateSeatId_820() => Assert.AreEqual(820, CalculateSeatId("BBFFBBFRLL"));

        [TestMethod]
        public void Answer1() => Assert.AreEqual(835, GetInput().Max(code => CalculateSeatId(code)));

        [TestMethod]
        public void Answer2() => Assert.AreEqual(649, CalculateMySeat());

        #endregion Tests...
    }
}
