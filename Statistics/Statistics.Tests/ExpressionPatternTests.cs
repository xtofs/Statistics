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

    [TestClass()]
    public class ExpressionPatternTests
    {

        [TestMethod(), TestCategory("Matching")]
        public void MatchTest1()
        {
            var pattern = P.Term("*", P.Var("a", "Literal"), P.Var("b", "Var"));

            var expression = E.Binary("*", E.Literal(2), E.Var("x"));

            var actual = pattern.Match(expression);        

            Assert.IsTrue(actual["a"].Equals(((BinaryExpression)expression).Left), "left side is equal");
            Assert.IsTrue(actual["b"].Equals(((BinaryExpression)expression).Right), "right side is equal");
        }
    }
}