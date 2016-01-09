using System;

namespace Xof
{
    public class Binary : IExpression
    {
        public Binary(String symbol, IExpression left, IExpression right) { Operator = symbol; Left = left; Right = right; }
        public String Operator { get; private set; }
        public IExpression Left { get; private set; }
        public IExpression Right { get; private set; }
        public T Visit<T>(IVisitor<T> visitor) { return visitor.Accept(this); }

        public bool Equals(IExpression other) { var x = other as Binary; return x != null && x.Left.Equals(this.Left) && x.Right.Equals(this.Right) && x.Operator.Equals(this.Operator); }
    }
}
