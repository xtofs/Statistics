using System;

namespace Xof
{
    public class VariableExpression : IExpression
    {
        public VariableExpression(String name) { Name = name; }
        public String Name { get; }
        public T Visit<T>(IVisitor<T> visitor) { return visitor.Accept(this); }

        public override String ToString() { return this.Show(); }

        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
                return false;
            var other = (VariableExpression)obj;
            return this.Name.Equals(other.Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
