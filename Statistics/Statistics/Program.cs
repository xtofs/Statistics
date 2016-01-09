using Sprache;
using System;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;

namespace Statistics
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}

public static class ExpressionParser
{

    static readonly Parser<String> Add = Parse.String("+").Text().Token();
    static readonly Parser<String> Subtract = Parse.String("-").Text().Token();
    static readonly Parser<String> Multiply = Parse.String("*").Text().Token();
    static readonly Parser<String> Divide = Parse.String("/").Text().Token();
    static readonly Parser<String> Power = Parse.String("^").Text().Token();


    static readonly Parser<IExpression> Var =
      Parse.Identifier(Parse.Letter, Parse.LetterOrDigit)
      .Select(name => Expression.Var(name))
      .Named("identifier");

    static readonly Parser<IExpression> Literal =
      Parse.Decimal
      .Select(digits => Expression.Literal(double.Parse(digits)))
      .Named("number");

    static readonly Parser<IExpression> Group = (
            from lparen in Parse.Char('(')
            from expr in Parse.Ref(() => Expr)
            from rparen in Parse.Char(')')
            select expr
       ).Named("group");

    static readonly Parser<IExpression> Factor =
       Group.XOr(Literal).XOr(Var);

    static readonly Parser<IExpression> Operand =
        ((from sign in Parse.Char('-')
          from factor in Factor
          select Expression.Unary("-", factor)
         ).XOr(Factor)).Token();


    static readonly Parser<IExpression> InnerTerm = Parse.ChainOperator(Power, Operand, Expression.Binary);

    static readonly Parser<IExpression> Term = Parse.ChainOperator(Multiply.Or(Divide), InnerTerm, Expression.Binary);

    public static readonly Parser<IExpression> Expr = Parse.ChainOperator(Add.Or(Subtract), Term, Expression.Binary);
}


public static class Expression
{
    public static IExpression Parse(string expression) { return ExpressionParser.Expr.Parse(expression); }

    public static IExpression Var(String name) { return new Var(name); }
    public static IExpression Literal(Double value) { return new Literal(value); }
    public static IExpression Unary(String symbol, IExpression expression) { return new Unary(symbol, expression); }
    public static IExpression Binary(string arg1, IExpression arg2, IExpression arg3) { return new Binary(arg1, arg2, arg3); }

    public static String Show(this IExpression expression)
    {
        return expression.Visit(new Stringify());
    }

    private class Stringify : IVisitor<String>
    {
        public string Accept(Binary binary) { return string.Format("({0} {1} {2})", binary.Left.Show(), binary.Operator, binary.Right.Show()); }
        public string Accept(Literal literal) { return literal.Value.ToString(); }
        public string Accept(Unary unary) { return string.Format("({0} {1}", unary.Operator, unary.Expression.Show()); }
        public string Accept(Var var) { return var.Name.ToString(); }
    }

    //public static Double Evaluate(this IExpression expression, Dictionary<string, Double> bindings)
    //{
    //    return expression.Visit(new Evaluator());
    //}

}

public interface IExpression : IEquatable<IExpression>
{
    T Visit<T>(IVisitor<T> visitor);
}

public interface IVisitor<T>
{
    T Accept(Var var);
    T Accept(Unary unary);
    T Accept(Binary binary);
    T Accept(Literal literal);
}

public class Var : IExpression
{
    public Var(String name) { Name = name; }
    public String Name { get; }
    public T Visit<T>(IVisitor<T> visitor) { return visitor.Accept(this); }

    public bool Equals(IExpression other) { var x = other as Var; return x != null && x.Name.Equals(this.Name); }
}

public class Literal : IExpression
{
    public Literal(Double value) { Value = value; }
    public Double Value { get; }
    public T Visit<T>(IVisitor<T> visitor) { return visitor.Accept(this); }

    public bool Equals(IExpression other) { var x = other as Literal; return x != null && x.Value == this.Value; }
}

public class Binary : IExpression
{
    public Binary(String symbol, IExpression left, IExpression right) { Operator = symbol; Left = left; Right = right; }
    public String Operator { get; private set; }
    public IExpression Left { get; private set; }
    public IExpression Right { get; private set; }
    public T Visit<T>(IVisitor<T> visitor) { return visitor.Accept(this); }

    public bool Equals(IExpression other) { var x = other as Binary; return x != null && x.Left.Equals(this.Left) && x.Right.Equals(this.Right) && x.Operator.Equals(this.Operator); }
}

public class Unary : IExpression
{
    public Unary(String symbol, IExpression operand) { Operator = symbol; Expression = operand; }
    public String Operator { get; private set; }
    public IExpression Expression { get; private set; }
    public T Visit<T>(IVisitor<T> visitor) { return visitor.Accept(this); }

    public bool Equals(IExpression other) { var x = other as Unary; return x != null && x.Expression.Equals(this.Expression) && x.Operator.Equals(this.Operator); }
}

