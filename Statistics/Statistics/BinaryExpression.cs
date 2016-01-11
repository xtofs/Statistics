using System;

namespace Xof
{
    public class BinaryExpression : IExpression
    {
        public BinaryExpression(String symbol, IExpression left, IExpression right) { Operator = symbol; Left = left; Right = right; }
        public String Operator { get; }
        public IExpression Left { get; }
        public IExpression Right { get; }
        public T Visit<T>(IVisitor<T> visitor) { return visitor.Accept(this); }
                               
        public override String ToString() { return string.Format("Binary({0} {1} {2})", Operator, Left, Right); }
        
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            if (Object.ReferenceEquals(this, obj))
                return true;
            var other = (BinaryExpression)obj;
            return this.Operator.Equals(other.Operator) && this.Left.Equals(other.Left) && this.Right.Equals(other.Right);
        }
                                                      
        public override int GetHashCode()
        {
            return Tuple.Create(Operator, Left, Right).GetHashCode();
        }
    }
}
