using System;
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

        public static IExpressionPattern Var(String name, String kind) { return new VariableExpressionPattern(name, kind); }

        public static IDictionary<String, IExpression> Match(this IExpressionPattern pattern, IExpression expression)
        {
            return pattern.Match(expression, new Dictionary<String, IExpression>());
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
            var binary = expression as BinaryExpression;
            if (binary != null && Args.Count == 2 && binary.Operator.Equals(Symbol))
            {
                var a = Args[0].Match(binary.Left, binding);
                var b = Args[1].Match(binary.Right, a);
                return b;
            }

            throw new NotImplementedException("only binary expressions implemented yet.");
            // throw new NoMatchException();
        }
    }

    public class VariableExpressionPattern : IExpressionPattern
    {
        public VariableExpressionPattern(String name, String kind)
        {
            Name = name;
            Kind = kind;
        }

        public string Kind { get; }
        public string Name { get; }

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
            else
            {
                binding[Name] = expression;
                return binding;
            }
        }
    }
}
