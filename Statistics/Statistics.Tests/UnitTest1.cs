using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace Xof
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestBinaryExpressionParse()
        {
            var actual = Expression.Parse(" 2 * a ");
            var expected = Expression.Binary("*", Expression.Literal(2.0), Expression.Var("a"));

            Assert.IsTrue(actual.Equals(expected));
        }

        [TestMethod]
        public void TestParenthesisParse()
        {
            var actual = Expression.Parse("1 * (2 * a)");
            var expected = Expression.Binary("*", Expression.Literal(1.0),
                Expression.Binary("*", Expression.Literal(2.0), Expression.Var("a")));

            Assert.IsTrue(actual.Equals(expected));
        }

        [TestMethod]
        public void TestChainParse()
        {
            var actual = Expression.Parse("1 * 2 * a");
            var expected = Expression.Binary("*", 
                Expression.Binary("*", Expression.Literal(1.0),  Expression.Literal(2.0)),
                Expression.Var("a"));

            Assert.IsTrue(actual.Equals(expected));
        }

        [TestMethod]
        public void TestEvaluateSimpleExpression()
        {
            var expr = Expression.Binary("*", Expression.Literal(2.0), Expression.Var("a"));
            var binding = new Dictionary<string, Double> { { "a", 2.0 } };
            var actual = expr.Evaluate(binding);

            var expected = binding["a"] * 2.0;

            Assert.AreEqual(actual, expected);
        }

        [TestMethod]
        public void TestEvaluateFunctionCall()
        {
            var expr = Expression.Call("sin", Expression.Var("a"));

            var actual = expr.Evaluate(new Dictionary<string, Double> { { "a", Math.PI * 2 } });

            var expected = 0.0;

            Assert.AreEqual(expected, actual, 1E-5);
        }

        [TestMethod]
        public void TestJsonSerializer()
        {
            var expr = Expression.Binary("*", Expression.Literal(2.0), Expression.Var("a"));

            var actual = expr.ToJson();

            var expected = JObject.Parse(@"{'tag':'binary', 'op': '*',
                'left': {'tag': 'literal', 'value':2.0},
                'right': {'tag': 'var', 'name':'a'}} ");

            Assert.IsTrue(JToken.DeepEquals(expected, actual));
        }
    }
}

