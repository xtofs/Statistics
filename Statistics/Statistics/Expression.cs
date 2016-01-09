using System;

using Sprache;

namespace Xof
{
    public static class Expression
    {
        public static IExpression Parse(string expression) { return ExpressionParser.Instance.Parse(expression); }

        public static IExpression Var(String name) { return new VariableExpression(name); }
        public static IExpression Literal(Double value) { return new LiteralExpression(value); }
        public static IExpression Unary(String symbol, IExpression expression) { return new UnaryExpression(symbol, expression); }
        public static IExpression Binary(string arg1, IExpression arg2, IExpression arg3) { return new BinaryExpression(arg1, arg2, arg3); }

        public static String Show(this IExpression expression)
        {
            return expression.Visit(new Stringify());
        }

        private class Stringify : IVisitor<String>
        {
            public string Accept(BinaryExpression binary) { return string.Format("({0} {1} {2})", binary.Left.Show(), binary.Operator, binary.Right.Show()); }
            public string Accept(LiteralExpression literal) { return literal.Value.ToString(); }
            public string Accept(UnaryExpression unary) { return string.Format("({0} {1}", unary.Operator, unary.Expression.Show()); }
            public string Accept(VariableExpression var) { return var.Name.ToString(); }
        }

        //public static Double Evaluate(this IExpression expression, Dictionary<string, Double> bindings)
        //{
        //    return expression.Visit(new Evaluator());
        //}

    }
}
