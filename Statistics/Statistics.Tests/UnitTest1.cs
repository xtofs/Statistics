using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Xof
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
    }
}
