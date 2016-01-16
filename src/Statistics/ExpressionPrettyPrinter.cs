using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xof
{
    static class ExpressionPrettyPrinter
    {      
        public static String Show(this IExpression expression)
        {
            return expression.Process(new Stringifier());
        }
                                 
        private class Stringifier : IExpressionProcessor<String>
        {
            public string Process(string var) { return string.Format("{0}", var); }
            public string Process(double value) { return string.Format("{0}", value); }
            public string Process(string op, string expression) { return string.Format("({0} {1}", op, expression); }
            public string Process(string op, string left, string right) { return string.Format("({0} {1} {2})", left, op, right); }
            public string Process(string fun, IList<string> args) { return string.Format("{0}({1})", fun, String.Join(", ", args)); }
        }
    }
}
