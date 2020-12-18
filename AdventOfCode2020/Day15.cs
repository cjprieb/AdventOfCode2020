using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace AdventOfCode2020.Day15
{
    [TestClass]
    public class Day15
    {
        #region Input

        public int[] GetInput() => new int[] { 2, 20, 0, 4, 1, 17 };

        #endregion Input

        #region Code...

        #region Map
        private int GetNthSpokenNumber(int[] input, int n)
        {
            Dictionary<int, int> lastTurnNumberWasMentioned = new Dictionary<int, int>();

            for (int i = 0; i < input.Length; i++)
            {
                lastTurnNumberWasMentioned[input[i]] = i+1;
            }

            int lastNumberSpoken = 0;
            int lastTimePreviousNumberWasSpoken = 0;

            for (int turn = input.Length+1; turn <= n; turn++)
            {
                var numberToSpeak = lastTimePreviousNumberWasSpoken == 0 ? 0 : turn - lastTimePreviousNumberWasSpoken - 1;
                if (lastTurnNumberWasMentioned.ContainsKey(numberToSpeak))
                {
                    lastTimePreviousNumberWasSpoken = lastTurnNumberWasMentioned[numberToSpeak];
                }
                else
                {
                    lastTimePreviousNumberWasSpoken = 0;
                }
                lastTurnNumberWasMentioned[numberToSpeak] = turn;
                lastNumberSpoken = numberToSpeak;
            }
            return lastNumberSpoken;
        }
        #endregion Map

        #endregion Code...

        #region Tests...
        [TestMethod] public void Test1_0_3_6() => Assert.AreEqual(4, GetNthSpokenNumber(new int[] { 0, 3, 6 }, 9));
        [TestMethod] public void Test1_1_3_2() => Assert.AreEqual(1, GetNthSpokenNumber(new int[] { 1, 3, 2 }, 2020));
        [TestMethod] public void Test1_2_1_3() => Assert.AreEqual(10, GetNthSpokenNumber(new int[] { 2, 1, 3 }, 2020));
        [TestMethod] public void Test1_1_2_3() => Assert.AreEqual(27, GetNthSpokenNumber(new int[] { 1, 2, 3 }, 2020));
        [TestMethod] public void Test1_2_3_1() => Assert.AreEqual(78, GetNthSpokenNumber(new int[] { 2, 3, 1 }, 2020));
        [TestMethod] public void Test1_3_2_1() => Assert.AreEqual(438, GetNthSpokenNumber(new int[] { 3, 2, 1 }, 2020));
        [TestMethod] public void Test1_3_1_2() => Assert.AreEqual(1836, GetNthSpokenNumber(new int[] { 3, 1, 2 }, 2020));

        [TestMethod] public void Answer1() => Assert.AreEqual(758, GetNthSpokenNumber(GetInput(), 2020));

        [TestMethod] public void Test2_0_3_6() => Assert.AreEqual(175594, GetNthSpokenNumber(new int[] { 0, 3, 6 }, 30000000));
        [TestMethod] public void Test2_1_3_2() => Assert.AreEqual(2578, GetNthSpokenNumber(new int[] { 1, 3, 2 }, 30000000));
        [TestMethod] public void Test2_2_1_3() => Assert.AreEqual(3544142, GetNthSpokenNumber(new int[] { 2, 1, 3 }, 30000000));
        [TestMethod] public void Test2_1_2_3() => Assert.AreEqual(261214, GetNthSpokenNumber(new int[] { 1, 2, 3 }, 30000000));
        [TestMethod] public void Test2_2_3_1() => Assert.AreEqual(6895259, GetNthSpokenNumber(new int[] { 2, 3, 1 }, 30000000));
        [TestMethod] public void Test2_3_2_1() => Assert.AreEqual(18, GetNthSpokenNumber(new int[] { 3, 2, 1 }, 30000000));
        [TestMethod] public void Test2_3_1_2() => Assert.AreEqual(362, GetNthSpokenNumber(new int[] { 3, 1, 2 }, 30000000));
        [TestMethod] public void Answer2() => Assert.AreEqual(814, GetNthSpokenNumber(GetInput(), 30000000));

        #endregion Tests...
    }
}
