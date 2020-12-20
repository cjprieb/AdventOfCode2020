using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2020.Day20
{
    [TestClass]
    public class Day20
    {
        #region Input
        private static IEnumerable<string> GetInput() => Input.GetLines(Properties.Resources.Day20Input);
        private static IEnumerable<string> GetGridInput() => Input.GetLines(Properties.Resources.Day20GridActual);
        private static IEnumerable<string> GetTestGridInput() => Input.GetLines(Properties.Resources.Day20TestGridExpected);

        private static IEnumerable<string> GetTestInput() => new string[]
        {
            "Tile 1951:",
            "#.##...##.",
            "#.####...#",
            ".....#..##",
            "#...######",
            ".##.#....#",
            ".###.#####",
            "###.##.##.",
            ".###....#.",
            "..#.#..#.#",
            "#...##.#..",
            "",
            "Tile 2311:",
            "..##.#..#.",
            "##..#.....",
            "#...##..#.",
            "####.#...#",
            "##.##.###.",
            "##...#.###",
            ".#.#.#..##",
            "..#....#..",
            "###...#.#.",
            "..###..###",
            "",
            "Tile 1171:",
            "####...##.",
            "#..##.#..#",
            "##.#..#.#.",
            ".###.####.",
            "..###.####",
            ".##....##.",
            ".#...####.",
            "#.##.####.",
            "####..#...",
            ".....##...",
            "",
            "Tile 1427:",
            "###.##.#..",
            ".#..#.##..",
            ".#.##.#..#",
            "#.#.#.##.#",
            "....#...##",
            "...##..##.",
            "...#.#####",
            ".#.####.#.",
            "..#..###.#",
            "..##.#..#.",
            "",
            "Tile 1489:",
            "##.#.#....",
            "..##...#..",
            ".##..##...",
            "..#...#...",
            "#####...#.",
            "#..#.#.#.#",
            "...#.#.#..",
            "##.#...##.",
            "..##.##.##",
            "###.##.#..",
            "",
            "Tile 2473:",
            "#....####.",
            "#..#.##...",
            "#.##..#...",
            "######.#.#",
            ".#...#.#.#",
            ".#########",
            ".###.#..#.",
            "########.#",
            "##...##.#.",
            "..###.#.#.",
            "",
            "Tile 2971:",
            "..#.#....#",
            "#...###...",
            "#.#.###...",
            "##.##..#..",
            ".#####..##",
            ".#..####.#",
            "#..#.#..#.",
            "..####.###",
            "..#.#.###.",
            "...#.#.#.#",
            "",
            "Tile 2729:",
            "...#.#.#.#",
            "####.#....",
            "..#.#.....",
            "....#..#.#",
            ".##..##.#.",
            ".#.####...",
            "####.#.#..",
            "##.####...",
            "##..#.##..",
            "#.##...##.",
            "",
            "Tile 3079:",
            "#.#.#####.",
            ".#..######",
            "..#.......",
            "######....",
            "####.#..#.",
            ".#...#.##.",
            "#.#####.##",
            "..#.###...",
            "..#.......",
            "..#.###...",
        };

        #endregion Input

        #region Code...

        #region BuildImage
        private static void BuildImage(IEnumerable<string> input)
        {
            var image = new ImageTiles();
            image.Parse(input.ToArray());
            image.CreateImage(includeBorders: false);
        }
        #endregion BuildImage

        #region FindProductOfCorners
        private static long FindProductOfCorners(IEnumerable<string> input)
        {
            var image = new ImageTiles();
            image.Parse(input.ToArray());
            var corners = image.FindCornerTiles();
            Assert.AreEqual(4, corners.Length);
            var answer = corners.Aggregate(1L, (prod, next) => prod * next);
            return answer;
        }
        #endregion FindProductOfCorners

        #region FindOceanRoughness
        private static long FindOceanRoughness(IEnumerable<string> input)
        {
            Assert.IsTrue(input.Count() > 1, "invalid input");
            var image = new CameraImage();
            image.ParseAsImage(input.ToArray());
            image.FindSeaMonsters();
            return image.OceanRoughness;
        }
        #endregion FindOceanRoughness

        #endregion Code...

        #region Tests...
        [TestMethod] public void Test1() => Assert.AreEqual(20899048083289, FindProductOfCorners(GetTestInput()));
        [TestMethod] public void Answer1() => Assert.AreEqual(60145080587029, FindProductOfCorners(GetInput()));

        [TestMethod] public void BuildImageForTest() => BuildImage(GetTestInput());
        [TestMethod] public void BuildImageForAnswer() => BuildImage(GetInput());

        [TestMethod] public void Test2() => Assert.AreEqual(273, FindOceanRoughness(GetTestGridInput()));

        /// <summary>
        /// 2471 is too high
        /// </summary>
        [TestMethod] public void Answer2() => Assert.AreEqual(1901, FindOceanRoughness(GetGridInput()));

        [TestMethod]
        public void GetNextPointToPlace_1_1()
        {
            (int x, int y) = ImageTiles.GetNextPointToPlace(1, 1, 3);
            Assert.AreEqual(new Point(0, 2), new Point(x, y));
        }

        [TestMethod]
        public void GetNextPointToPlace_0_2()
        {
            (int x, int y) = ImageTiles.GetNextPointToPlace(0, 2, 3);
            Assert.AreEqual(new Point(1, 2), new Point(x,y));
        }

        [TestMethod]
        public void GetNextPointToPlace_1_2()
        {
            (int x, int y) = ImageTiles.GetNextPointToPlace(1, 2, 3);
            Assert.AreEqual(new Point(2, 0), new Point(x, y));
        }

        [TestMethod]
        public void GetNextPointToPlace_2_0()
        {
            (int x, int y) = ImageTiles.GetNextPointToPlace(2, 0, 3);
            Assert.AreEqual(new Point(2, 1), new Point(x, y));
        }
        #endregion Tests...
    }

    public class RotationHelper
    {
        #region FlipHorizontal
        public static T[,] FlipHorizontal<T>(T[,] grid)
        {
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);
            T[,] flippedGrid = new T[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var adjX = x;
                    var adjY = height - y - 1;
                    flippedGrid[adjX, adjY] = grid[x, y];
                }
            }

            return flippedGrid;
        }
        #endregion FlipHorizontal

        #region FlipVertical
        public static T[,] FlipVertical<T>(T[,] grid)
        {
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);
            T[,] flippedGrid = new T[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var adjX = width - x - 1;
                    var adjY = y;
                    flippedGrid[adjX, adjY] = grid[x, y];
                }
            }

            return flippedGrid;
        }
        #endregion FlipVertical

        #region RotateLeft
        public static T[,] RotateLeft<T>(T[,] grid)
        {
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);
            T[,] rotatedGrid = new T[height, width];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var adjX = y;
                    var adjY = width - x - 1;
                    rotatedGrid[adjX, adjY] = grid[x, y];
                }
            }

            return rotatedGrid;
        }
        #endregion RotateLeft

        #region RotateRight
        public static T[,] RotateRight<T>(T[,] grid)
        {
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);
            T[,] rotatedGrid = new T[height, width];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var adjX = height - y - 1;
                    var adjY = x;
                    rotatedGrid[adjX, adjY] = grid[x, y];
                }
            }

            return rotatedGrid;
        }
        #endregion RotateRight

        #region JoinToBottom
        public static T[,] JoinToBottom<T>(T[,] _Grid, string edgeToMatch, string top, string left, string bottom, string right)
        {
            var reversedEdgeToMatch = Tile.Reverse(edgeToMatch);
            if (top == edgeToMatch)
            {
                // do nothing; already in correct orientation
            }
            else if (right == edgeToMatch)
            {
                _Grid = RotateLeft(_Grid);
            }
            else if (bottom == edgeToMatch)
            {
                _Grid = FlipHorizontal(_Grid);
            }
            else if (left == edgeToMatch)
            {
                _Grid = FlipHorizontal(_Grid);
                _Grid = RotateRight(_Grid);
            }
            else if (top == reversedEdgeToMatch)
            {
                _Grid = FlipVertical(_Grid);
            }
            else if (right == reversedEdgeToMatch)
            {
                _Grid = RotateLeft(_Grid);
                _Grid = FlipVertical(_Grid);
            }
            else if (bottom == reversedEdgeToMatch)
            {
                _Grid = RotateLeft(_Grid);
                _Grid = RotateLeft(_Grid);
            }
            else if (left == reversedEdgeToMatch)
            {
                _Grid = RotateRight(_Grid);
            }
            return _Grid;
        }
        #endregion JoinToBottom

        #region JoinToLeft
        public static T[,] JoinToLeft<T>(T[,] _Grid, string edgeToMatch, string top, string left, string bottom, string right)
        {
            var reversedEdgeToMatch = Tile.Reverse(edgeToMatch);
            if (top == edgeToMatch)
            {
                _Grid = RotateLeft(_Grid);
                _Grid = FlipHorizontal(_Grid);
            }
            else if (right == edgeToMatch)
            {
                _Grid = FlipVertical(_Grid);
            }
            else if (bottom == edgeToMatch)
            {
                _Grid = RotateRight(_Grid);
            }
            else if (left == edgeToMatch)
            {
                // do nothing; already in correct orientation
            }
            else if (top == reversedEdgeToMatch)
            {
                _Grid = RotateLeft(_Grid);
            }
            else if (right == reversedEdgeToMatch)
            {
                _Grid = RotateLeft(_Grid);
                _Grid = RotateLeft(_Grid);
            }
            else if (bottom == reversedEdgeToMatch)
            {
                _Grid = RotateRight(_Grid);
                _Grid = FlipHorizontal(_Grid);
            }
            else if (left == reversedEdgeToMatch)
            {
                _Grid = FlipHorizontal(_Grid);
            }
            return _Grid;
        }
        #endregion JoinToLeft
    }

    public class Tile
    {
        #region Member Variables...
        private const int _TileSideLength = 10;
        private char[,] _Grid = new char[_TileSideLength, _TileSideLength];
        #endregion Member Variables...

        #region Properties...
        public string TopEdge => new string(GetRange(0, _TileSideLength - 1).Select(x => _Grid[x, 0]).ToArray());
        public string BottomEdge => new string(GetRange(0, _TileSideLength - 1).Select(x => _Grid[x, _TileSideLength - 1]).ToArray());
        public string LeftEdge => new string(GetRange(0, _TileSideLength - 1).Select(y => _Grid[0, y]).ToArray());
        public string RightEdge => new string(GetRange(0, _TileSideLength - 1).Select(y => _Grid[_TileSideLength - 1, y]).ToArray());

        public HashSet<string> Edges { get; private set; } = new HashSet<string>();

        public int Id { get; set; }

        public Dictionary<string, int> MatchingEdges { get; private set; } = new Dictionary<string, int>();
        public char[,] Grid { get; set; } = new char[_TileSideLength, _TileSideLength];
        #endregion Properties...

        #region Constructors...
        public Tile(int id, string[] tile)
        {
            Id = id;
            ParseEdges(tile);
        }
        #endregion Constructors...

        #region Methods...

        #region AddMatchingEdges
        public void AddMatchingEdges(Tile tile)
        {
            foreach (var key in Edges)
            {
                if (tile.IsMatch(key, out string matchingEdge))
                {
                    if (MatchingEdges.ContainsKey(key)) throw new ArgumentException($"{key} already exists in edges for {MatchingEdges[key]} - tried to add {tile.Id}");
                    MatchingEdges.Add(key, tile.Id);

                    if (tile.MatchingEdges.ContainsKey(key)) throw new ArgumentException($"{matchingEdge} already exists in edges for {tile.MatchingEdges[key]} - tried to add {Id}");
                    tile.MatchingEdges.Add(matchingEdge, Id);
                }
            }
        }
        #endregion AddMatchingEdges

        #region CountEdgesWithMatches
        public int CountEdgesWithMatches()
        {
            return MatchingEdges.Values.Count;
        }
        #endregion CountEdgesWithMatches

        #region GetRange
        public static IEnumerable<int> GetRange(int min, int max)
        {
            for (int i = min; i <= max; i++) yield return i;
        }
        #endregion GetRange

        #region GetValueAt
        public char GetValueAt(int x, int y, bool includeBorders)
        {
            if (includeBorders)
            {
                return _Grid[x % 10, y % 10];
            }

            var imageBounds = _TileSideLength - 2;

            var gridX = (x % imageBounds) + 1;
            if (gridX < 0 || gridX >= _TileSideLength) throw new IndexOutOfRangeException($"X index {gridX} is outside the bounds of the grid: {imageBounds}");

            var gridY = (y % imageBounds) + 1;
            if (gridY < 0 || gridY >= _TileSideLength) throw new IndexOutOfRangeException($"Y index {gridY} is outside the bounds of the grid: {imageBounds}");

            return _Grid[gridX, gridY];
        }
        #endregion GetValueAt

        #region IsMatch
        private bool IsMatch(string key, out string matchingEdge)
        {
            matchingEdge = null;
            string reversedKey = Reverse(key);
            //if (key == reversedKey) throw new Exception($"{key} is a palindrome");

            if (Edges.Contains(key))
            {
                matchingEdge = key;
                return true;
            }
            else
            {
                if (Edges.Contains(reversedKey))
                {
                    matchingEdge = reversedKey;
                    return true;
                }
            }
            return false;
        }
        #endregion IsMatch

        #region JoinTo
        internal void JoinTo(Tile tileAbove, Tile tileLeft)
        {
            if (tileAbove != null)
            {
                _Grid = RotationHelper.JoinToBottom(_Grid, tileAbove.BottomEdge, TopEdge, LeftEdge, BottomEdge, RightEdge);
            }

            if (tileLeft != null)
            {
                _Grid = RotationHelper.JoinToLeft(_Grid, tileLeft.RightEdge, TopEdge, LeftEdge, BottomEdge, RightEdge);
            }
        }
        #endregion JoinTo

        #region ParseEdges
        private void ParseEdges(string[] tile)
        {
            if (tile.Length != _TileSideLength) throw new Exception($"The tile needs to have {_TileSideLength} rows, but found {tile.Length}");

            for (int y = 0; y < _TileSideLength; y++)
            {
                var line = tile[y];
                if (line.Length != _TileSideLength) throw new Exception($"The line at {y} needs to have {_TileSideLength} characters, but found {line.Length}");
                for (int x = 0; x < _TileSideLength; x++)
                {
                    _Grid[x, y] = line[x];
                }
            }

            Edges.Add(TopEdge);
            Edges.Add(BottomEdge);
            Edges.Add(LeftEdge);
            Edges.Add(RightEdge);
        }
        #endregion ParseEdges

        #region Reverse
        public static string Reverse(string key)
        {
            return new string(key.Reverse().ToArray());
        }
        #endregion Reverse

        #region ToString
        public override string ToString()
        {
            return Id + ": " + string.Join(", ", Edges.Select(edge => $"{edge} ({(MatchingEdges.ContainsKey(edge) ? MatchingEdges[edge].ToString() : "")})"));
        }
        #endregion ToString

        #region PrintGrid
        public void PrintGrid()
        {
            StringBuilder builder = new StringBuilder();
            for (int y = 0; y < _Grid.GetLength(1); y++)
            {
                for (int x = 0; x < _Grid.GetLength(0); x++)
                {
                    builder.Append(_Grid[x, y]);
                }
                builder.Append(Environment.NewLine);
            }
            Console.WriteLine("Tile " + Id);
            Console.WriteLine(builder.ToString());
            Console.WriteLine();
        }
        #endregion PrintGrid

        #endregion Methods...
    }

    public class ImageTiles
    {
        #region Member Variables...

        private static Regex _TileIdPattern = new Regex(@"Tile (\d+):");

        private Dictionary<int, Tile> _Tiles = new Dictionary<int, Tile> ();
        private int _SideLength = 0;

        #endregion Member Variables...

        #region Constructors...
        #endregion Constructors...

        #region Methods...

        #region CreateImage
        public void CreateImage(bool includeBorders)
        {
            var tileSideLength = includeBorders ? 10 : 8;
            var tileArray = PlaceTiles();
            var imageSideLength = tileSideLength * _SideLength;
            var builder = new StringBuilder();

            for (int y = 0; y < imageSideLength; y++)
            {
                if (includeBorders)
                {
                    if (y != 0 && y % 10 == 0) Console.WriteLine();
                    if (y != 0) Console.WriteLine();
                }

                for (int x = 0; x < imageSideLength; x++)
                {
                    var c = GetTileValueAt(tileArray, x, y, tileSideLength, includeBorders);
                    builder.Append(c);

                    if (includeBorders)
                    {
                        if (x != 0 && x % 10 == 0) Console.Write(' ');
                        Console.Write(c);
                    }
                }
                builder.Append(Environment.NewLine);
            }
            if (!includeBorders)
            {
                Console.WriteLine(builder.ToString());
            }
        }
        #endregion CreateImage

        #region FindCommonTileId
        private int FindCommonTileId(HashSet<int> placedTiles, Tile tileAbove, Tile tileLeft)
        {
            var commonTileId = 0;
            foreach (var linkedTileId in tileAbove.MatchingEdges.Values)
            {
                if (!placedTiles.Contains(linkedTileId) && tileLeft.MatchingEdges.Values.Any(id => id == linkedTileId))
                {
                    if (commonTileId != 0) throw new Exception($"Found more than one unplaced tile for {tileAbove.Id} and {tileLeft.Id}: {linkedTileId} and {commonTileId}");
                    commonTileId = linkedTileId;
                }
            }
            return commonTileId;
        }
        #endregion FindCommonTileId

        #region FindCornerTiles
        public int[] FindCornerTiles()
        {
            FindMatchingEdges();
            return _Tiles.Values
                .Where(tile => tile.CountEdgesWithMatches() == 2)
                .Select(tile => tile.Id)
                .ToArray();
        }
        #endregion FindCornerTiles

        #region FindMatchingEdges
        public void FindMatchingEdges()
        {
            var tiles = _Tiles.Values.ToList();
            for (int i = 0; i < tiles.Count; i++)
            {
                var firstTile = tiles[i];
                for (var j = i+1; j < tiles.Count; j++)
                {
                    var secondTile = tiles[j];
                    if (firstTile.Id == secondTile.Id) continue;

                    firstTile.AddMatchingEdges(secondTile);
                }
            }
        }
        #endregion FindMatchingEdges

        #region FindUnplacedTileId
        private int FindUnplacedTileId(HashSet<int> placedTiles, Tile singleTile)
        {
            var id = 0;
            foreach (var edge in singleTile.MatchingEdges.Values)
            {
                if (!placedTiles.Contains(edge))
                {
                    //if (id != 0) throw new Exception($"Found more than one unplaced tile for {singleTile}: {id} and {edge}");
                    id = edge;
                }
            }
            return id;
        }
        #endregion FindUnplacedTileId

        #region GetNextPointToPlace
        public static (int x, int y) GetNextPointToPlace(int x, int y, int sideLength)
        {
            var nextX = x;
            var nextY = y;

            if (nextX == 0)
            {
                nextX = 1;
                nextY = y;
            }
            else
            {
                nextY++;
                if (nextY == sideLength)
                {
                    nextX++;
                    nextY = 0;
                }
                else if (nextX == 1)
                {
                    nextX = 0;
                }
            }
            return (nextX, nextY);
        }
        #endregion GetNextPointToPlace

        #region GetTileValueAt
        private char GetTileValueAt(Dictionary<Point, Tile> tileArray, int x, int y, int tileSideLength, bool includeBorders)
        {
            var tileX = (int)(x / tileSideLength);
            var tileY = (int)(y / tileSideLength);
            return tileArray[new Point(tileX, tileY)].GetValueAt(x, y, includeBorders);
        }
        #endregion GetTileValueAt

        #region Parse
        public void Parse(string[] input)
        {
            var tileInputHeight = 10;
            for (int i = 0; i < input.Length; i += tileInputHeight + 2)
            {
                ParseTile(input, i, tileInputHeight);
            }
            _SideLength = (int)Math.Sqrt(_Tiles.Count);
        }
        #endregion Parse

        #region ParseTile
        public void ParseTile(string[] input, int start, int height)
        {
            Match match = _TileIdPattern.Match(input[start]);
            if (!match.Success) throw new Exception($"{input[start]} does not match the tile id pattern");

            var id = int.Parse(match.Groups[1].Value);
            _Tiles[id] = new Tile(id, input.Skip(start + 1).Take(height).ToArray());
        }
        #endregion ParseTile

        #region PlaceTiles
        private Dictionary<Point, Tile> PlaceTiles()
        {
            var corners = FindCornerTiles();
            var tileArray = new Dictionary<Point, Tile>();
            var placedTiles = new HashSet<int>();
            var cornerTile = _Tiles[corners[0]];

            tileArray[new Point(0, 0)] = cornerTile;
            cornerTile.Grid = RotationHelper.FlipHorizontal(cornerTile.Grid);

            placedTiles.Add(corners[0]);

            int x = 1, y = 0;
            while (placedTiles.Count != _Tiles.Count)
            {
                var tileIdToPlace = 0;

                var pointAbove = new Point(x, y - 1);
                Tile tileAbove = tileArray.ContainsKey(pointAbove) ? tileArray[pointAbove] : null;

                var pointLeft = new Point(x - 1, y);
                Tile tileLeft = tileArray.ContainsKey(pointLeft) ? tileArray[pointLeft] : null;

                if (tileAbove != null && tileLeft != null)
                {
                    tileIdToPlace = FindCommonTileId(placedTiles, tileAbove, tileLeft);
                }
                else if (x == 0 || y == 0)
                {
                    var singleTile = tileAbove != null ? tileAbove : tileLeft;
                    tileIdToPlace = FindUnplacedTileId(placedTiles, singleTile);
                }
                if (tileIdToPlace != 0)
                {
                    placedTiles.Add(tileIdToPlace);
                    tileArray[new Point(x, y)] = _Tiles[tileIdToPlace];
                    _Tiles[tileIdToPlace].JoinTo(tileAbove, tileLeft);

                    (x, y) = GetNextPointToPlace(x, y, _SideLength);
                }
                else
                {
                    throw new Exception($"No tile find to place at ({x},{y})");
                }
            }
            return tileArray;
        }
        #endregion PlaceTiles

        #endregion Methods...
    }
            
    public class CameraImage
    {
        #region Member Variables...
        private char[,] _Image = new char[,] { };
        private char[,] _SeaMonster = new char[,] { };
        #endregion Member Variables...

        #region Properties...
        public long OceanRoughness
        {
            get
            {
                var sum = 0;
                for (int x = 0; x < _Image.GetLength(0); x++)
                {
                    for (int y = 0; y < _Image.GetLength(1); y++)
                    {
                        if (_Image[x, y] == '#') sum++;
                    }
                }
                return sum;
            }
        }

        #endregion Properties...

        #region Constructors...
        public CameraImage()
        {
            BuildSeaMonsterMask();
        }
        #endregion Constructors...

        #region Methods...

        #region BuildSeaMonsterMask
        private void BuildSeaMonsterMask()
        {
            var seaMonster = new string[]
            {
                "                  # ",
                "#    ##    ##    ###",
                " #  #  #  #  #  #   "
            };
            _SeaMonster = new char[seaMonster[0].Length, seaMonster.Length];
            for (int y = 0; y < seaMonster.Length; y++)
            {
                for (int x = 0; x < seaMonster[y].Length; x++)
                {
                    _SeaMonster[x, y] = seaMonster[y][x];
                }
            }
        }
        #endregion BuildSeaMonsterMask

        #region DoesRegionMatch
        private bool DoesRegionMatch(char[,] monster, int xMin, int yMin)
        {
            bool isMatch = true;
            for (int x = 0; x < monster.GetLength(0); x++)
            {
                for (int y = 0; y < monster.GetLength(1); y++)
                {
                    if (monster[x, y] == ' ') continue;

                    if (_Image[x+xMin, y+yMin] == '.')
                    {
                        isMatch = false;
                        break;
                    }
                }
            }
            return isMatch;
        }
        #endregion DoesRegionMatch

        #region FindSeaMonsters
        internal void FindSeaMonsters()
        {
            FindSeaMonsters(_SeaMonster);

            _SeaMonster = RotationHelper.RotateLeft(_SeaMonster);
            FindSeaMonsters(_SeaMonster);

            _SeaMonster = RotationHelper.RotateLeft(_SeaMonster);
            FindSeaMonsters(_SeaMonster);

            _SeaMonster = RotationHelper.RotateLeft(_SeaMonster);
            FindSeaMonsters(_SeaMonster);

            _SeaMonster = RotationHelper.FlipHorizontal(_SeaMonster);
            FindSeaMonsters(_SeaMonster);

            _SeaMonster = RotationHelper.RotateLeft(_SeaMonster);
            FindSeaMonsters(_SeaMonster);

            _SeaMonster = RotationHelper.RotateLeft(_SeaMonster);
            FindSeaMonsters(_SeaMonster);

            _SeaMonster = RotationHelper.RotateLeft(_SeaMonster);
            FindSeaMonsters(_SeaMonster);

            PrintGrid();
        }
        #endregion FindSeaMonsters

        #region FindSeaMonsters
        private void FindSeaMonsters(char[,] monster)
        {
            int maxX = _Image.GetLength(0) - monster.GetLength(0);
            int maxY = _Image.GetLength(1) - monster.GetLength(1);
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    if (DoesRegionMatch(monster, x, y))
                    {
                        MarkMonster(monster, x, y);
                    }
                }
            }
        }
        #endregion FindSeaMonsters

        #region ParseAsImage
        public void ParseAsImage(string[] input)
        {
            var imageSideLength = input.Length;
            _Image = new char[imageSideLength, imageSideLength];

            for (int y = 0; y < imageSideLength; y++)
            {
                for (int x = 0; x < imageSideLength; x++)
                {
                    _Image[x, y] = input[y][x];
                }
            }
        }
        #endregion ParseAsImage

        #region MarkMonster
        private void MarkMonster(char[,] monster, int xMin, int yMin)
        {
            for (int x = 0; x < monster.GetLength(0); x++)
            {
                for (int y = 0; y < monster.GetLength(1); y++)
                {
                    if (monster[x, y] == ' ') continue;

                    _Image[x + xMin, y + yMin] = 'O';
                }
            }
        }
        #endregion MarkMonster

        #region PrintGrid
        public void PrintGrid()
        {
            StringBuilder builder = new StringBuilder();
            for (int y = 0; y < _Image.GetLength(1); y++)
            {
                for (int x = 0; x < _Image.GetLength(0); x++)
                {
                    builder.Append(_Image[x, y]);
                }
                builder.Append(Environment.NewLine);
            }
            Console.WriteLine(builder.ToString());
            Console.WriteLine();
        }
        #endregion PrintGrid

        #endregion Methods...
    }
}
