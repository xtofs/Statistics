using System;

namespace Xof
{
    public class Literal : IExpression
    {
        public Literal(Double value) { Value = value; }
        public Double Value { get; }
        public T Visit<T>(IVisitor<T> visitor) { return visitor.Accept(this); }

        public bool Equals(IExpression other) { var x = other as Literal; return x != null && x.Value == this.Value; }
    }
}
