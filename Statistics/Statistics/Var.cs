using System;

namespace Xof
{
    public class Var : IExpression
    {
        public Var(String name) { Name = name; }
        public String Name { get; }
        public T Visit<T>(IVisitor<T> visitor) { return visitor.Accept(this); }

        public bool Equals(IExpression other) { var x = other as Var; return x != null && x.Name.Equals(this.Name); }
    }
}
