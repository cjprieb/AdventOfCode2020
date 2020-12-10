using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace AdventOfCode2020.Day10
{
    [TestClass]
    public class Day10
    {
        #region Input

        public int[] GetInput() => new int[]
        {
            115,
            134,
            121,
            184,
            78,
            84,
            77,
            159,
            133,
            90,
            71,
            185,
            152,
            165,
            39,
            64,
            85,
            50,
            20,
            75,
            2,
            120,
            137,
            164,
            101,
            56,
            153,
            63,
            70,
            10,
            72,
            37,
            86,
            27,
            166,
            186,
            154,
            131,
            1,
            122,
            95,
            14,
            119,
            3,
            99,
            172,
            111,
            142,
            26,
            82,
            8,
            31,
            53,
            28,
            139,
            110,
            138,
            175,
            108,
            145,
            58,
            76,
            7,
            23,
            83,
            49,
            132,
            57,
            40,
            48,
            102,
            11,
            105,
            146,
            149,
            66,
            38,
            155,
            109,
            128,
            181,
            43,
            44,
            94,
            4,
            169,
            89,
            96,
            60,
            69,
            9,
            163,
            116,
            45,
            59,
            15,
            178,
            34,
            114,
            17,
            16,
            79,
            91,
            100,
            162,
            125,
            156,
            65
        };

        public int[] GetTestInput1() => new int[] {16,10,15,5,1,11,7,19,6,12,4};

        public int[] GetTestInput2() => new int[] {
            28,
            33,
            18,
            42,
            31,
            14,
            46,
            20,
            48,
            47,
            24,
            23,
            49,
            45,
            19,
            38,
            39,
            11,
            1,
            32,
            25,
            35,
            8,
            17,
            7,
            9,
            4,
            2,
            34,
            10,
            3
            };

        #endregion Input

        #region Code...

        private Dictionary<int, HashSet<int>> BuildGraph(int[] values)
        {
            var graph = new Dictionary<int, HashSet<int>>();
            foreach (var value in values.Append(0))
            {
                if (!graph.ContainsKey(value))
                {
                    graph[value] = new HashSet<int>();
                }
                int min = value + 1;
                int max = value + 3;
                foreach (var otherValue in values)
                {                    
                    if (otherValue >= min && otherValue <= max)
                    {
                        graph[value].Add(otherValue);
                    }
                }
            }
            return graph;
        }

        private IEnumerable<int> FindOrder(Dictionary<int, HashSet<int>> graph)
        {
            int source = 0;
            var distances = new Dictionary<int, int>();
            distances[source] = 0;

            var previous = new Dictionary<int, int>();
            var next = new Dictionary<int, int>();

            var set = new HashSet<int>();
            foreach (var key in graph.Keys) 
            {
                set.Add(key);
            }

            while (set.Count > 0)
            {
                int? minDistance = null;
                int nextValue = 0;
                foreach (var kvp in distances.Where(kvp => set.Contains(kvp.Key)))
                {
                    if (!minDistance.HasValue || minDistance > kvp.Value)
                    {
                        nextValue = kvp.Key;
                        minDistance = kvp.Value;
                    }
                }

                set.Remove(nextValue);
                foreach (var neighbor in graph[nextValue])
                {
                    var alt = distances[nextValue] + (neighbor - nextValue);
                    if (!distances.ContainsKey(neighbor) || alt < distances[neighbor])
                    {
                        distances[neighbor] = alt;
                        previous[neighbor] = nextValue;
                        next[nextValue] = neighbor;
                    }
                }
            }

            var order = new List<int>();
            int? current = source;
            while (current.HasValue) 
            {
                order.Add(current.Value);
                if (next.ContainsKey(current.Value)) current = next[current.Value];
                else current = null;
            }

            return order;
        }

        private int GetProductOfJoltDifferences(int[] values)
        {
            //var graph = BuildGraph(values);
            //var order = FindOrder(graph);
            var order = values.Append(0).OrderBy(i => i).ToArray();

            int sumOfDiff1 = 0;
            int sumOfDiff2 = 0;
            int sumOfDiff3 = 1; // the built-in adapter in device is always going to have a diff of 3 from the last
            int previous = 0;
            foreach (var i in order)
            {
                int diff = i - previous;
                if (diff == 1) sumOfDiff1++;
                else if (diff == 2) sumOfDiff2++;
                else if (diff == 3) sumOfDiff3++;
                previous = i;
            }
            return sumOfDiff1 * sumOfDiff3;
        }

        private long GetCountOfDistinctArragements(int[] values)
        {
            //var graph = BuildGraph(values);
            var order = values.Append(0).OrderBy(i => i).ToArray();
            long totalPossibilities = 1;

            int previousIndex = 0;
            for (int i = 1; i < order.Length; i++)
            {
                int value = order[i];
                int diff = value - order[i-1];
                if (diff == 3)
                {
                    int itemsToTake = i - previousIndex;
                    long current = CountPossiblities(i - previousIndex);
                    totalPossibilities = totalPossibilities * current;
                    previousIndex = i;
                }
            }
            totalPossibilities = totalPossibilities * CountPossiblities(order.Length - previousIndex);

            return totalPossibilities;
        }

        private Dictionary<int, long> _PossiblitiesByLength = new Dictionary<int, long>();
        private long CountPossiblities(int itemsInSubList)
        {
            var count = 1L;
            if (_PossiblitiesByLength.ContainsKey(itemsInSubList))
            {
                count = _PossiblitiesByLength[itemsInSubList];
            }
            else
            {
                if (itemsInSubList > 2)
                {
                    count = CountPossiblities(itemsInSubList - 1) + CountPossiblities(itemsInSubList - 2);
                    if (itemsInSubList > 3)
                    {
                        count += CountPossiblities(itemsInSubList - 3);
                    }
                }
                _PossiblitiesByLength[itemsInSubList] = count;
            }
            return count;
        }

        #endregion Code...

        #region Tests...
        [TestMethod] public void Test1a() => Assert.AreEqual(35, GetProductOfJoltDifferences(GetTestInput1()));

        [TestMethod] public void Test1b() => Assert.AreEqual(220, GetProductOfJoltDifferences(GetTestInput2()));

        [TestMethod] public void Answer1() => Assert.AreEqual(2760, GetProductOfJoltDifferences(GetInput()));

        [TestMethod] public void CountPossiblitiesTest2() => Assert.AreEqual(1, CountPossiblities(2));
        [TestMethod] public void CountPossiblitiesTest3() => Assert.AreEqual(2, CountPossiblities(3));
        [TestMethod] public void CountPossiblitiesTest4() => Assert.AreEqual(4, CountPossiblities(4));
        [TestMethod] public void CountPossiblitiesTest5() => Assert.AreEqual(7, CountPossiblities(5));
        [TestMethod] public void CountPossiblitiesTest6() => Assert.AreEqual(13, CountPossiblities(6));

        [TestMethod] public void Test2a() => Assert.AreEqual(8, GetCountOfDistinctArragements(GetTestInput1()));

        [TestMethod] public void Test2b() => Assert.AreEqual(19208, GetCountOfDistinctArragements(GetTestInput2()));

        [TestMethod] public void Answer2() => Assert.AreEqual(13816758796288, GetCountOfDistinctArragements(GetInput()));
        #endregion Tests...
    }
}
