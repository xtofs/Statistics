using System;

namespace Xof
{
    public interface IExpression 
    {
        T Visit<T>(IVisitor<T> visitor);
    }
}
