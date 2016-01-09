namespace Xof
{
    public interface IVisitor<T>
    {
        T Accept(Var var);
        T Accept(Unary unary);
        T Accept(Binary binary);
        T Accept(Literal literal);
    }
}
