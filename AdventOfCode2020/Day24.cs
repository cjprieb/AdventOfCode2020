using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Drawing;

namespace AdventOfCode2020.Day24
{
    [TestClass]
    public class Day24
    {
        #region Input

        public IEnumerable<string> GetInput() => Input.GetLines(Properties.Resources.Day24Input);

        public string[] GetTestInput() => new string[] {
            "sesenwnenenewseeswwswswwnenewsewsw",
            "neeenesenwnwwswnenewnwwsewnenwseswesw",
            "seswneswswsenwwnwse",
            "nwnwneseeswswnenewneswwnewseswneseene",
            "swweswneswnenwsewnwneneseenw",
            "eesenwseswswnenwswnwnwsewwnwsene",
            "sewnenenenesenwsewnenwwwse",
            "wenwwweseeeweswwwnwwe",
            "wsweesenenewnwwnwsenewsenwwsesesenwne",
            "neeswseenwwswnwswswnw",
            "nenwswwsewswnenenewsenwsenwnesesenew",
            "enewnwewneswsewnwswenweswnenwsenwsw",
            "sweneswneswneneenwnewenewwneswswnese",
            "swwesenesewenwneswnwwneseswwne",
            "enesenwswwswneneswsenwnewswseenwsese",
            "wnwnesenesenenwwnenwsewesewsesesew",
            "nenewswnwewswnenesenwnesewesw",
            "eneswnwswnwsenenwnwnwwseeswneewsenese",
            "neswnwewnwnwseenwseesewsenwsweewe",
            "wseweeenwnesenwwwswnew"
        };

        #endregion Input

        #region Code...

        #region CountBlackTiles
        public int CountBlackTiles(IEnumerable<string> input)
        {
            var floor = new Floor();
            floor.InitializeFloor(input);
            return floor.BlackTiles.Count;
        }
        #endregion CountBlackTiles

        #region CountBlackTilesAfterDays
        public int CountBlackTilesAfterDays(IEnumerable<string> input, int days)
        {
            var floor = new Floor();
            floor.InitializeFloor(input);
            for (int i = 0; i < days; i++)
            {
                floor.UpdateFloorForTheDay();
            }
            return floor.BlackTiles.Count;
        }
        #endregion CountBlackTilesAfterDays

        #endregion Code...

        #region Tests...
        [TestMethod] public void Test1() => Assert.AreEqual(10, CountBlackTiles(GetTestInput()));
        [TestMethod] public void Answer1() => Assert.AreEqual(479, CountBlackTiles(GetInput()));

        [TestMethod] public void Test2_1_Day() => Assert.AreEqual(15, CountBlackTilesAfterDays(GetTestInput(), 1));
        [TestMethod] public void Test2_10_Days() => Assert.AreEqual(37, CountBlackTilesAfterDays(GetTestInput(), 10));
        [TestMethod] public void Test2_100_Days() => Assert.AreEqual(2208, CountBlackTilesAfterDays(GetTestInput(), 100));
        [TestMethod] public void Answer2() => Assert.AreEqual(4135, CountBlackTilesAfterDays(GetInput(), 100));
        #endregion Tests...
    }

    public class Floor
    {
        #region BlackTiles
        public HashSet<Point> BlackTiles { get; private set; } = new HashSet<Point>();
        #endregion BlackTiles

        #region FlipTileAt
        public void FlipTileAt(string line)
        {
            Point centerPoint = new Point(0, 0);
            Point destination = centerPoint;
            char previous = (char)0;
            foreach (var current in line)
            {
                if (current != 'n' && current != 's')
                {
                    destination = GetNeighborTileLocation(destination, previous, current);
                }
                previous = current;
            }
            FlipTileAt(destination);
        }
        #endregion FlipTileAt

        #region FlipTileAt
        private void FlipTileAt(Point destination)
        {
            if (BlackTiles.Contains(destination))
            {
                BlackTiles.Remove(destination);
            }
            else
            {
                BlackTiles.Add(destination);
            }
        }
        #endregion FlipTileAt

        #region GetNeighbors
        public IEnumerable<Point> GetNeighbors(Point center)
        {
            yield return new Point(center.X - 2, center.Y);
            yield return new Point(center.X + 2, center.Y);
            yield return new Point(center.X - 1, center.Y - 1);
            yield return new Point(center.X + 1, center.Y - 1);
            yield return new Point(center.X - 1, center.Y + 1);
            yield return new Point(center.X + 1, center.Y + 1);
        }
        #endregion GetNeighbors

        #region GetNeighborTileLocation
        private Point GetNeighborTileLocation(Point point, char prev, char curr)
        {
            var x = point.X;
            var y = point.Y;
            var horizontalAdjust = 2;

            if (prev == 'n' || prev == 's')
            {
                y += (prev == 'n' ? 1 : -1);
                horizontalAdjust = 1;
            }

            x += ((curr == 'w' ? 1 : -1) * horizontalAdjust);

            return new Point(x, y);
        }
        #endregion GetNeighborTileLocation

        #region InitializeFloor
        public void InitializeFloor(IEnumerable<string> input)
        {
            foreach (var line in input)
            {
                FlipTileAt(line);
            }
        }
        #endregion InitializeFloor

        #region UpdateFloorForTheDay
        public void UpdateFloorForTheDay()
        {
            var newBlackTiles = new HashSet<Point>();
            var whiteTilesNextToBlack = new HashSet<Point>();

            // Check black tiles
            foreach (var black in BlackTiles)
            {
                var neighbors = GetNeighbors(black);
                var count = 0;
                foreach (var tile in neighbors)
                {
                    if (BlackTiles.Contains(tile))
                    {
                        count++;
                    }
                    else
                    {
                        whiteTilesNextToBlack.Add(tile);
                    }
                }
                if (count == 1 || count == 2)
                {
                    newBlackTiles.Add(black);
                }
            }

            // Check white tiles
            foreach (var white in whiteTilesNextToBlack)
            {
                var neighbors = GetNeighbors(white);
                var count = 0;
                foreach (var tile in neighbors)
                {
                    if (BlackTiles.Contains(tile))
                    {
                        count++;
                    }
                }
                if (count == 2)
                {
                    newBlackTiles.Add(white);
                }
            }

            BlackTiles = newBlackTiles;
        }
        #endregion UpdateFloorForTheDay
    }

}
