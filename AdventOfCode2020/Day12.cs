using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace AdventOfCode2020.Day12
{
    [TestClass]
    public class Day12
    {
        #region Input

        public IEnumerable<string> GetInput() => Input.GetLines(Properties.Resources.Day12Input);

        public string[] GetTestInput() => new string[]
        {
            "F10",
            "N3",
            "F7",
            "R90",
            "F11",
        };

        public string[] GetTestInput2() => new string[]
        {
            "F10",
            "N3",
            "F7",
            "L90",
            "F11",
        };

        public string[] GetTestInput3() => new string[]
        {
            "E10",
            "N3",
            "F7",
            "L180",
            "F11",
        };

        public string[] GetTestInput4() => new string[]
        {
            "F10",
            "N3",
            "F7",
            "S11",
        };

        #endregion Input

        #region Code...

        #region GetDistance
        private int GetDistance(IEnumerable<string> input)
        {
            var ship = new Ship();
            foreach (var instruction in input)
            {
                ship.Navigate(instruction);
            }
            var endingLocation = ship.Location;
            return Math.Abs(endingLocation.X) + Math.Abs(endingLocation.Y);
        }
        #endregion GetDistance

        #region GetDistance2
        private int GetDistance2(IEnumerable<string> input)
        {
            var ship = new Ship2();
            foreach (var instruction in input)
            {
                ship.Navigate(instruction);
            }
            var endingLocation = ship.Location;
            return Math.Abs(endingLocation.X) + Math.Abs(endingLocation.Y);
        }
        #endregion GetDistance2

        #endregion Code...

        #region Tests...
        [TestMethod] public void Test1a() => Assert.AreEqual(25, GetDistance(GetTestInput()));
        [TestMethod] public void Test1b() => Assert.AreEqual(31, GetDistance(GetTestInput2()));
        [TestMethod] public void Test1c() => Assert.AreEqual(9, GetDistance(GetTestInput3()));
        [TestMethod] public void Test1d() => Assert.AreEqual(25, GetDistance(GetTestInput4()));

        [TestMethod] public void Answer1() => Assert.AreEqual(1645, GetDistance(GetInput()));

        [TestMethod] public void Test2a() => Assert.AreEqual(286, GetDistance2(GetTestInput()));
        [TestMethod] public void Answer2() => Assert.AreEqual(35292, GetDistance2(GetInput()));
        #endregion Tests...
    }

    public enum Direction { North, South, East, West }

    public class Ship
    {
        #region Properties...

        public Point Location { get; set; } = new Point(0, 0);
        public Direction FacingDirection { get; set; } = Direction.East;

        #endregion Properties...

        #region Move
        private void Move(Direction direction, int value)
        {
            int xOffset = 0;
            int yOffset = 0;
            switch (direction)
            {
                case Direction.North: xOffset = value; break;
                case Direction.South: xOffset = -value; break;
                case Direction.East: yOffset = value; break;
                case Direction.West: yOffset = -value; break;
                default:
                    break;
            }
            Location = new Point(Location.X + xOffset, Location.Y + yOffset);
        }
        #endregion Move

        #region Navigate
        public void Navigate(string instruction)
        {
            char action = instruction[0];
            int value = int.Parse(instruction.Substring(1));
            switch (action)
            {
                case 'N': Move(Direction.North, value); break;
                case 'S': Move(Direction.South, value); break;
                case 'E': Move(Direction.East, value); break;
                case 'W': Move(Direction.West, value); break;
                case 'R': Rotate(value); break;
                case 'L': Rotate(360 - value); break;
                case 'F': Move(FacingDirection, value); break;
                default: break;
            }
        }
        #endregion Navigate

        #region Rotate
        private void Rotate(int degreesToTheRight)
        {
            if (degreesToTheRight == 90)
            {
                switch (FacingDirection)
                {
                    case Direction.North: FacingDirection = Direction.East; break;
                    case Direction.South: FacingDirection = Direction.West; break;
                    case Direction.East: FacingDirection = Direction.South; break;
                    case Direction.West: FacingDirection = Direction.North; break;
                }
            }
            else if (degreesToTheRight == 180)
            {
                switch (FacingDirection)
                {
                    case Direction.North: FacingDirection = Direction.South; break;
                    case Direction.South: FacingDirection = Direction.North; break;
                    case Direction.East: FacingDirection = Direction.West; break;
                    case Direction.West: FacingDirection = Direction.East; break;
                }
            }
            else if (degreesToTheRight == 270)
            {
                switch (FacingDirection)
                {
                    case Direction.North: FacingDirection = Direction.West; break;
                    case Direction.South: FacingDirection = Direction.East; break;
                    case Direction.East: FacingDirection = Direction.North; break;
                    case Direction.West: FacingDirection = Direction.South; break;
                }
            }
            else if (degreesToTheRight != 0)
            {
                throw new Exception($"{degreesToTheRight} is an invalid degree");
            }
        }
        #endregion Rotate
    }

    public class Ship2
    {
        #region Properties...

        public Point Location { get; set; } = new Point(0, 0);
        public Point Waypoint { get; set; } = new Point(10, -1);
        //public Direction FacingDirection { get; set; } = Direction.East;

        #endregion Properties...

        #region MoveForward
        private void MoveForward(int value)
        {
            int xOffset = Waypoint.X * value;
            int yOffset = Waypoint.Y * value;
            Location = new Point(Location.X + xOffset, Location.Y + yOffset);
        }
        #endregion MoveForward

        #region MoveWaypoint
        private void MoveWaypoint(Direction direction, int value)
        {
            int xOffset = 0;
            int yOffset = 0;
            switch (direction)
            {
                case Direction.North: yOffset = -value; break;
                case Direction.South: yOffset = value; break;
                case Direction.East: xOffset = value; break;
                case Direction.West: xOffset = -value; break;
                default:
                    break;
            }
            Waypoint = new Point(Waypoint.X + xOffset, Waypoint.Y + yOffset);
        }
        #endregion MoveWaypoint

        #region Navigate
        public void Navigate(string instruction)
        {
            char action = instruction[0];
            int value = int.Parse(instruction.Substring(1));
            switch (action)
            {
                case 'N': MoveWaypoint(Direction.North, value); break;
                case 'S': MoveWaypoint(Direction.South, value); break;
                case 'E': MoveWaypoint(Direction.East, value); break;
                case 'W': MoveWaypoint(Direction.West, value); break;
                case 'R': Rotate(value); break;
                case 'L': Rotate(360 - value); break;
                case 'F': MoveForward(value); break;
                default: break;
            }
        }
        #endregion Navigate

        #region Rotate
        private void Rotate(int degreesToTheRight)
        {
            if (degreesToTheRight == 90)
            {
                // 10, -4 => 4, 10 => -10, 4 => -4, -10
                Waypoint = new Point(-Waypoint.Y, Waypoint.X);
            }
            else if (degreesToTheRight == 180)
            {
                Waypoint = new Point(-Waypoint.X, -Waypoint.Y);
            }
            else if (degreesToTheRight == 270)
            {
                Waypoint = new Point(Waypoint.Y, -Waypoint.X);
            }
            else if (degreesToTheRight != 0)
            {
                throw new Exception($"{degreesToTheRight} is an invalid degree");
            }
        }
        #endregion Rotate
    }

}
