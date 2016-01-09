﻿using System;

using Sprache;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Xof
{
    public static class Expression
    {
        public static IExpression Parse(string expression) { return ExpressionParser.Expr.Parse(expression); }

        public static IExpression Var(String name) { return new Var(name); }
        public static IExpression Literal(Double value) { return new Literal(value); }
        public static IExpression Unary(String symbol, IExpression expression) { return new Unary(symbol, expression); }
        public static IExpression Binary(string arg1, IExpression arg2, IExpression arg3) { return new Binary(arg1, arg2, arg3); }

        public static String Show(this IExpression expression)
        {
            return expression.Visit(new Stringify());
        }

        private class Stringify : IVisitor<String>
        {
            public string Accept(Binary binary) { return string.Format("({0} {1} {2})", binary.Left.Show(), binary.Operator, binary.Right.Show()); }
            public string Accept(Literal literal) { return literal.Value.ToString(); }
            public string Accept(Unary unary) { return string.Format("({0} {1}", unary.Operator, unary.Expression.Show()); }
            public string Accept(Var var) { return var.Name.ToString(); }
        }

        public static Double Evaluate(this IExpression expression, IDictionary<string, Double> bindings)
        {
            return expression.Visit(new Evaluator(bindings));
        }

        class Evaluator : IVisitor<Double>
        {
            public Evaluator(IDictionary<string, Double> bindings)
            {
                Bindings = bindings;
            }

            IDictionary<string, Double> Bindings { get; }

            public double Accept(Binary binary)
            {
                switch (binary.Operator)
                {
                    case "*": return binary.Left.Visit(this) * binary.Right.Visit(this);
                    case "/": return binary.Left.Visit(this) / binary.Right.Visit(this);
                    case "+": return binary.Left.Visit(this) + binary.Right.Visit(this);
                    case "-": return binary.Left.Visit(this) - binary.Right.Visit(this);
                    default: throw new NotSupportedException();
                }
            }

            public double Accept(Literal literal)
            {
                return literal.Value;
            }

            public double Accept(Unary unary)
            {
                switch (unary.Operator)
                {
                    case "-": return -unary.Expression.Visit(this);
                    default: throw new NotSupportedException();
                }
            }

            public double Accept(Var var)
            {
                return Bindings[var.Name];
            }
        }


        public static JToken ToJson(this IExpression expression)
        {
            return expression.Visit(new Jsonifier());
        }

        private class Jsonifier : IVisitor<JToken>
        {
            private static string Tag = "tag";
            public JToken Accept(Binary binary)
            {
                return new JObject(
                    new JProperty(Tag, "binary"),
                    new JProperty("op", binary.Operator),
                    new JProperty("left", binary.Left.Visit(this)),
                    new JProperty("right", binary.Right.Visit(this)));
            }

            public JToken Accept(Literal literal)
            {
                return new JObject(
                    new JProperty(Tag, "literal"),
                    new JProperty("value", literal.Value));
            }

            public JToken Accept(Unary unary)
            {
                return new JObject(
                    new JProperty(Tag, "unary"),
                    new JProperty("op", unary.Operator),
                    new JProperty("expr", unary.Expression.Visit(this)));
            }

            public JToken Accept(Var var)
            {
                return new JObject(
                    new JProperty(Tag, "var"),
                    new JProperty("name", var.Name));
            }
        }
    }
}
