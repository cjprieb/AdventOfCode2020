using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace AdventOfCode2020.Day11
{
    [TestClass]
    public class Day11
    {
        #region Input

        public string[] GetInput() => Input.GetLines(Properties.Resources.Day11Input).ToArray();

        public string[] GetTestInput() => new string[]
        {
            "L.LL.LL.LL",
            "LLLLLLL.LL",
            "L.L.L..L..",
            "LLLL.LL.LL",
            "L.LL.LL.LL",
            "L.LLLLL.LL",
            "..L.L.....",
            "LLLLLLLLLL",
            "L.LLLLLL.L",
            "L.LLLLL.LL"
        };

        #endregion Input

        #region Code...

        #region GetOccupiedSeats
        public int GetOccupiedSeats(string[] input, int ruleset)
        {
            Lobby lobby = new Lobby(input, ruleset);
            do
            {
                lobby.DoRound();
            } while (!lobby.IsStabilized);
            return lobby.CountTotalOccupiedSeats();
        }
        #endregion GetOccupiedSeats

        #endregion Code...

        #region Tests...
        [TestMethod] public void Test1() => Assert.AreEqual(37, GetOccupiedSeats(GetTestInput(), 1));
        [TestMethod] public void Answer1() => Assert.AreEqual(2093, GetOccupiedSeats(GetInput(), 1));
        [TestMethod] public void Test2() => Assert.AreEqual(26, GetOccupiedSeats(GetTestInput(), 2));
        [TestMethod] public void Answer2() => Assert.AreEqual(1862, GetOccupiedSeats(GetInput(), 2));
        #endregion Tests...
    }

    public class Lobby
    {
        #region Member Variables...

        private char[,] _Lobby = new char[0, 0];
        private int _SeatsChanged = 0;
        private int _Rounds = 0;
        private int _Ruleset = 0;

        #endregion Member Variables...

        #region Properties...

        public bool IsStabilized => (_SeatsChanged == 0);

        public int Height => _Lobby.GetLength(1);
        public int Width => _Lobby.GetLength(0);

        #endregion Properties...

        #region Constructors...

        #region Lobby
        public Lobby(string[] input, int ruleset)
        {
            _Ruleset = ruleset;
            if (input.Length > 0)
            {
                _Lobby = new char[input[0].Length, input.Length];
            }
            for (int y = 0; y < input.Length; y++)
            {
                var line = input[y];
                for (int x = 0; x < line.Length; x++)
                {
                    _Lobby[x, y] = line[x];
                }
            }
        }
        #endregion Lobby

        #endregion Constructors...

        #region Methods...

        //#region CountEmptyAdjacentSeats
        //private int CountEmptyAdjacentSeats(int x, int y) => GetValidSeats(x, y).Where(IsEmptySeat).Count();
        //#endregion CountEmptyAdjacentSeats

        #region CountOccupiedAdjacentSeats
        private int CountOccupiedAdjacentSeats(int x, int y) => GetValidPoints(x, y).Where(IsFilledSeat).Count();
        #endregion CountOccupiedAdjacentSeats

        #region CountTotalOccupiedSeats
        public int CountTotalOccupiedSeats()
        {
            int count = 0;
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (_Lobby[x, y] == '#') count++;
                }
            }
            return count;
        }
        #endregion CountTotalOccupiedSeats

        #region DoRound
        public void DoRound()
        {
            char[,] newLobby = new char[Width, Height];
            _SeatsChanged = 0;
            _Rounds++;

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    newLobby[x, y] = GetNewValue(x, y);
                }
            }
            _Lobby = newLobby;
        }
        #endregion DoRound

        #region GetNearestSeat
        private Point? GetNearestSeat(int originX, int originY, int offsetX, int offsetY)
        {
            Point? nearestValidSeat = null;
            int x = originX + offsetX;
            int y = originY + offsetY;
            while (IsValidPoint(x, y) && !nearestValidSeat.HasValue)
            {
                if (_Lobby[x, y] != '.')
                {
                    nearestValidSeat = new Point(x, y);
                }
                x += offsetX;
                y += offsetY;
            }
            return nearestValidSeat;
        }
        #endregion GetNearestSeat

        #region GetNewValue
        private char GetNewValue(int x, int y)
        {
            char currentValue = _Lobby[x, y];
            char newValue = currentValue;

            if (currentValue == 'L') // empty
            {
                if (CountOccupiedAdjacentSeats(x, y) == 0)
                {
                    newValue = '#';
                    _SeatsChanged++;
                }
            }
            else if (currentValue == '#') // occupied
            {
                if (_Ruleset == 1)
                {
                    if (CountOccupiedAdjacentSeats(x, y) >= 4)
                    {
                        newValue = 'L';
                        _SeatsChanged++;
                    }
                }
                else
                {
                    if (CountOccupiedAdjacentSeats(x, y) >= 5)
                    {
                        newValue = 'L';
                        _SeatsChanged++;
                    }
                }
            }

            return newValue;
        }
        #endregion GetNewValue

        #region GetSurroundingPoints
        private IEnumerable<Point> GetSurroundingPoints(int x, int y)
        {
            bool onTopEdge = y == 0;
            bool onLeftEdge = x == 0;
            bool onBottomEdge = y == (Height - 1);
            bool onRightEdge = x == (Width - 1);

            if (!onTopEdge && !onLeftEdge) yield return new Point(x - 1, y - 1);
            if (!onTopEdge) yield return new Point(x, y - 1);
            if (!onTopEdge && !onRightEdge) yield return new Point(x + 1, y - 1);

            if (!onLeftEdge) yield return new Point(x - 1, y);
            if (!onRightEdge) yield return new Point(x + 1, y);

            if (!onBottomEdge && !onLeftEdge) yield return new Point(x - 1, y + 1);
            if (!onBottomEdge) yield return new Point(x, y + 1);
            if (!onBottomEdge && !onRightEdge) yield return new Point(x + 1, y + 1);
        }
        #endregion GetSurroundingPoints

        #region GetValidSeats
        private IEnumerable<Point> GetValidPoints(int x, int y)
        {
            if (_Ruleset == 1) return GetSurroundingPoints(x, y);

            List<Point> nearestSeats = new List<Point>();

            for (int offsetX = -1; offsetX <= 1; offsetX++)
            {
                for (int offsetY = -1; offsetY <= 1; offsetY++)
                {
                    if (offsetX == 0 && offsetY == 0) continue;
                    var point = GetNearestSeat(x, y, offsetX, offsetY);
                    if (point != null) nearestSeats.Add(point.Value);
                }
            }

            return nearestSeats;
        }
        #endregion GetValidSeats

        #region IsEmptySeat
        private bool IsEmptySeat(Point p) => _Lobby[p.X, p.Y] == 'L';
        #endregion IsEmptySeat

        #region IsFilledSeat
        private bool IsFilledSeat(Point p) => _Lobby[p.X, p.Y] == '#';
        #endregion IsFilledSeat

        #region IsValidPoint
        private bool IsValidPoint(int x, int y)
        {
            return x >= 0 && y >= 0 && x < Width && y < Height;
        }
        #endregion IsValidPoint

        #endregion Methods...
    }
}
