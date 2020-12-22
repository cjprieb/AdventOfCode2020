using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Day22
{
    [TestClass]
    public class Day22
    {
        #region Input

        public Game GetGame() => new Game()
        {
            PlayerOneDeck = new Queue<int>(new int[] { 3,42,4,25,14,36,32,18,33,10,35,50,16,31,34,46,9,6,41,7,15,45,30,27,49 }),
            PlayerTwoDeck = new Queue<int>(new int[] { 8,11,47,21,17,39,29,43,23,28,13,22,5,20,44,38,26,37,2,24,48,12,19,1,40 })
        };

        public Game GetTestGame() => new Game()
        {
            PlayerOneDeck = new Queue<int>(new int[] { 9,2,6,3,1 }),
            PlayerTwoDeck = new Queue<int>(new int[]{ 5,8,4,7,10 })
        };

        public Game GetTestGame2() => new Game()
        {
            PlayerOneDeck = new Queue<int>(new int[] { 43, 19 }),
            PlayerTwoDeck = new Queue<int>(new int[] { 2, 29, 14 })
        };

        #endregion Input

        #region Code...

        #region GetDeckAfterRound
        public string GetDeckAfterRound(Game game, int rounds, int player)
        {
            for (int i = 0; i < rounds; i++)
            {
                game.PlayRound();
            }
            IEnumerable<int> deck = null;
            if (player == 1) deck = game.PlayerOneDeck;
            else if (player == 2) deck = game.PlayerTwoDeck;

            return (deck != null) ? string.Join(", ", deck) : null;
        }
        #endregion GetDeckAfterRound

        #region GetWinner
        public int GetWinner(Game game)
        {
            while (game.Winner == null)
            {
                game.PlayRound();
            }
            return game.Score;
        }
        #endregion GetWinner

        #region GetRecursiveWinner
        public int GetRecursiveWinner(Game game)
        {
            while (game.Winner == null)
            {
                game.PlayRecursiveRound();
            }
            return game.Score;
        }
        #endregion GetRecursiveWinner

        #endregion Code...

        #region Tests...
        [TestMethod] public void Round1_PlayerOne() => Assert.AreEqual("2, 6, 3, 1, 9, 5", GetDeckAfterRound(GetTestGame(), 1, 1));
        [TestMethod] public void Round1_PlayerTwo() => Assert.AreEqual("8, 4, 7, 10", GetDeckAfterRound(GetTestGame(), 1, 2));
        [TestMethod] public void Round2_PlayerOne() => Assert.AreEqual("6, 3, 1, 9, 5", GetDeckAfterRound(GetTestGame(), 2, 1));
        [TestMethod] public void Round2_PlayerTwo() => Assert.AreEqual("4, 7, 10, 8, 2", GetDeckAfterRound(GetTestGame(), 2, 2));

        [TestMethod] public void Test1() => Assert.AreEqual(306, GetWinner(GetTestGame()));
        [TestMethod] public void Answer1() => Assert.AreEqual(32272, GetWinner(GetGame()));

        [TestMethod] public void Test2() => Assert.AreEqual(291, GetRecursiveWinner(GetTestGame()));
        [TestMethod] public void Answer2() => Assert.AreEqual(33206, GetRecursiveWinner(GetGame()));
        #endregion Tests...
    }

    public class Game
    {
        #region Member Variables...
        private const int None = 0;
        private const int PlayerOne = 1;
        private const int PlayerTwo = 2;

        private HashSet<string> _DeckArrangements = new HashSet<string>();
        private bool _HadCardsPreviouslyPlayed = false;
        private int _RecursiveLevel = 0;
        #endregion Member Variables...

        #region Constructors...
        public Game() 
        {
        }

        public Game(int level) : base()
        {
            _RecursiveLevel = level;
        }
        #endregion Constructors...

        #region Properties...

        #region PlayerOneDeck
        public Queue<int> PlayerOneDeck { get; set; } = new Queue<int>();
        #endregion PlayerOneDeck

        #region PlayerTwoDeck
        public Queue<int> PlayerTwoDeck { get; set; } = new Queue<int>();
        #endregion PlayerTwoDeck

        #region Score
        public int Score
        {
            get
            {
                var sum = 0;
                var i = Winner.Count();
                foreach (var next in Winner)
                {
                    sum += next * i;
                    i--;
                }
                return sum;
            }
        }
        #endregion Score

        #region Winner
        public IEnumerable<int> Winner
        {
            get
            {
                if (WinningPlayer == PlayerOne) return PlayerOneDeck;
                else if (WinningPlayer == PlayerTwo) return PlayerTwoDeck;
                return null;
            }
        }
        #endregion Winner

        #region WinningPlayer
        public int WinningPlayer
        {
            get
            {
                if (_HadCardsPreviouslyPlayed) return PlayerOne;
                else if (PlayerOneDeck.Count == 0) return PlayerTwo;
                else if (PlayerTwoDeck.Count == 0) return PlayerOne;
                return None;
            }
        }
        #endregion WinningPlayer

        #endregion Properties...

        #region Methods...

        #region PlayRound
        public void PlayRound()
        {
            var card1 = PlayerOneDeck.Dequeue();
            var card2 = PlayerTwoDeck.Dequeue();

            if (card1 > card2)
            {
                PlayerOneDeck.Enqueue(card1);
                PlayerOneDeck.Enqueue(card2);
            }
            else if (card1 < card2)
            {
                PlayerTwoDeck.Enqueue(card2);
                PlayerTwoDeck.Enqueue(card1);
            }
            else
            {
                throw new NotSupportedException($"Game does not support ties: {card1}");
            }
        }
        #endregion PlayRound

        #region PlayRecursiveRound
        public void PlayRecursiveRound()
        {
            var deckStatus = string.Join(",",PlayerOneDeck) + "|" + string.Join(",", PlayerTwoDeck);
            _HadCardsPreviouslyPlayed = _DeckArrangements.Contains(deckStatus);
            _DeckArrangements.Add(deckStatus);

            if (_HadCardsPreviouslyPlayed) return;

            var card1 = PlayerOneDeck.Dequeue();
            var card2 = PlayerTwoDeck.Dequeue();

            int winner;
            if (card1 <= PlayerOneDeck.Count && card2 <= PlayerTwoDeck.Count)
            {
                winner = GetWinnerOfSubGame(card1, card2);
            }
            else if (card1 > card2)
            {
                winner = PlayerOne;
            }
            else if (card1 < card2)
            {
                winner = PlayerTwo;
            }
            else
            {
                throw new NotSupportedException($"Game does not support ties: {card1}");
            }

            if (winner == PlayerOne)
            {
                PlayerOneDeck.Enqueue(card1);
                PlayerOneDeck.Enqueue(card2);
            }
            else if (winner == PlayerTwo)
            {
                PlayerTwoDeck.Enqueue(card2);
                PlayerTwoDeck.Enqueue(card1);
            }
        }
        #endregion

        #region GetWinnerOfSubGame
        private int GetWinnerOfSubGame(int playerOneCard, int playerTwoCard)
        {
            var subGame = new Game(_RecursiveLevel+1)
            {
                PlayerOneDeck = new Queue<int>(PlayerOneDeck.Take(playerOneCard)),
                PlayerTwoDeck = new Queue<int>(PlayerTwoDeck.Take(playerTwoCard)),
            };
            while (subGame.WinningPlayer == None)
            {
                subGame.PlayRecursiveRound();
            }
            return subGame.WinningPlayer;
        }
        #endregion GetWinnerOfSubGame

        #endregion Methods...
    }
}
