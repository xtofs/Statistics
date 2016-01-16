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
        
        public override String ToString() { return string.Format("Call({0} {1} {2})", Function, String.Join(" ", Arguments)); }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            if (Object.ReferenceEquals(this, obj))
                return true;
            var other = (CallExpression)obj;
            return this.Function.Equals(other.Function) && Enumerable.SequenceEqual(this.Arguments, other.Arguments);
        }

        public override int GetHashCode()
        {
            return Tuple.Create(Function, Arguments).GetHashCode();
        }
    }
}
