namespace Xof
{
    public interface IVisitor<T>
    {
        T Accept(VariableExpression var);
        T Accept(UnaryExpression unary);
        T Accept(BinaryExpression binary);
        T Accept(LiteralExpression literal);
    }
}
