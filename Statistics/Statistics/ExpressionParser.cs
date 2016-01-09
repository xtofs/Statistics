using System;
using System.Linq;

using Sprache;

namespace Xof
{
    public static class ExpressionParser
    {
        private static readonly Lazy<Parser<IExpression>> _instance = new Lazy<Parser<IExpression>>(() => Parse.ChainOperator(Add.Or(Subtract), Term, Expression.Binary));

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
                from expr in Parse.Ref(() => Instance)
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

        public static Parser<IExpression> Instance
        {
            get
            {
                return _instance.Value;
            }
        }
    }
}
