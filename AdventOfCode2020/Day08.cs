using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2020.Day08
{
    [TestClass]
    public class Day08
    {
        #region Input

        IEnumerable<string> GetInput() => Input.GetLines(Properties.Resources.Day08Input);

        IEnumerable<string> GetTestInput() => new string[]
        {
            "nop +0",
            "acc +1",
            "jmp +4",
            "acc +3",
            "jmp -3",
            "acc -99",
            "acc +1",
            "jmp -4",
            "acc +6",
        };

        #endregion Input

        #region Code...

        #region Run
        private long Run(IEnumerable<string> lines)
        {
            var computer = Computer.Parse(lines);
            computer.RunProgram(haltIfRepeating: true);
            return computer.Accumulator;
        }
        #endregion Run

        #region FindBug
        private long FindBug(IEnumerable<string> lines)
        {
            var instructions = lines.Select(line => Instruction.Parse(line)).ToArray();
            bool found = false;
            long answer = 0;
            int indexToChange = 0;
            while (!found && indexToChange < instructions.Length)
            {
                Instruction instruction = instructions[indexToChange];
                indexToChange++;

                if (instruction.Operation == Operation.acc) continue;

                var oldOperation = instruction.Operation;
                instruction.Operation = (oldOperation == Operation.jmp) ? Operation.nop : Operation.jmp;

                var computer = new Computer(instructions);
                computer.RunProgram(haltIfRepeating: true);

                if (!computer.HasInfiniteLoop)
                {
                    found = true;
                    answer = computer.Accumulator;
                }
                instruction.Operation = oldOperation;
            }
            return answer;
        }
        #endregion FindBug

        #endregion Code...

        #region Tests...

        [TestMethod] public void Test1() => Assert.AreEqual(5, Run(GetTestInput()));

        [TestMethod] public void Answer1() => Assert.AreEqual(2058, Run(GetInput()));

        [TestMethod] public void Test2() => Assert.AreEqual(8, FindBug(GetTestInput()));

        [TestMethod] public void Answer2() => Assert.AreEqual(1000, FindBug(GetInput()));
        #endregion Tests...
    }

    public class Computer
    {
        #region Member Variables...
        private bool _HaltProgram = false;
        private int _InstructionPointer = 0;
        private List<Instruction> _Instructions = new List<Instruction>();
        #endregion Member Variables...

        #region Properties...

        public long Accumulator { get; private set; } = 0;

        public bool HasInfiniteLoop => _HaltProgram;

        #endregion Properties...

        #region Constructors...
        public Computer(IEnumerable<Instruction> instructions)
        {
            _Instructions.AddRange(instructions);
            foreach (var item in _Instructions)
            {
                item.TimesRan = 0;
            }
        }
        #endregion Constructors...

        #region Methods...

        #region Parse
        public static Computer Parse(IEnumerable<string> lines)
        {
            var instructions = lines.Select(line => Instruction.Parse(line));
            return new Computer(instructions);
        }
        #endregion Parse

        #region RunProgram
        public void RunProgram(bool haltIfRepeating)
        {
            while (!_HaltProgram && _InstructionPointer < _Instructions.Count)
            {
                RunNextInstruction(haltIfRepeating);
            }
        }
        #endregion RunProgram

        #region RunNextInstruction
        public void RunNextInstruction(bool haltIfRepeating = false)
        {
            if (_InstructionPointer >= _Instructions.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(_InstructionPointer), _InstructionPointer, $"Length of list is {_Instructions.Count}");
            }
            var currentInstruction = _Instructions[_InstructionPointer];
            if (haltIfRepeating && currentInstruction.TimesRan > 0)
            {
                _HaltProgram = true;
            }

            if (!_HaltProgram)
            {
                currentInstruction.TimesRan++;
                switch (currentInstruction.Operation)
                {
                    case Operation.nop:
                        _InstructionPointer++;
                        break;

                    case Operation.acc:
                        Accumulator += currentInstruction.Argument;
                        _InstructionPointer++;
                        break;

                    case Operation.jmp:
                        _InstructionPointer += currentInstruction.Argument;
                        break;
                }
            }
        }
        #endregion RunNextInstruction

        #endregion Methods...
    }

    public enum Operation
    {
        nop, acc, jmp
    }

    public class Instruction
    {
        #region Properties...

        public int Argument { get; set; }

        public Operation Operation { get; set; }

        public int TimesRan { get; internal set; } = 0;

        #endregion Properties...

        #region Methods...

        #region Parse
        internal static Instruction Parse(string line)
        {
            Match match = Regex.Match(line, @"(nop|acc|jmp) ([\+\-]\d+)");
            if (!match.Success)
            {
                throw new Exception($"Line '{line}' does not match the expected pattern");
            }
            return new Instruction()
            {
                Operation = (Operation)Enum.Parse(typeof(Operation), match.Groups[1].Value),
                Argument = int.Parse(match.Groups[2].Value)
            };
        }
        #endregion Parse

        #region ToString
        public override string ToString()
        {
            return $"{Operation} {Argument}";
        }
        #endregion ToString

        #endregion Methods...
    }
}
