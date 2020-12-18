using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2020.Day18
{
    public enum TokenType
    {
        OpeningBracket, ClosingBracket, Add, Multiply, Number
    }

    public struct Token
    {
        public int Value;
        public TokenType Type;
        public Token(TokenType type)
        {
            Type = type;
            Value = 0;
        }
        public Token(int value)
        {
            Type = TokenType.Number;
            Value = value;
        }
    }

    [TestClass]
    public class Day18
    {
        #region Input

        IEnumerable<string> GetInput() => Input.GetLines(Properties.Resources.Day18Input);

        #endregion Input

        #region Code...

        #region ContinueUnwindingStack
        private static bool ContinueUnwindingStack(Token next, Token topOfStack, bool isAdvancedMath)
        {
            if (isAdvancedMath)
            {
                if (next.Type == TokenType.Add) 
                {
                    return topOfStack.Type == TokenType.Add;
                }
                else // if (next.Type == TokenType.Multiply)
                {
                    return topOfStack.Type == TokenType.Add || topOfStack.Type == TokenType.Multiply;
                }
            }
            else
            {
                return topOfStack.Type == TokenType.Add || topOfStack.Type == TokenType.Multiply;
            }
        }
        #endregion ContinueUnwindingStack

        #region ConvertToPostfix
        private static List<Token> ConvertToPostfix(Queue<Token> tokens, bool isAdvancedMath)
        {
            tokens.Enqueue(new Token(TokenType.ClosingBracket));

            Stack<Token> workingStack = new Stack<Token>();
            workingStack.Push(new Token(TokenType.OpeningBracket));

            Token element;

            List<Token> result = new List<Token>();

            while (tokens.Count > 0)
            {
                var next = tokens.Dequeue();
                if (next.Type == TokenType.Number)
                {
                    result.Add(next);
                }
                else if (next.Type == TokenType.OpeningBracket)
                {
                    workingStack.Push(next);
                }
                else if (next.Type == TokenType.ClosingBracket)
                {
                    element = workingStack.Pop();
                    while (element.Type != TokenType.OpeningBracket)
                    {
                        result.Add(element);
                        element = workingStack.Pop();
                    }
                }
                else
                {
                    while (ContinueUnwindingStack(next, workingStack.Peek(), isAdvancedMath))
                    {
                        element = workingStack.Pop();
                        result.Add(element);
                    }
                    workingStack.Push(next);
                }
            }

            while (workingStack.Count > 0)
            {
                element = workingStack.Pop();
                if (element.Type != TokenType.OpeningBracket) result.Add(element);
            }

            return result;
        }
        #endregion ConvertToPostfix

        #region Evaluate
        public long Evaluate(string line, bool isAdvancedMath = false)
        {
            Queue<Token> tokens = ParseTokens(line);
            List<Token> postfix = ConvertToPostfix(tokens, isAdvancedMath);
            Stack<long> stack = new Stack<long>();
            
            foreach (var token in postfix)
            {
                if (token.Type == TokenType.Number)
                {
                    stack.Push(token.Value);
                }
                else
                {
                    var operand1 = stack.Pop();
                    var operand2 = stack.Pop();
                    if (token.Type == TokenType.Add)
                    {
                        stack.Push(operand1 + operand2);
                    }
                    else if (token.Type == TokenType.Multiply)
                    {
                        stack.Push(operand1 * operand2);
                    }
                }
            }
            return stack.Peek();
        }
        #endregion Evaluate

        #region ParseTokens
        private static Queue<Token> ParseTokens(string line)
        {
            StringBuilder numberBuilder = new StringBuilder();
            Queue<Token> tokens = new Queue<Token>();

            foreach (var c in line)
            {
                if (char.IsDigit(c))
                {
                    numberBuilder.Append(c);
                }
                else
                {
                    if (numberBuilder.Length > 0)
                    {
                        var value = int.Parse(numberBuilder.ToString());
                        numberBuilder.Clear();
                        tokens.Enqueue(new Token(value));
                    }
                    if (c == '(') tokens.Enqueue(new Token(TokenType.OpeningBracket));
                    else if (c == ')') tokens.Enqueue(new Token(TokenType.ClosingBracket));
                    else if (c == '+') tokens.Enqueue(new Token(TokenType.Add));
                    else if (c == '*') tokens.Enqueue(new Token(TokenType.Multiply));
                }
            }
            if (numberBuilder.Length > 0)
            {
                var value = int.Parse(numberBuilder.ToString());
                numberBuilder.Clear();
                tokens.Enqueue(new Token(value));
            }

            return tokens;
        }
        #endregion ParseTokens

        #region Sum
        public long Sum(IEnumerable<string> equations, bool isAdvancedMath = false)
        {
            return equations.Sum(eq => Evaluate(eq, isAdvancedMath));
        }
        #endregion Sum

        #endregion Code...

        #region Tests...

        [TestMethod] public void EvaluateSimple() => Assert.AreEqual(71, Evaluate("1 + 2 * 3 + 4 * 5 + 6"));
        [TestMethod] public void EvaluateParenthesis() => Assert.AreEqual(51, Evaluate("1 + (2 * 3) + (4 * (5 + 6))"));
        [TestMethod] public void EvaluateParenthesisWith3Ops() => Assert.AreEqual(13632, Evaluate("((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2"));

        [TestMethod] public void Answer1() => Assert.AreEqual(4940631886147, Sum(GetInput()));

        [TestMethod] public void EvaluateSimpleWithAdvancedMath() => Assert.AreEqual(231, Evaluate("1 + 2 * 3 + 4 * 5 + 6", true));
        [TestMethod] public void EvaluateParenthesisWithAdvancedMath() => Assert.AreEqual(51, Evaluate("1 + (2 * 3) + (4 * (5 + 6))"));
        [TestMethod] public void EvaluateParenthesisWith3OpsAndAdvancedMath() => Assert.AreEqual(23340, Evaluate("((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2", true));
        [TestMethod] public void Answer2() => Assert.AreEqual(283582817678281, Sum(GetInput(), true));
        #endregion Tests...
    }
}
