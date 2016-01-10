using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xof
{
    // names should reflect the names of the Expressoion factory methods.
    public enum ExpressionKind
    {
        Var,   
        Literal,
        Unary,
        Binary,
        Call
    }
}
