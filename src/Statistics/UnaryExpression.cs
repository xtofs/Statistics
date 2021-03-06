﻿using System;

namespace Xof
{
    public class UnaryExpression : IExpression
    {
        public UnaryExpression(String symbol, IExpression operand) { Operator = symbol; Operand = operand; }

        public String Operator { get; }

        public IExpression Operand { get; }

        public T Visit<T>(IVisitor<T> visitor) { return visitor.Accept(this); }

        public override String ToString() { return string.Format("Unary({0} {1})", Operator, Operand); }
                                                  
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            if (Object.ReferenceEquals(this, obj))
                return true;
            var other = (UnaryExpression)obj;
            return this.Operator.Equals(other.Operator) && this.Operand.Equals(other.Operand);
        }

        public override int GetHashCode()
        {
            return Tuple.Create(Operator, Operand).GetHashCode();
        }
    }
}
