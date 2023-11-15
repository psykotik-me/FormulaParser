using FormulaParser;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;

List<string> ParseFile(string fname)
{
    List<string> Results = new List<string>();
    string[] lines = File.ReadAllLines(fname);
    if (lines.Length > 0)
    {
        LexemsParser my = new LexemsParser();
        foreach (string line in lines)
        {
            List<Lexem> Lexems = my.LexAnalyze(line);
            if (Lexems == null)
            {
                Results.Add(line + " = Syntax error");
                continue;
            }
            LexemBuffer LexemBuffer = new LexemBuffer(Lexems);
            Results.Add(line + " = " + my.ParseExpression(LexemBuffer));
        }
    }
    return Results;
}

try
{

    Console.WriteLine("Програма розраховує математичні вирази виду 2*(3+4)/(3+4*5)^2 - доступні операції (+,-,*,/,^,%). ");
    Console.WriteLine("Для введення виразу з клавіатури введіть `1`, або ввдеіть ім'я текстового файлу з формулами(результати будуть виведені в lexemresults.txt): ");
    string fname = Console.ReadLine();

    if (fname == "1")
    {
        LexemsParser my = new LexemsParser();
        string expressionText = "2*(3+4)/(3+4*5)^2";
        Console.WriteLine("Введіть математичний вираз:   наприклад  122 - 34 - 3* (55 + 5* (3 - 2)) / 2^6)");
        expressionText = Console.ReadLine();
        expressionText = expressionText.Replace(" ", "");
        List<Lexem> Lexems = my.LexAnalyze(expressionText);
        if (Lexems != null)
        {
            LexemBuffer LexemBuffer = new LexemBuffer(Lexems);
            Console.WriteLine(my.ParseExpression(LexemBuffer));
        }
        else Console.WriteLine("Неправильний формат виразу - "+expressionText);
    }
    
    else if (File.Exists(fname))
    {
        List<string> Results = ParseFile(fname);
       
            if (Results.Count > 0)
            {
                File.WriteAllLines("lexemresults.txt", Results);
                Console.WriteLine("Результати успішно записані у файл lexemresults.txt");
            }
        
    }
    else Console.WriteLine($"Файл {fname} не знайдено( До побачення.");

}
catch (Exception Ex)
{
    Console.WriteLine(Ex.Message);
}



