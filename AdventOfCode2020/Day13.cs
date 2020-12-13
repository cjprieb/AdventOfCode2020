using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace AdventOfCode2020.Day13
{
    [TestClass]
    public class Day13
    {
        #region Input

        public string[] Input => new string[]
        {
            "17","x","x","x","x","x","x","41","x","x","x","x","x","x","x",
            "x","x","983","x","29","x","x","x","x","x","x","x","x","x","x",
            "x","x","x","x","x","x","19","x","x","x","23","x","x","x",
            "x","x","x","x","397","x","x","x","x","x","37","x","x","x","x",
            "x","x","13"
        };

        public string[] TestInput => new string[]
        {
            "7","13","x","x","59","x","31","19"
        };

        public string[] TestInput2 => new string[]
        {
            "1789","37","47","1889"
        };

        #endregion Input

        #region Code...

        #region CalculateEarliestPossibleBus
        private (int busId, int minutesWaiting) CalculateEarliestPossibleBus(int[] validBuses, int earliestDepartureTime)
        {
            int chosenBus = 0;
            int? chosenDepatureTime = null;
            foreach (var bus in validBuses)
            {
                int priorDepartureTime = earliestDepartureTime / bus;
                if (priorDepartureTime == 0)
                {
                    chosenDepatureTime = earliestDepartureTime;
                    chosenBus = bus;
                }
                else
                {
                    int nextDepartureTime = (bus * priorDepartureTime) + bus;
                    if (!chosenDepatureTime.HasValue || chosenDepatureTime.Value > nextDepartureTime)
                    {
                        chosenDepatureTime = nextDepartureTime;
                        chosenBus = bus;
                    }
                }
            }
            return (chosenBus, (chosenDepatureTime.Value - earliestDepartureTime));
        }
        #endregion CalculateEarliestPossibleBus

        #region GetValidBuses
        public int[] GetValidBuses(IEnumerable<string> input)
        {
            return input.Where(x => x != "x").Select(x => int.Parse(x)).ToArray();
        }
        #endregion GetValidBuses

        #region ProductOfEarliestBus
        private int ProductOfEarliestBus(string[] testInput, int earliestDepartureTime)
        {
            var validBuses = GetValidBuses(testInput);
            (int busId, int minutesWaiting) = CalculateEarliestPossibleBus(validBuses, earliestDepartureTime);
            return busId * minutesWaiting;
        }
        #endregion ProductOfEarliestBus

        #region ContestResult
        /// <summary>
        /// Modified from 
        /// https://www.reddit.com/r/adventofcode/comments/kc4njx/2020_day_13_solutions/gfpog4g?utm_source=share&utm_medium=web2x&context=3
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private long ContestResult(string[] input)
        {
            var buses = GetBusSchedules(input);
            long time = buses[0].BusId;
            long step = buses[0].BusId;
            foreach (var bus in buses.Skip(1))
            {
                while ((time + bus.Index) % bus.BusId != 0)
                {
                    time += step;
                }
                step *= bus.BusId;
            }
            return time;
        }
        #endregion ContestResult

        #region GetBusSchedules
        public List<Bus> GetBusSchedules(string[] input)
        {
            var result = new List<Bus>();
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] != "x")
                {
                    result.Add(new Bus(int.Parse(input[i]), i));
                }
            }
            return result;
        }
        #endregion GetBusSchedules

        #endregion Code...

        #region Tests...
        [TestMethod] public void Test1() => Assert.AreEqual(295, ProductOfEarliestBus(TestInput, 939));
        [TestMethod] public void Answer1() => Assert.AreEqual(2382, ProductOfEarliestBus(Input, 1000434));

        [TestMethod] public void Test2a() => Assert.AreEqual(1068781, ContestResult(TestInput));
        [TestMethod] public void Test2b() => Assert.AreEqual(1202161486, ContestResult(TestInput2));

        /// <summary>
        /// too low: 1962415654
        /// </summary>
        [TestMethod] public void Answer2() => Assert.AreEqual(906332393333683, ContestResult(Input));
        #endregion Tests...
    }

    public class Bus 
    { 
        public int BusId { get; private set; }
        public int Index { get; private set; }

        public Bus(int busId, int index)
        {
            BusId = busId;
            Index = index;
        }
    }
}
