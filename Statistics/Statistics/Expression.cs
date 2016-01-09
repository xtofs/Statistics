﻿using System;

using Sprache;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Xof
{
    public static class Expression
    {
        public static IExpression Parse(string expression) { return ExpressionParser.Instance.Parse(expression); }

        public static IExpression Var(String name) { return new VariableExpression(name); }
        public static IExpression Literal(Double value) { return new LiteralExpression(value); }
        public static IExpression Unary(String symbol, IExpression expression) { return new UnaryExpression(symbol, expression); }
        public static IExpression Binary(string arg1, IExpression arg2, IExpression arg3) { return new BinaryExpression(arg1, arg2, arg3); }

        public static T Process<T>(this IExpression expression, IExpressionProcessor<T> processor)
        {
            return expression.Visit(new ProcessingVisitor<T>(processor));
        }

        private class ProcessingVisitor<T> : IVisitor<T>
        {
            public IExpressionProcessor<T> Processor { get; }

            public ProcessingVisitor(IExpressionProcessor<T> processor)
            {
                Processor = processor;
            }

            public T Accept(VariableExpression var)
            {
                return Processor.Process(var.Name);
            }

            public T Accept(UnaryExpression unary)
            {
                return Processor.Process(unary.Operator, unary.Expression.Visit(this));
            }

            public T Accept(BinaryExpression binary)
            {
                return Processor.Process(binary.Operator, binary.Left.Visit(this), binary.Right.Visit(this));
            }

            public T Accept(LiteralExpression literal)
            {
                return Processor.Process(literal.Value);
            }
        }

        public static String Show(this IExpression expression)
        {
            return expression.Process(new Stringifier());
        }

        private class Stringifier : IExpressionProcessor<String>
        {
            public string Process(string var) { return string.Format("{0}", var); }
            public string Process(double value) { return string.Format("{0}", value); }
            public string Process(string op, string expression) { return string.Format("({0} {1}", op, expression); }
            public string Process(string op, string left, string right) { return string.Format("({0} {1} {2}", op, left, right); }
        }

        public static Double Evaluate(this IExpression expression, IDictionary<string, Double> bindings)
        {
            return expression.Process(new Evaluator(bindings));
        }

        private class Evaluator : IExpressionProcessor<Double>
        {
            public Evaluator(IDictionary<string, Double> bindings)
            {
                Bindings = bindings;
            }

            private IDictionary<string, Double> Bindings { get; }

            public Double Process(string var) { return Bindings[var]; }
            public Double Process(double value) { return value; }
            public Double Process(string op, Double expression) { var f = UnaryOps[op]; return f(expression); }
            public Double Process(string op, Double left, Double right) { var f = BinaryOps[op]; return f(left, right); }

            private static IDictionary<string, Func<Double, Double>> UnaryOps =
                new Dictionary<string, Func<Double, Double>> {
                    { "-", v => - v }
                };

            private static IDictionary<string, Func<Double, Double, Double>> BinaryOps =
                new Dictionary<string, Func<Double, Double, Double>>
            {
                { "+", (a, b) => a + b },
                { "-", (a, b) => a - b },
                { "*", (a, b) => a * b },
                { "/", (a, b) => a / b }
            };
        }

        public static JToken ToJson(this IExpression expression)
        {
            return expression.Process(new Jsonifier());
        }

        private class Jsonifier : IExpressionProcessor<JToken>
        {
            private static string Tag = "tag";  

            public JToken Process(double value)
            {
                return new JObject(
                    new JProperty(Tag, "literal"),
                    new JProperty("value", value));
            }

            public JToken Process(string name)
            {
                return new JObject(
                    new JProperty(Tag, "var"),
                    new JProperty("name", name));
            }

            public JToken Process(string op, JToken expression)
            {
                return new JObject(
                    new JProperty(Tag, "unary"),
                    new JProperty("op", op),
                    new JProperty("expr", expression));
            }

            public JToken Process(string op, JToken left, JToken right)
            {
                return new JObject(
                  new JProperty(Tag, "binary"),
                  new JProperty("op", op),
                  new JProperty("left", left),
                  new JProperty("right", right));
            }
        }
    }
}
