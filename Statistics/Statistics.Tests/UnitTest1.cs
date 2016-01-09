using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Statistics.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var actual = Expression.Parse(" 2 * a ");
            var expected = Expression.Binary("*", Expression.Literal(2.0), Expression.Var("a"));

            Assert.IsTrue(actual.Equals(expected));
        }

        [TestMethod]
        public void TestMethod2()
        {
            var binding = new Dictionary<string, Double> { { "a", 2.0 } };

            var expr = Expression.Binary("*", Expression.Literal(2.0), Expression.Var("a"));
            var actual = expr.Evaluate(binding);

            var expected = binding["a"] * 2.0;

            Assert.AreEqual(actual, expected);        
        }
    }
}
