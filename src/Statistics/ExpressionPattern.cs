﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xof
{        
    public interface IExpressionPattern
    {
        // QUESTION: ideally this would be IImutableDictionary because it simplifies the code 
        // for the recursive construction of the dictionary in Match
        // OK to introduce the dependency to System.Collections.Immutable  ?
        IDictionary<string, IExpression> Match(IExpression expression, IDictionary<String, IExpression> binding);
    }

    public static class ExpressionPattern
    {
        public static IExpressionPattern Term(String symbol, params IExpressionPattern[] args) { return new TermExpressionPattern(symbol, args); }

        public static IExpressionPattern Var(String name, ExpressionKind kind) { return new VariableExpressionPattern(name, kind); }

        public static IExpressionPattern Var(String name) { return new VariableExpressionPattern(name, null); }

        public static IDictionary<String, IExpression> Match(this IExpressionPattern pattern, IExpression expression)
        {
            return pattern.Match(expression, new Dictionary<String, IExpression>());
        }

        public static IDictionary<String, IExpression> TryMatch(this IExpressionPattern pattern, IExpression expression)
        {
            try
            {
                return pattern.Match(expression, new Dictionary<String, IExpression>());
            }
            catch
            {
                return null;
            }
        }
    }

    public class NoMatchException : Exception
    {
        public NoMatchException(String message) : base(message)
        {
        }
    }

    public class TermExpressionPattern : IExpressionPattern
    {
        public TermExpressionPattern(String symbol, IList<IExpressionPattern> args)
        {
            Symbol = symbol;
            Args = args;
        }

        public IList<IExpressionPattern> Args { get; }
        public String Symbol { get; }

        public IDictionary<string, IExpression> Match(IExpression expression, IDictionary<string, IExpression> binding)
        {
            return expression.Visit(new Matcher(this, binding));
        }

        private class Matcher : IVisitor<IDictionary<string, IExpression>>
        {
            private IDictionary<string, IExpression> binding;
            private TermExpressionPattern pattern;

            public Matcher(TermExpressionPattern termExpressionPattern, IDictionary<string, IExpression> binding)
            {
                this.pattern = termExpressionPattern;
                this.binding = binding;
            }

            public IDictionary<string, IExpression> Accept(CallExpression call)
            {
                if (pattern.Args.Count == call.Arguments.Count &&
                    (string.IsNullOrWhiteSpace(pattern.Symbol) || pattern.Symbol.Equals(call.Function)))
                {
                    var current = binding;
                    foreach (var pair in pattern.Args.Zip(call.Arguments, Tuple.Create))
                    {
                        current = pair.Item1.Match(pair.Item2, current);
                    }
                    return current;
                }
                throw new NoMatchException(string.Empty);
            }

            public IDictionary<string, IExpression> Accept(BinaryExpression binary)
            {
                if (pattern.Args.Count == 2 && (string.IsNullOrWhiteSpace(pattern.Symbol) || pattern.Symbol.Equals(binary.Operator)))
                {
                    var a = pattern.Args[0].Match(binary.Left, binding);
                    var b = pattern.Args[1].Match(binary.Right, a);
                    return b;
                }
                throw new NoMatchException(string.Empty);
            }

            public IDictionary<string, IExpression> Accept(LiteralExpression literal)
            {
                throw new NotImplementedException();
            }

            public IDictionary<string, IExpression> Accept(UnaryExpression unary)
            {
                var a = pattern.Args[0].Match(unary.Operand, binding);
                return a;
            }

            public IDictionary<string, IExpression> Accept(VariableExpression var)
            {
                throw new NotImplementedException();
            }
        }
    }

    public class VariableExpressionPattern : IExpressionPattern
    {
        public VariableExpressionPattern(String name, Nullable<ExpressionKind> kind)
        {
            Name = name;
            Kind = kind;
        }

        public string Name { get; }

        public Nullable<ExpressionKind> Kind { get; }

        public IDictionary<string, IExpression> Match(IExpression expression, IDictionary<string, IExpression> binding)
        {
            if (binding.ContainsKey(Name))
            {
                if (binding[Name].Equals(expression))
                {
                    return binding;
                }
                else
                {
                    throw new NoMatchException(String.Format("variable {0} already bound to {1} != {2}", Name, binding[Name], expression));
                }
            }
            else if (Kind == null || Kind.Equals(expression.Kind()))
            {
                binding[Name] = expression;
                return binding;
            }
            else
            {
                throw new NoMatchException(String.Format("unmatching pattern variable kind {0} != {1}", expression.Kind(), Kind));
            }
        }
    }
}
