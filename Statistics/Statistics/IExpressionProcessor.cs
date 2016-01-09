using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xof
{    
    public interface IExpressionProcessor<T>
    {
        T Process(double value);
        T Process(string var);
        T Process(string op, T expression);
        T Process(string op, T left, T right);
    }
}
