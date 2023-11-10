using NCalc;
using Jint;
using System.Data;


namespace FormulaParser
{
    public static class FormulaParser
    {

        // Evaluate using NCalc
        public static object Evaluate(string expression)
        {
            NCalc.Expression e = new(expression);
            if (e.HasErrors())
            {
                Console.WriteLine(e.Error);
            }
            return e.Evaluate();
        }

        //  Evaluate using Jint
        public static double Jvaluate(string expression)
        {
            var engine = new Engine();
            var result = engine.Execute(expression).GetCompletionValue().AsNumber();
            return result;
        }

        // Evaluate using System.Data.DataTable 
        public static double DTEvaluate(string expression)
        {
            DataTable table = new DataTable();
            table.Columns.Add("expression", typeof(string), expression);

            DataRow row = table.NewRow();
            table.Rows.Add(row);

            double result = double.Parse((string)row["expression"]);
            return result;
        }
    }

    // Клас, що використовує рекурсивний спуск
    class RecursionDownCalculator
    {
        private string expression;
        private int index;

        public double Calculate(string expr)
        {
            expression = expr;
            index = 0;
            return ParseExpression();
        }

        private double ParseExpression()
        {
            double left = ParseTerm();

            while (index < expression.Length)
            {
                char op = expression[index];
                if (op != '+' && op != '-')
                    break;
                index++;


                {
                    double right = ParseTerm();

                    if (op == '+')
                        left += right;
                    else
                        left -= right;
                }
            }

            return left;
        }

        private double ParseTerm()
        {
            double left = ParseFactor();
            Console.Write(" ParseTerm: left-> " + left);
            while (index < expression.Length)
            {
                char op = expression[index];
                if (op != '*' && op != '/' && op != '^')
                    break;
                index++;

                if (op == '^')
                {
                    double right = ParseExpression();
                    Console.Write("  powleft-> " + left + "  powright-> " + right);
                    left = Math.Pow(left, right);
                    Console.Write("  pow-> " + left);
                }
                else
                {
                    double right = ParseFactor();
                    Console.Write(" */right-> " + right);
                    if (op == '*')
                        left *= right;
                    else
                        left /= right;
                }
            }
            Console.WriteLine(" return-> "+left);

            return left;
        }

        private double ParseFactor()
        {
            double result;

            if (expression[index] == '(')
            {
                index++;
                result = ParseExpression();
                if (index >= expression.Length || expression[index] != ')')
                    throw new ArgumentException("Неправильний формат виразу.");
                index++;
            }
            else
            {
                int startIndex = index;
                while (index < expression.Length && (char.IsDigit(expression[index]) || expression[index] == '.'))
                    index++;

                if (startIndex == index)
                    throw new ArgumentException("Неправильний формат виразу.");

                result = double.Parse(expression.Substring(startIndex, index - startIndex));
            }

            return result;
        }
    }

}
