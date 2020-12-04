using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace AdventOfCode2020.Day03
{
    [TestClass]
    public class Day03
    {
        #region Input

        static TreeMap GetInput()
        {
            var lines = Properties.Resources.Day03Input.Split('\n');
            return new TreeMap(lines);
        }

        static Slope[] GetSlopesToCheck()
        {
            return new Slope[]
            {
                new Slope(1, 1),
                new Slope(3, 1),
                new Slope(5, 1),
                new Slope(7, 1),
                new Slope(1, 2)
            };
        }

        static TreeMap GetTestInput()
        {
            var lines = new string[]
            {
                "..##.......",
                "#...#...#..",
                ".#....#..#.",
                "..#.#...#.#",
                ".#...##..#.",
                "..#.##.....",
                ".#.#.#....#",
                ".#........#",
                "#.##...#...",
                "#...##....#",
                ".#..#...#.#"
            };
            return new TreeMap(lines);
        }

        #endregion Input

        #region Code...

        private static long GetProductOfTreesEncountered(TreeMap map, Slope[] slopes)
        {
            var treesEncountered = slopes.Select(slope =>
            {
                map.Slope = slope;
                long count = map.CountTreesEncountered();
                return count;
            });
            return treesEncountered.Aggregate(1L, (product, i) => product * i);
        }

        #endregion Code...

        #region Tests...

        [TestMethod] public void Test01() => Assert.AreEqual(7, GetTestInput().CountTreesEncountered());

        [TestMethod]
        public void Answer1() => Assert.AreEqual(252, GetInput().CountTreesEncountered());

        [TestMethod]
        public void Test02()
        {
            var map = GetTestInput();
            var slopes = GetSlopesToCheck();
            
            Assert.AreEqual(336, GetProductOfTreesEncountered(map, slopes));
        }

        [TestMethod]
        public void Answer2()
        {
            var map = GetInput();
            var slopes = GetSlopesToCheck();

            Assert.AreEqual(2608962048, GetProductOfTreesEncountered(map, slopes));
        }
        #endregion Tests...
    }

    class Slope
    {
        public int XOffset { get; set; }
        public int YOffset { get; set; }
        public (int, int) NextCoordinate(int x, int y) => (x + XOffset, y + YOffset);
        public Slope(int x, int y)
        {
            XOffset = x;
            YOffset = y;
        }
    }

    class TreeMap
    {
        private string[] _Grid;

        public int Height => _Grid.Length;

        public Slope Slope { get; set; } = new Slope(3, 1);

        public TreeMap(string[] lines)
        {
            _Grid = lines;
            for (int i = 0; i < lines.Length; i++)
            {
                _Grid[i] = _Grid[i].Trim();
            }
        }

        public bool HasTree(int x, int y)
        {
            if (y >= _Grid.Length) throw new Exception($"row index {y} exceeds the height of the grid {_Grid.Length}");
            string line = _Grid[y];
            x = x % line.Length;
            return line[x] == '#';
        }

        public int CountTreesEncountered()
        {
            int x = 0, y = 0;
            int count = 0;
            while (y < Height)
            {
                if (HasTree(x, y)) count++;
                (x, y) = Slope.NextCoordinate(x, y);
            }
            return count;
        }
    }
}
