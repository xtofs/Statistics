using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xof
{
    public class CallExpression : IExpression
    {
        public CallExpression(String function, IEnumerable<IExpression> arguments) { Function = function; Arguments = arguments.ToList(); }

        public String Function { get; }

        public IList<IExpression> Arguments { get; }

        public T Visit<T>(IVisitor<T> visitor) { return visitor.Accept(this); }

        public bool Equals(IExpression other) {
            var x = other as CallExpression;
            return x != null && x.Function == this.Function && AllEqual(this.Arguments, x.Arguments); }

        private static bool AllEqual(IEnumerable<IExpression> arguments1, IEnumerable<IExpression> arguments2)
        {
            // TODO: this iterates twice over the list. 
            return arguments1.Count() == arguments2.Count() && arguments1.Zip(arguments2, (a, b) => a.Equals(b)).All(x => x);
        }
    }
}
