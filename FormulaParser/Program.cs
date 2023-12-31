﻿using FormulaParser;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Versioning;
using System.Xml.Linq;


try
{

    Console.WriteLine("Програма розраховує математичні вирази виду 2*(3+4)/(3+4*5)^2 - доступні операції (+,-,*,/,^,%). ");
    string fname;
    do
    {
        Console.WriteLine("Для виходу з програми введіть `exit`");
        Console.WriteLine("Для введення виразу з клавіатури введіть `1`, або ввдеіть ім'я текстового файлу з формулами(результати будуть виведені в lexemresults.txt): ");
        fname = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(fname))
            while (string.IsNullOrWhiteSpace(fname))
                fname = Console.ReadLine();

        if (fname == "1")
        {
            LexemsParser my = new LexemsParser();
            Console.WriteLine(my.ParseConsoleInput());
        }

        else if (File.Exists(fname))
        {
            LexemsParser my = new LexemsParser();
            List<string> Results = my.ParseFile(fname);

            if (Results.Count > 0)
            {
                File.WriteAllLines("lexemresults.txt", Results);
                Console.WriteLine("Результати успішно записані у файл lexemresults.txt");
            }

        }
        else if (fname!="exit") Console.WriteLine($"Файл {fname} не знайдено( До побачення.");
    } while (fname!="exit");
}
catch (Exception Ex)
{
    Console.WriteLine(Ex.Message+" lalal");
}



