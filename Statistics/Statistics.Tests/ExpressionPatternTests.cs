using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xof;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xof
{

    using E = Expression;      
    using P = ExpressionPattern;
    using K = ExpressionKind;

    [TestClass()]
    public class ExpressionPatternTests
    {

        [TestMethod(), TestCategory("Matching")]
        public void MatchBinaryPatternTest1()
        {
            var pattern = P.Term("*", P.Var("a", K.Literal), P.Var("b", K.Var));

            var expression = E.Binary("*", E.Literal(2), E.Var("x"));

            var actual = pattern.Match(expression);        

            Assert.IsTrue(actual["a"].Equals(((BinaryExpression)expression).Left), "left side is equal");
            Assert.IsTrue(actual["b"].Equals(((BinaryExpression)expression).Right), "right side is equal");
        }

        [TestMethod(), TestCategory("Matching")]
        public void MatchCallPatternTest1()
        {
            var pattern = P.Term("sin", P.Var("a", K.Literal));

            var expression = E.Call("sin", E.Literal(2));

            var actual = pattern.Match(expression);

            Assert.IsTrue(actual["a"].Equals(((CallExpression)expression).Arguments[0]), "argument is equal");
        }

        [TestMethod(), TestCategory("Matching")]
        public void MatchAnyOperatorTest()
        {
            var pattern = P.Term(null, P.Var("a", K.Literal), P.Var("b", K.Var));

            var expression = E.Binary("*", E.Literal(2), E.Var("x"));

            var actual = pattern.Match(expression);

            Assert.IsTrue(actual["a"].Equals(((BinaryExpression)expression).Left), "left side is equal");
            Assert.IsTrue(actual["b"].Equals(((BinaryExpression)expression).Right), "right side is equal");
        }

        [TestMethod(), TestCategory("Matching")]
        public void MatchTestFailedKindTest()
        {
            var pattern = P.Term("*", P.Var("a", K.Literal), P.Var("b", K.Literal));

            var expression = E.Binary("*", E.Literal(2), E.Var("x"));

            Exception exception = null;
            try {
                var actual = pattern.Match(expression);
            }
            catch(NoMatchException ex) {
                exception = ex;
            }

            Assert.IsNotNull(exception);
        }
    }
}