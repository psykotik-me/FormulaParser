using NCalc;
using System;

namespace FormulaParser
{
    public static class FormulaParser
    {
        public static double Evaluate(string expression)
        {
            NCalc.Expression e = new(expression);
            if (e.HasErrors())
            {
                Console.WriteLine(e.Error);
            }
            return (double)e.Evaluate();
        }
    }

}
