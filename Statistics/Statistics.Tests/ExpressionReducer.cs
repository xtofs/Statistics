using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Xof
{
    using E = Expression;
    using P = ExpressionPattern;
    using K = ExpressionKind;

    [TestClass]
    public class ExpressionReducerTest
    {
        public TestContext TestContext { get; set; }


        [TestMethod]
        public void TestXTimesXIsTwoX()
        {
            var expression = E.Binary("+", E.Var("x"), E.Var("x"));

            var actual = expression.Reduce();

            var expected = E.Binary("*", E.Literal(2), E.Var("x"));

            Assert.AreEqual(expected, actual);
        }
    }

    static class ArithmeticExpressionReducerExtensions
    {
        public static IExpression Reduce(this IExpression expression)
        {
            return ArithmeticExpressionReducer.Instance.Reduce(expression);
        }
    }


    class ArithmeticExpressionReducer
    {
        private static Lazy<ArithmeticExpressionReducer> _instance = 
            new Lazy<ArithmeticExpressionReducer>(() => new ArithmeticExpressionReducer());

        public static ArithmeticExpressionReducer Instance { get { return _instance.Value; } }

        internal IExpression Reduce(IExpression expression)
        {
            var match1 = P.Term("+", P.Var("X"), P.Var("X")).TryMatch(expression);
            if (match1 != null)
                return E.Binary("*", E.Literal(2), match1["X"]);

            return expression;
        }
    }
}
