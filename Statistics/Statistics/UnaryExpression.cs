﻿using System;

namespace Xof
{
    public class UnaryExpression : IExpression
    {
        public UnaryExpression(String symbol, IExpression operand) { Operator = symbol; Expression = operand; }
        public String Operator { get; private set; }
        public IExpression Expression { get; private set; }
        public T Visit<T>(IVisitor<T> visitor) { return visitor.Accept(this); }

        public bool Equals(IExpression other) { var x = other as UnaryExpression; return x != null && x.Expression.Equals(this.Expression) && x.Operator.Equals(this.Operator); }
    }
}