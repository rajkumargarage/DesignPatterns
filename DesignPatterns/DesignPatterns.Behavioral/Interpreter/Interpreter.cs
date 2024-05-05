using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DesignPatterns.Behavioral.InterpreterPattern
{
    public class Interpreter
    {
        public static void Invoke()
        {
            var parser = new Parser();
            var value = parser.Parse("(10+2)-(22-99)");
            Console.WriteLine(value.Value);
        }
    }

    class Token
    {
        internal enum Type
        {
            Integer, Plus, Minus, LParen, RParen
        }

        internal Type TokenType;

        internal string Text { get; }

        public Token(Type tokenType, string text)
        {
            TokenType = tokenType;
            this.Text = text ?? throw new ArgumentNullException(nameof(text));
        }

        public override string ToString()
        {
            return $"`{Text}`";
        }
    }

    class Parser
    {
        List<Token> GetTokens(string expression)
        {
            var tokens = new List<Token>();


            for (int i = 0; i < expression.Length; i++)
            {
                var token = expression[i];
                switch (token)
                {
                    case '+':
                        tokens.Add(new Token(Token.Type.Plus, token.ToString()));
                        break;
                    case '-':
                        tokens.Add(new Token(Token.Type.Minus, token.ToString()));
                        break;
                    case '(':
                        tokens.Add(new Token(Token.Type.LParen, token.ToString()));
                        break;
                    case ')':
                        tokens.Add(new Token(Token.Type.RParen, token.ToString()));
                        break;
                    default:
                        if (!char.IsNumber(expression[i]))
                            break;
                        int j = i + 1;
                        var sb = new StringBuilder(expression[i].ToString());
                        for (; j < expression.Length; j++)
                        {
                            if (char.IsNumber(expression[j]))
                            {
                                sb.Append(expression[j]);
                                ++i;
                            }
                            else
                            {
                                tokens.Add(new Token(Token.Type.Integer, sb.ToString()));
                                break;
                            }
                        }
                        break;
                }
            }

            return tokens;
        }

        IElement Parse(IList<Token> tokens)
        {
            var result = new BinaryElement();

            for (int i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];
                switch (token.TokenType)
                {
                    case Token.Type.Integer:
                        var integer = new Element(int.Parse(token.Text));
                        if (result.Left == null)
                        {
                            result.Left = integer;
                        }
                        else
                        {
                            result.Right = integer;
                        }
                        break;
                    case Token.Type.Plus:
                        result.MyType = BinaryElement.Type.Addition;
                        break;
                    case Token.Type.Minus:
                        result.MyType = BinaryElement.Type.Subtraction;
                        break;
                    case Token.Type.LParen:
                        int j = i;
                        for (; j < tokens.Count; j++)
                        {
                            if (tokens[j].TokenType == Token.Type.RParen)
                                break;
                        }
                        var subExpression = tokens.Skip(i + 1).Take(j - i - 1).ToList();
                        i = j;
                        var element = Parse(subExpression);
                        if (result.Left == null)
                        {
                            result.Left = element;
                        }
                        else
                        {
                            result.Right = element;
                        }
                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        internal IElement Parse(string v)
        {
            return Parse(GetTokens(v));
        }
    }

    interface IElement
    {
        int Value { get; }
    }

    class Element : IElement
    {
        public int Value { get; }

        public Element(int value)
        {
            this.Value = value;
        }
    }

    class BinaryElement : IElement
    {
        internal enum Type
        {
            Addition, Subtraction
        }

        internal Type MyType;
        public int Value
        {
            get
            {
                switch (MyType)
                {
                    case Type.Addition:
                        return Left.Value + Right.Value;
                    case Type.Subtraction:
                        return Left.Value - Right.Value;
                    default:
                        return 0;
                }
            }
        }

        internal IElement Left;
        internal IElement Right;

        internal BinaryElement(Type type, IElement left, IElement right)
        {
            this.MyType = type;
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        public BinaryElement()
        {
        }
    }
}
