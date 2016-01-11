using System;
using System.Linq;

using Sprache;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Xof
{
    public static class Expression
    {
        public static IExpression Var(String name) { return new VariableExpression(name); }
        public static IExpression Literal(Double value) { return new LiteralExpression(value); }
        public static IExpression Unary(String symbol, IExpression expression) { return new UnaryExpression(symbol, expression); }
        public static IExpression Binary(string op, IExpression arg2, IExpression arg3) { return new BinaryExpression(op, arg2, arg3); }
        public static IExpression Call(string fun, params IExpression[] args) { return new CallExpression(fun, args); }

        public static ExpressionKind Kind(this IExpression expression) {
            return expression.Visit(new KindVisitor());
        }

        private class KindVisitor : IVisitor<ExpressionKind>
        {
            public ExpressionKind Accept(BinaryExpression binary) { return ExpressionKind.Binary; }
            public ExpressionKind Accept(CallExpression call) { return ExpressionKind.Call; }
            public ExpressionKind Accept(LiteralExpression literal) { return ExpressionKind.Literal; }
            public ExpressionKind Accept(UnaryExpression unary) { return ExpressionKind.Unary; }
            public ExpressionKind Accept(VariableExpression var) { return ExpressionKind.Var; }
        }

        public static IExpression Parse(string expression) { return ExpressionParser.Instance.Parse(expression); }


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
                return Processor.Process(unary.Operator, unary.Operand.Visit(this));
            }

            public T Accept(BinaryExpression binary)
            {
                return Processor.Process(binary.Operator, binary.Left.Visit(this), binary.Right.Visit(this));
            }

            public T Accept(LiteralExpression literal)
            {
                return Processor.Process(literal.Value);
            }

            public T Accept(CallExpression call)
            {
                return Processor.Process(call.Function, call.Arguments.Select(a => a.Visit(this)).ToList());
            }
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

            public double Process(string fun, IList<double> args)
            {
                var mangled = String.Format("{0}~{1}", fun, args.Count);
                switch (mangled)
                {
                    case "sin~1":
                        return Math.Sin(args[0]);
                    case "cos~1":
                        return Math.Cos(args[0]);
                    default:
                        // TODO: define specific Exception Type
                        throw new NotImplementedException(String.Format("no such function {0} ( with {1} arguments)", fun, args.Count));
                }
            }

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

            public JToken Process(string fun, IList<JToken> args)
            {
                return new JObject(
                    new JProperty(Tag, "call"),
                    new JProperty("fun", fun),
                    new JProperty("args", new JArray(args)));
            }
        }
    }
}
