using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace AdventOfCode2020.Day17
{
    [TestClass]
    public class Day17
    {
        #region Input

        public string[] GetInput() => new string[]
        {
            ".##.####",
            ".#.....#",
            "#.###.##",
            "#####.##",
            "#...##.#",
            "#######.",
            "##.#####",
            ".##...#."
        };

        public string[] GetTestInput() => new string[]
        {
            ".#.",
            "..#",
            "###"
        };

        #endregion Input

        #region Code...

        #region CountActiveCubes
        private int CountActiveCubes(string[] input, int cycles)
        {
            var cube = new ConwayCube(input);
            for (int i = 0; i < cycles; i++)
            {
                cube.RunCycle();
            }
            return cube.ActivePointsCount;
        }
        #endregion CountActiveCubes

        #region CountActiveHypercubes
        private int CountActiveHypercubes(string[] input, int cycles)
        {
            var hypercube = new ConwayHypercube(input);
            for (int i = 0; i < cycles; i++)
            {
                hypercube.RunCycle();
            }
            return hypercube.ActivePointsCount;
        }
        #endregion CountActiveHypercubes

        #endregion Code...

        #region Tests...

        [TestMethod] public void Test1() => Assert.AreEqual(112, CountActiveCubes(GetTestInput(), 6));

        [TestMethod] public void Answer1() => Assert.AreEqual(372, CountActiveCubes(GetInput(), 6));

        [TestMethod] public void Test2() => Assert.AreEqual(848, CountActiveHypercubes(GetTestInput(), 6));

        [TestMethod] public void Answer2() => Assert.AreEqual(1896, CountActiveHypercubes(GetInput(), 6));
        #endregion Tests...
    }

    public struct Point3d
    {
        public int X;
        public int Y;
        public int Z;
        public Point3d(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public override int GetHashCode()
        {
            return X * 13 + Z * 11 + Y * 3;
        }
        public override bool Equals(object obj)
        {
            if (obj is Point3d point)
            {
                return X == point.X && Y == point.Y && Z == point.Z;
            }
            else return false;
        }
        public override string ToString()
        {
            return $"({X},{Y},{Z})";
        }
    }

    public class ConwayCube
    {
        private HashSet<Point3d> ActivePoints = new HashSet<Point3d>();
        private int _StartingSize = 0;
        private int _Cycle = 0;

        public int ActivePointsCount => ActivePoints.Count;

        public ConwayCube(string[] values)
        {
            _StartingSize = values.Length;
            for (int y = 0; y < values.Length; y++)
            {
                var line = values[y];
                for (int x = 0; x < line.Length; x++)
                {
                    if (line[x] == '#') ActivePoints.Add(new Point3d(x, y, 0));
                }
            }
        }

        public void RunCycle()
        {
            HashSet<Point3d> newActivePoints = new HashSet<Point3d>();
            _Cycle++;

            foreach (var x in GetRange(-_Cycle, _StartingSize + _Cycle))
            {
                foreach (var y in GetRange(-_Cycle, _StartingSize + _Cycle))
                {
                    foreach (var z in GetRange(-_Cycle, _Cycle))
                    {
                        var point = new Point3d(x, y, z);
                        var surroundingActivePoints = GetSurroundingPoints(point).Count(p => ActivePoints.Contains(p));
                        if (ActivePoints.Contains(point))
                        {
                            if (surroundingActivePoints == 2 || surroundingActivePoints == 3)
                            {
                                newActivePoints.Add(point);
                            }
                        }
                        else if (surroundingActivePoints == 3)
                        {
                            newActivePoints.Add(point);
                        }
                    }
                }
            }

            ActivePoints = newActivePoints;
        }

        private static IEnumerable<Point3d> GetSurroundingPoints(Point3d center)
        {
            foreach (var x in GetRange(center.X-1, center.X+1))
            {
                foreach (var y in GetRange(center.Y - 1, center.Y + 1))
                {
                    foreach (var z in GetRange(center.Z - 1, center.Z + 1))
                    {
                        if (center.X == x && center.Y == y && center.Z == z) continue;
                        yield return new Point3d(x, y, z);
                    }
                }
            }
        }

        private static IEnumerable<int> GetRange(int min, int max)
        {
            for (int i = min; i <= max; i++)
            {
                yield return i;
            }
        }
    }

    public struct Point4d
    {
        public int X;
        public int Y;
        public int Z;
        public int W;
        public Point4d(int x, int y, int z, int w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        } 
        public override int GetHashCode()
        {
            return X * 13 + Z * 11 + Y * 3 + W * 5;
        }
        public override bool Equals(object obj)
        {
            if (obj is Point4d point)
            {
                return X == point.X && Y == point.Y && Z == point.Z && W == point.W;
            }
            else return false;
        }
        public override string ToString()
        {
            return $"({X},{Y},{Z},{W})";
        }
    }

    public class ConwayHypercube
    {
        #region Member Variables...
        private HashSet<Point4d> ActivePoints = new HashSet<Point4d>();
        private int _StartingSize = 0;
        private int _Cycle = 0;
        #endregion Member Variables...

        #region Properties...
        public int ActivePointsCount => ActivePoints.Count;
        #endregion Properties...

        #region Constructors...

        #region ConwayHypercube
        public ConwayHypercube(string[] values)
        {
            _StartingSize = values.Length;
            for (int y = 0; y < values.Length; y++)
            {
                var line = values[y];
                for (int x = 0; x < line.Length; x++)
                {
                    if (line[x] == '#') ActivePoints.Add(new Point4d(x, y, 0, 0));
                }
            }
        }
        #endregion ConwayHypercube 

        #endregion Constructors...

        #region RunCycle
        public void RunCycle()
        {
            HashSet<Point4d> newActivePoints = new HashSet<Point4d>();
            _Cycle++;

            foreach (var x in GetRange(-_Cycle, _StartingSize + _Cycle))
            {
                foreach (var y in GetRange(-_Cycle, _StartingSize + _Cycle))
                {
                    foreach (var z in GetRange(-_Cycle, _Cycle))
                    {
                        foreach (var w in GetRange(-_Cycle, _Cycle))
                        {
                            var point = new Point4d(x, y, z, w);
                            var surroundingActivePoints = GetSurroundingPoints(point).Count(p => ActivePoints.Contains(p));
                            if (ActivePoints.Contains(point))
                            {
                                if (surroundingActivePoints == 2 || surroundingActivePoints == 3)
                                {
                                    newActivePoints.Add(point);
                                }
                            }
                            else if (surroundingActivePoints == 3)
                            {
                                newActivePoints.Add(point);
                            }
                        }
                    }
                }
            }

            ActivePoints = newActivePoints;
        }
        #endregion RunCycle

        #region GetSurroundingPoints
        private static IEnumerable<Point4d> GetSurroundingPoints(Point4d center)
        {
            foreach (var x in GetRange(center.X - 1, center.X + 1))
            {
                foreach (var y in GetRange(center.Y - 1, center.Y + 1))
                {
                    foreach (var z in GetRange(center.Z - 1, center.Z + 1))
                    {
                        foreach (var w in GetRange(center.W - 1, center.W + 1))
                        {
                            if (center.X == x && center.Y == y && center.Z == z && center.W == w) continue;
                            yield return new Point4d(x, y, z, w);
                        }
                    }
                }
            }
        }
        #endregion GetSurroundingPoints

        #region GetRange
        private static IEnumerable<int> GetRange(int min, int max)
        {
            for (int i = min; i <= max; i++)
            {
                yield return i;
            }
        }
        #endregion GetRange
    }
}
