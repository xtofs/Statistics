using System;
using System.Linq;

using Sprache;

namespace Xof
{
    public static class ExpressionParser
    {
        public static Parser<IExpression> Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        // TODO: move static members to a separate class.
        // the lazy evaluation is nice, but since these are all static properties they get initialized at startup anyways.
        private static readonly Lazy<Parser<IExpression>> _instance = new Lazy<Parser<IExpression>>(() => Expr);


        static readonly Parser<char> OpenParenthesis = Parse.Char('(');
        static readonly Parser<char> CloseParenthesis = Parse.Char(')');
        static readonly Parser<char> Comma = Parse.Char(',');

        static readonly Parser<String> Add = Parse.String("+").Text().Token();
        static readonly Parser<String> Subtract = Parse.String("-").Text().Token();
        static readonly Parser<String> Multiply = Parse.String("*").Text().Token();
        static readonly Parser<String> Divide = Parse.String("/").Text().Token();
        static readonly Parser<String> Power = Parse.String("^").Text().Token();

        static readonly Parser<String> Identifier = Parse.Identifier(Parse.Letter, Parse.LetterOrDigit);

        static readonly Parser<IExpression> Expr0 = Parse.Ref(() => Expr);

        static readonly Parser<IExpression> Var = 
            (from name in Identifier select Expression.Var(name)).Named("identifier");

        static readonly Parser<IExpression> Literal =
            (from digits in Parse.Decimal select Expression.Literal(double.Parse(digits))).Named("number");
                                                                                                  
        static readonly Parser<IExpression> Group = Expr0.Contained(OpenParenthesis, CloseParenthesis).Named("subexpression");

        static readonly Parser<IExpression> Call =
            from fun in Identifier
            from args in Expr0.DelimitedBy(Comma).Contained(OpenParenthesis, CloseParenthesis)
            select Expression.Call(fun, args.ToArray());

        static readonly Parser<IExpression> Factor =
           Group.XOr(Literal).XOr( Call.Or(Var) );

        static readonly Parser<IExpression> Prefixed =
            from sign in Parse.Char('-')
            from factor in Factor
            select Expression.Unary("-", factor);

        static readonly Parser<IExpression> Operand =
            ((Prefixed).XOr(Factor)).Token();


        static readonly Parser<IExpression> InnerTerm = Parse.ChainOperator(Power, Operand, Expression.Binary);

        static readonly Parser<IExpression> Term = Parse.ChainOperator(Multiply.Or(Divide), InnerTerm, Expression.Binary);

        static readonly Parser<IExpression> Expr = Parse.ChainOperator(Add.Or(Subtract), Term, Expression.Binary);
    }
}
