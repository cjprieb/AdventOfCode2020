using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2020.Day23
{
    [TestClass]
    public class Day23
    {
        #region Input
        public int[] GetInput() => new int[] { 2, 5, 3, 1, 4, 9, 8, 6, 7 };
        public int[] GetTestInput() => new int[] { 3, 8, 9, 1, 2, 5, 4, 6, 7 };
        #endregion Input

        #region Code...

        #region GetCurrentCupAfter
        public int GetCurrentCupAfter(int[] input, int moves)
        {
            var game = new CupCircle(input);
            for (int i = 0; i < moves; i++)
            {
                game.Move();
            }
            return game.CurrentCup;
        }
        #endregion GetCurrentCupAfter

        #region GetDestinationAfter
        public int GetDestinationAfter(int[] input, int moves)
        {
            var game = new CupCircle(input);
            for (int i = 0; i < moves; i++)
            {
                game.Move();
            }
            return game.Destination;
        }
        #endregion GetDestinationAfter

        #region PlayGame
        public string PlayGame(int[] input, int moves)
        {
            var game = new CupCircle(input);
            for (int i = 0; i < moves; i++)
            {
                game.Move();
            }
            return game.Arrangement;
        }
        #endregion PlayGame

        #region PlayLongGame
        public long PlayLongGame(int[] input, int moves)
        {
            var game = new CupCircle(input, true);
            for (int i = 0; i < moves; i++)
            {
                game.Move();
            }
            return game.LongResult;
        }
        #endregion PlayLongGame

        #endregion Code...

        #region Tests...
        [TestMethod] public void GetCurrentCupAfter_1Move() => Assert.AreEqual(2, GetCurrentCupAfter(GetTestInput(), 1));
        [TestMethod] public void GetCurrentCupAfter_2Moves() => Assert.AreEqual(5, GetCurrentCupAfter(GetTestInput(), 2));
        [TestMethod] public void GetCurrentCupAfter_3Moves() => Assert.AreEqual(8, GetCurrentCupAfter(GetTestInput(), 3));
        [TestMethod] public void GetCurrentCupAfter_4Moves() => Assert.AreEqual(4, GetCurrentCupAfter(GetTestInput(), 4));
        [TestMethod] public void GetCurrentCupAfter_5Moves() => Assert.AreEqual(1, GetCurrentCupAfter(GetTestInput(), 5));
        [TestMethod] public void GetCurrentCupIndexAfter_6Moves() => Assert.AreEqual(9, GetDestinationAfter(GetTestInput(), 6));

        [TestMethod] public void GetTestDestinationAfter_1Move() => Assert.AreEqual(2, GetDestinationAfter(GetTestInput(), 1));
        [TestMethod] public void GetTestDestinationAfter_2Moves() => Assert.AreEqual(7, GetDestinationAfter(GetTestInput(), 2));
        [TestMethod] public void GetTestDestinationAfter_3Moves() => Assert.AreEqual(3, GetDestinationAfter(GetTestInput(), 3));
        [TestMethod] public void GetTestDestinationAfter_4Moves() => Assert.AreEqual(7, GetDestinationAfter(GetTestInput(), 4));
        [TestMethod] public void GetTestDestinationAfter_5Moves() => Assert.AreEqual(3, GetDestinationAfter(GetTestInput(), 5));
        [TestMethod] public void GetTestDestinationAfter_6Moves() => Assert.AreEqual(9, GetDestinationAfter(GetTestInput(), 6));

        [TestMethod] public void PlayTestGame_1Move() => Assert.AreEqual("54673289", PlayGame(GetTestInput(), 1));
        [TestMethod] public void PlayTestGame_3Moves() => Assert.AreEqual("34672589", PlayGame(GetTestInput(), 3));
        [TestMethod] public void PlayTestGame_6Moves() => Assert.AreEqual("93672584", PlayGame(GetTestInput(), 6));
        [TestMethod] public void PlayTestGame_10Moves() => Assert.AreEqual("92658374", PlayGame(GetTestInput(), 10));

        [TestMethod] public void Test1() => Assert.AreEqual("67384529", PlayGame(GetTestInput(), 100));
        [TestMethod] public void Answer1() => Assert.AreEqual("34952786", PlayGame(GetInput(), 100));

        [TestMethod] public void Test2() => Assert.AreEqual(149245887792, PlayLongGame(GetTestInput(), 10000000));
        [TestMethod] public void Answer2() => Assert.AreEqual(505334281774, PlayLongGame(GetInput(), 10000000));    
        #endregion Tests...
    }

    public class CupCircle
    {
        #region Member Variables...
        private LinkedList<int> _Circle = null;
        private LinkedListNode<int> _CurrentCup = null;
        private Dictionary<int, LinkedListNode<int>> _AllCups = new Dictionary<int, LinkedListNode<int>>();
        private int _TotalItems = 0;

        //private int _Highest = int.MinValue;
        //private int _Lowest = int.MaxValue;
        #endregion Member Variables...

        #region Properties...
        public string Arrangement
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                var cupOne = _AllCups[1];
                var next = cupOne.Next ?? _Circle.First;
                for (int i = 1; i < 9; i++)
                {
                    builder.Append(next.Value);
                    next = next.Next ?? _Circle.First;
                }
                return builder.ToString();
            }
        }
        public int Destination { get; private set; }
        public int CurrentCup => _CurrentCup.Value;
        public long LongResult
        {
            get
            {
                var cupOne = _AllCups[1];
                var next1 = cupOne.Next ?? _Circle.First;
                var next2 = next1.Next ?? _Circle.First;
                return (long)next1.Value * (long)next2.Value;
            }
        }
        #endregion Properties...

        #region Constructors...
        public CupCircle(int[] cups, bool isLongGame = false)
        {
            _Circle = new LinkedList<int>();
            _TotalItems = isLongGame ? 1000000 : cups.Length;

            for (int i = 1; i <= _TotalItems; i++)
            {
                var value = i <= cups.Length ? cups[i - 1] : i;
                _AllCups[value] = _Circle.AddLast(value);
            }

            _CurrentCup = _Circle.First;
            //_Highest = _TotalItems;
            //_Lowest = 1;
        }
        #endregion Constructors...

        #region Methods...

        #region BuildNextCircle
        private void BuildNextCircle(LinkedListNode<int>[] valuesToMove, int destinationValue)
        {
            var destinationNode = _AllCups[destinationValue];

            for (int i = 0; i < valuesToMove.Length; i++)
            {
                _Circle.AddAfter(destinationNode, valuesToMove[i]);
                destinationNode = valuesToMove[i];
            }
        }
        #endregion BuildNextCircle

        #region GetDestination
        private int GetDestination(LinkedListNode<int>[] nodesToSkip, int size)
        {
            int destCup = WrapDestination(_CurrentCup.Value - 1, size);
            while (nodesToSkip.Any(node => node.Value == destCup))
            {
                destCup = WrapDestination(destCup - 1, size);
            }
            Destination = destCup;
            return destCup;
        }
        #endregion GetDestination

        #region GetValuesToSkip
        private LinkedListNode<int>[] GetValuesToSkip()
        {
            var valuesToMove = new LinkedListNode<int>[3];
            for (int i = 0; i < valuesToMove.Length; i++)
            {
                valuesToMove[i] = _CurrentCup.Next ?? _Circle.First;
                _Circle.Remove(valuesToMove[i]);
            }
            return valuesToMove;
        }
        #endregion GetValuesToSkip

        #region Move
        public void Move()
        {
            var valuesToSkip = GetValuesToSkip();
            var destCup = GetDestination(valuesToSkip, _TotalItems);
            BuildNextCircle(valuesToSkip, destCup);
            _CurrentCup = _CurrentCup.Next ?? _Circle.First;
        }
        #endregion Move

        #region WrapDestination
        private int WrapDestination(int destination, int size)
        {
            return (destination == 0) ? size : destination;
        }
        #endregion WrapDestination

        #endregion Methods...
    }

    public class CircleList<T>
    {
        #region Properties...
        public CircleListNode<T> First { get; private set; }

        #region Last
        public CircleListNode<T> Last
        {
            get
            {
                var prev = First;
                var next = First.Next;
                while (next != First)
                {
                    prev = next;
                    next = next.Next;
                }

                return prev;
            }
        }
        #endregion Last

        #endregion Properties...

        #region Constructors...

        public CircleList(IEnumerable<T> items) 
        {
            CircleListNode<T> previous = null;
            foreach (var item in items)
            {
                var node = new CircleListNode<T>(item);
                if (First == null)
                {
                    First = node;
                    node.Next = First;
                }
                else
                {
                    AddAfter(previous, node);
                }
                previous = node;
            }
        }

        #endregion Constructors...

        #region AddAfter
        public CircleListNode<T> AddAfter(CircleListNode<T> node, CircleListNode<T> nextNode)
        {
            if (node == null) throw new NullReferenceException(nameof(node));
            if (nextNode == null) throw new NullReferenceException(nameof(nextNode));

            nextNode.Next = node.Next;
            node.Next = nextNode;
            return nextNode;
        }
        #endregion AddAfter

        #region AddAfter
        public CircleListNode<T> AddAfter(CircleListNode<T> node, T value)
        {
            return AddAfter(node, new CircleListNode<T>(value));
        }
        #endregion AddAfter

        #region Find
        public CircleListNode<T> Find(T value)
        {
            if (First.Value.Equals(value)) return First;

            var next = First.Next;
            while (next != First)
            {
                if (next.Value.Equals(value)) return next;
                next = next.Next;
            }

            return null;
        }
        #endregion Find

        #region RemoveNext
        public CircleListNode<T> RemoveNext(CircleListNode<T> node)
        {
            var nodeToRemove = node.Next;
            node.Next = nodeToRemove.Next;
            return nodeToRemove;
        }
        #endregion RemoveNext
    }

    public class CircleListNode<T>
    {
        public CircleListNode<T> Next { get; internal set; }
        public T Value { get; set; }

        internal CircleListNode(T value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
