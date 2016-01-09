using System;

namespace Xof
{
    public interface IExpression : IEquatable<IExpression>
    {
        T Visit<T>(IVisitor<T> visitor);
    }
}
