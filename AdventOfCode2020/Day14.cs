using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace AdventOfCode2020.Day14
{
    [TestClass]
    public class Day14
    {
        #region Input
        #endregion Input

        #region Code...
        #endregion Code...

        #region Tests...

        [TestMethod]
        public void Test1()
        {
            var program = new BitProgram();
            program.Run(new string[]
            {
                "mask = XXXXXXXXXXXXXXXXXXXXXXXXXXXXX1XXXX0X",
                "mem[8] = 11",
                "mem[7] = 101",
                "mem[8] = 0"
            });
            Assert.AreEqual("165", program.Sum());
        }

        [TestMethod] public void Answer1()
        {
            var program = new BitProgram();
            program.Run(Input.GetLines(Properties.Resources.Day14Input));
            Assert.AreEqual("15514035145260", program.Sum());
        }

        [TestMethod]
        public void Test2()
        {
            var program = new BitProgram();
            program.Run(new string[]
            {
                "mask = 000000000000000000000000000000X1001X",
                "mem[42] = 100",
                "mask = 00000000000000000000000000000000X0XX",
                "mem[26] = 1"
            }, true);
            Assert.AreEqual(208L, program.SumVersion2());
        }

        /// <summary>
        /// 4091311717392 is too high
        /// 3926790061594
        /// </summary>
        [TestMethod]
        public void Answer2()
        {
            var program = new BitProgram();
            program.Run(Input.GetLines(Properties.Resources.Day14Input), true);
            Assert.AreEqual(4091311717392L, program.SumVersion2());
        }
        #endregion Tests...
    }

    public class BitProgram
    {
        private string _Mask = string.Empty;
        public Dictionary<int, bool[]> Memory { get; private set; } = new Dictionary<int, bool[]>();

        public Dictionary<long, long> Memory2 { get; private set; } = new Dictionary<long, long>();

        public BitProgram() { }

        #region GetPossibleAddresses
        /// <summary>
        /// 2^32 = 4294967296
        ///  int = 2147483647
        /// long = 9223372036854775807
        /// </summary>
        /// <param name="maskedAddress"></param>
        /// <returns></returns>
        private IEnumerable<long> GetPossibleAddresses(byte[] maskedAddress)
        {
            List<long> addresses = new List<long>();
            addresses.Add(0L);
            long multiplier = 1;
            for (int i = maskedAddress.Length - 1; i >= 0; i--)
            {
                if (maskedAddress[i] == 1)
                {
                    UpdateAddresses(addresses, multiplier, false);
                }
                else if (maskedAddress[i] == 2)
                {
                    UpdateAddresses(addresses, multiplier, true);
                }

                multiplier *= 2;
            }
            return addresses;
        }
        #endregion GetPossibleAddresses

        #region UpdateAddresses
        private void UpdateAddresses(List<long> addresses, long multiplier, bool isFloating)
        {
            int count = addresses.Count;
            for (int i = 0; i < count; i++)
            {
                var newValue = addresses[i] + multiplier;
                if (isFloating) addresses.Add(newValue);
                else addresses[i] = newValue;                
            }
        }
        #endregion UpdateAddresses

        #region SetMask
        public void SetMask(string mask, bool useVersion2 = false)
        {
            _Mask = mask;
        }
        #endregion SetMask

        #region Write
        public void Write(int address, long value, bool useVersion2 = false)
        {
            if (useVersion2)
            {
                var maskedAddress = ApplyMaskVersion2(address);
                foreach (var addr in GetPossibleAddresses(maskedAddress))
                {
                    Memory2[addr] = value;
                }
            }
            else
            {
                Memory[address] = ApplyMask(value);
            }
        }
        #endregion Write

        #region Run
        public void Run(IEnumerable<string> instructions, bool useVersion2 = false)
        {
            int max = 0;
            foreach (var line in instructions)
            {
                var tokens = line.Split('=');
                if (tokens.Length == 2)
                {
                    if (tokens[0].StartsWith("mask"))
                    {
                        int count = tokens[1].Count(c => c == 'X');
                        if (count > max) max = count;
                        SetMask(tokens[1].Trim(), useVersion2);
                    }
                    else // memory line
                    {
                        var length = tokens[0].Length - 6;
                        var address = int.Parse(tokens[0].Substring(4, length));
                        var value = long.Parse(tokens[1]);
                        Write(address, value, useVersion2);
                    }
                }
            }
            Console.WriteLine($"Max X's in a mask: {max}");
        }
        #endregion Run

        #region ApplyMask
        private bool[] ApplyMask(long value)
        {
            //return (value | OnesMask) & ZerosMask;
            string stringValue = Convert.ToString(value, toBase: 2);
            int lengthDifference = _Mask.Length - stringValue.Length;
            bool[] bytes = new bool[_Mask.Length];
            for (int i = _Mask.Length - 1; i >= 0; i--)
            {
                var c = _Mask[i];
                bytes[i] = false;

                var valueIndex = i - lengthDifference;
                if (c == 'X' && valueIndex >= 0 && valueIndex < stringValue.Length)
                {
                    bytes[i] = (stringValue[valueIndex] == '1');
                }
                else if (c == '1')
                {
                    bytes[i] = true;
                }
            }
            return bytes;
        }
        #endregion ApplyMask

        #region ApplyMaskVersion2
        private byte[] ApplyMaskVersion2(long value)
        {
            string stringValue = Convert.ToString(value, toBase: 2);
            int lengthDifference = _Mask.Length - stringValue.Length;
            byte[] bytes = new byte[_Mask.Length];
            for (int i = _Mask.Length - 1; i >= 0; i--)
            {
                var c = _Mask[i];
                bytes[i] = 0;

                var valueIndex = i - lengthDifference;
                if (c == 'X')
                {
                    bytes[i] = 2;
                }
                else if (c == '1')
                {
                    bytes[i] = 1;
                }
                else if (valueIndex >= 0 && valueIndex < stringValue.Length)
                {
                    bytes[i] = (byte)((c == '0') ? (stringValue[valueIndex] == '1' ? 1 : 0) : 1);
                }
            }
            return bytes;
        }
        #endregion ApplyMaskVersion2

        #region Sum
        public string Sum()
        {
            BigInteger sum = 0L;
            foreach (var value in Memory.Values)
            {
                sum += GetValueFromBitString(value);
                //sum += value;
            }
            return sum.ToString();
        }
        #endregion Sum

        #region SumVersion2
        public long SumVersion2()
        {
            long sum = 0L;
            foreach (var value in Memory2.Values)
            {
                sum += value;
            }
            return sum;
        }
        #endregion SumVersion2

        #region GetValueFromBitString
        private long GetValueFromBitString(bool[] bytes)
        {
            //BigInteger value = new BigInteger(0);
            long value = 0L;
            long multiplier = 1;
            for (int i = bytes.Length - 1; i >= 0; i--)
            {
                if (bytes[i])
                {
                    value += multiplier;
                }
                multiplier *= 2;
            }
            return value;
        }
        #endregion GetValueFromBitString

    }
}
