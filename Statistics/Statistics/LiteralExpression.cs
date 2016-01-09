using System;

namespace Xof
{
    public class LiteralExpression : IExpression
    {
        public LiteralExpression(Double value) { Value = value; }
        public Double Value { get; }
        public T Visit<T>(IVisitor<T> visitor) { return visitor.Accept(this); }

        public bool Equals(IExpression other) { var x = other as LiteralExpression; return x != null && x.Value == this.Value; }
    }
}
