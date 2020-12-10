using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AdventOfCode2020.Day09
{
    [TestClass]
    public class Day09
    {
        #region Input

        static IEnumerable<long> GetInput() => Input.GetLines(Properties.Resources.Day09Input).Select(line => long.Parse(line));

        static IEnumerable<long> GetTestInput() => new long[]
        {
            35L,
            20L,
            15L,
            25L,
            47L,
            40L,
            62L,
            55L,
            65L,
            95L,
            102L,
            117L,
            150L,
            182L,
            127L,
            219L,
            299L,
            277L,
            309L,
            576L,
        };

        #endregion Input

        #region Code...

        bool IsValid(long[] values, int currIndex, int preambleSize)
        {
            if (currIndex < preambleSize) return true;
            bool isValid = false;

            long currValue = values[currIndex];
            int min = currIndex - preambleSize;
            for (int i = min; i < currIndex; i++)
            {
                if (values[i] >= currValue) continue;
                for (int j = i+1; j < currIndex; j++)
                {
                    if (values[j] >= currValue) continue;
                    if (values[i] + values[j] == currValue)
                    {
                        isValid = true;
                        break;
                    }
                }

                if (isValid) break;
            }

            return isValid;
        }

        private long FindFirstInvalidNumber(IEnumerable<long> input, int preambleSize)
        {
            var values = input.ToArray();
            long? invalidValue = null;

            int min = preambleSize;
            for (int i = min; i < values.Length; i++)
            {
                if (!IsValid(values, i, preambleSize))
                {
                    invalidValue = values[i];
                    break;
                }
            }

            return invalidValue ?? 0;
        }

        private long FindEncryptionWeakness(IEnumerable<long> input, long targetValue)
        {
            var values = input.ToArray();

            int targetIndex = 0;
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] == targetValue)
                {
                    targetIndex = i;
                    break;
                }
            }

            int min = 0; int max = 0;
            int setSize = 3;
            bool found = false;
            while (!found && setSize < targetIndex)
            {
                int length = targetIndex - setSize;
                for (int i = 0; i < length; i++)
                {
                    var sum = values.Skip(i).Take(setSize).Sum();
                    if (sum == targetValue)
                    {
                        min = i;
                        max = i + setSize;
                        break;
                    }
                    else if (sum > targetValue) break;
                }
                setSize++;
            }

            long valueMin = targetValue, valueMax = 0;
            for (int i = min; i < max; i++)
            {
                if (values[i] < valueMin) valueMin = values[i];
                if (values[i] > valueMax) valueMax = values[i];
            }
            return valueMin + valueMax;
        }

        #endregion Code...

        #region Tests...
        [TestMethod] public void Test1() => Assert.AreEqual(127L, FindFirstInvalidNumber(GetTestInput(), 5));

        [TestMethod] public void Answer1() => Assert.AreEqual(257342611L, FindFirstInvalidNumber(GetInput(), 25));

        [TestMethod] public void Test2() => Assert.AreEqual(62L, FindEncryptionWeakness(GetTestInput(), 127L));

        [TestMethod] public void Answer2() => Assert.AreEqual(35602097L, FindEncryptionWeakness(GetInput(), 257342611L));
        #endregion Tests...
    }
}
