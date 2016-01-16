using System;                                   

namespace Xof
{
    public class LiteralExpression : IExpression
    {
        public LiteralExpression(Double value) { Value = value; }

        public Double Value { get; }

        public T Visit<T>(IVisitor<T> visitor) { return visitor.Accept(this); }

        public override String ToString() { return string.Format("Literal({0})", Value); }

        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
                return false;            
            var other = (LiteralExpression)obj;
            return this.Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
