using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace AdventOfCode2020.Day25
{
    [TestClass]
    public class Day25
    {
        #region Code...

        #region BreakEncryption
        public long BreakEncryption(long cardPublicKey, long doorPublicKey)
        {
            var loopSizes = FindLoopSizes(cardPublicKey, doorPublicKey);
            
            Assert.IsTrue(loopSizes.ContainsKey(0), "doesn't have card value");
            Assert.AreNotEqual(int.MaxValue, loopSizes[0], "Card Loop Value is max int value");
            var card = new Device(loopSizes[0]);

            Assert.IsTrue(loopSizes.ContainsKey(1), "doesn't have door value");
            Assert.AreNotEqual(int.MaxValue, loopSizes[1], "Door Loop Value is max int value");
            var door = new Device(loopSizes[1]);

            var cardEncryptionKey = card.Transform(doorPublicKey);
            var doorEncryptionKey = door.Transform(cardPublicKey);
            Assert.AreEqual(cardEncryptionKey, doorEncryptionKey);
            return doorEncryptionKey;
        }
        #endregion BreakEncryption

        #region FindLoopSizes
        public Dictionary<int, long> FindLoopSizes(params long[] publicKeys)
        {
            var results = new Dictionary<int, long>();
            var loopCount = 0L;
            var subject = 7L;
            var value = 1L;
            while (results.Count != publicKeys.Length && loopCount < int.MaxValue)
            {
                value = Device.TransformStep(value, subject);
                loopCount++;
                for (int i = 0; i < publicKeys.Length; i++)
                {
                    if (value == publicKeys[i]) results[i] = loopCount;
                }
            }
            return results;
        }
        #endregion FindLoopSizes

        #endregion Code...

        #region Tests...
        [TestMethod] public void Loop_5764801() => Assert.AreEqual(8, FindLoopSizes(5764801)[0]);
        [TestMethod] public void Loop_17807724() => Assert.AreEqual(11, FindLoopSizes(17807724)[0]);
        [TestMethod] public void Test1() => Assert.AreEqual(14897079, BreakEncryption(5764801, 17807724));
        [TestMethod] public void Answer1() => Assert.AreEqual(5414549, BreakEncryption(1614360, 7734663));

        [TestMethod] public void Answer2() { }
        #endregion Tests...
    }

    public class Device
    {
        #region Properties...
        public long LoopSize { get; set; }
        #endregion Properties...

        #region Constructors...
        public Device(long loopSize)
        {
            LoopSize = loopSize;
        }
        #endregion Constructors...

        #region Methods...

        #region ToString
        public override string ToString()
        {
            return $"Loop Size: {LoopSize}";
        }
        #endregion ToString

        #region Transform
        public long Transform(long subject)
        {
            var value = 1L;
            for (int i = 0; i < LoopSize; i++)
            {
                value = TransformStep(value, subject);
            }
            return value;
        }
        #endregion Transform

        #region TransformStep
        public static long TransformStep(long value, long subject) => (value * subject) % 20201227;
        #endregion TransformStep

        #endregion Methods...

    }
}
