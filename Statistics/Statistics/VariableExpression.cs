using System;

namespace Xof
{
    public class VariableExpression : IExpression
    {
        public VariableExpression(String name) { Name = name; }
        public String Name { get; }
        public T Visit<T>(IVisitor<T> visitor) { return visitor.Accept(this); }

        public bool Equals(IExpression other) { var x = other as VariableExpression; return x != null && x.Name.Equals(this.Name); }
    }
}
