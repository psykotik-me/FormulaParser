using FormulaParser;

try
{


    LexemsParser my = new LexemsParser();
    string expressionText = "122 - 34 - 3* (55 + 5* (3 - 2)) / 2";
    expressionText = "2*(3+4)/(3+4*5)^2";
    Console.WriteLine("Введіть математичний вираз(операції +,-,*,/,^,%):   наприклад  122 - 34 - 3* (55 + 5* (3 - 2)) / 2^6)");
    expressionText = Console.ReadLine();
    expressionText = expressionText.Replace(" ", "");
    List<Lexem> Lexems = my.LexAnalyze(expressionText);
    //foreach (Lexem lexem in Lexems)
    //{
    //    Console.WriteLine(lexem.ToString());
    //}
    LexemBuffer LexemBuffer = new LexemBuffer(Lexems);
    Console.WriteLine(my.ParseExpression(LexemBuffer));    
    
   //string expr = "2*(3+4)/(3+4*5)^2";
    
    //// Використовучи рекурсивний спуск
    //Console.WriteLine(expr);
    //RecursionDownCalculator calc = new RecursionDownCalculator();
    //double result = calc.Calculate(expr);
    //Console.WriteLine($"Рекурсивний спуск: {result}");
    //Console.WriteLine("Next formula: ");
    //expr = Console.ReadLine();
    //result = calc.Calculate(expr);
    //Console.WriteLine($"Рекурсивний спуск: {result}");

    //// Використовуючи Jint    
    //result = FormulaParser.FormulaParser.Jvaluate(expr);
    //Console.WriteLine($"Jint Результат: {result}");

    //// Використовуючи NCalc
    ////double res = FormulaParser.FormulaParser.Evaluate(expr);
    //Console.WriteLine("NCalc результат: " + FormulaParser.FormulaParser.Evaluate(expr));

}
catch (Exception Ex)
{
    Console.WriteLine(Ex.Message);
}



