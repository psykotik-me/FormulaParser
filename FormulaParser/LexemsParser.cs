using FormulaParser;
namespace FormulaParser
{
    /*------------------------------------------------------------------
    * PARSER RULES
    *------------------------------------------------------------------*/

    //    expr : plusminus* EOF ;
    //
    //    plusminus: multdiv ( ( '+' | '-' ) multdiv )* ;
    //
    //    multdiv : factor ( ( '*' | '/' ) factor )* ;
    //
    //    pow:  factor ( ( '^' | '%') factor)* ;
    //
    //    factor : NUMBER | '(' expr ')' ;


    public enum LexemType
    {
        LEFT_BRACKET, RIGHT_BRACKET,
        OP_PLUS, OP_MINUS, OP_MUL, OP_DIV,
        OP_POW, OP_MOD,
        NUMBER,
        EOF
    }

    public class LexemsParser
    {

        public List<string> ParseFile(string fname)
        {
            List<string> Results = new List<string>();
            string[] lines = File.ReadAllLines(fname);
            if (lines.Length > 0)
            {
                LexemsParser my = new LexemsParser();
                foreach (string line in lines)
                {
                    List<Lexem> Lexems = my.LexAnalyze(line.Replace(" ", ""));
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


        public string ParseConsoleInput()
        {
            Console.WriteLine("Введіть математичний вираз:   наприклад  122 - 34 - 3* (55 + 5* (3 - 2)) / 2^6)");
            string res;
            string expressionText = Console.ReadLine();
            expressionText = expressionText.Replace(" ", "");
            List<Lexem> Lexems = LexAnalyze(expressionText);
            if (Lexems != null)
            {
                LexemBuffer LexemBuffer = new LexemBuffer(Lexems);
                res = ParseExpression(LexemBuffer);
                Console.WriteLine();
            }
            else res = "Вираз містить невідомі токени - " + expressionText;
            return res;
        }

        public List<Lexem> LexAnalyze(string expText)
        {
            List<Lexem> Lexems = new List<Lexem>();
            int pos = 0;
            while (pos < expText.Length)
            {
                char c = expText[pos];
                switch (c)
                {
                    case '(':
                        Lexems.Add(new Lexem(LexemType.LEFT_BRACKET, c));
                        pos++;
                        continue;
                    case ')':
                        Lexems.Add(new Lexem(LexemType.RIGHT_BRACKET, c));
                        pos++;
                        continue;
                    case '+':
                        Lexems.Add(new Lexem(LexemType.OP_PLUS, c));
                        pos++;
                        continue;
                    case '-':
                        Lexems.Add(new Lexem(LexemType.OP_MINUS, c));
                        pos++;
                        continue;
                    case '*':
                        Lexems.Add(new Lexem(LexemType.OP_MUL, c));
                        pos++;
                        continue;
                    case '/':
                        Lexems.Add(new Lexem(LexemType.OP_DIV, c));
                        pos++;
                        continue;
                    case '^':
                        Lexems.Add(new Lexem(LexemType.OP_POW, c));
                        pos++;
                        continue;
                    case '%':
                        Lexems.Add(new Lexem(LexemType.OP_MOD, c));
                        pos++;
                        continue;
                    default:
                        int start = pos;
                        while (pos < expText.Length && (char.IsDigit(expText[pos]) || expText[pos] == '.'))
                            pos++;

                        if (start == pos)
                            //throw new MyParserException("Вираз містить невідомі токени - "+expText);
                            return null;

                        Lexems.Add(new Lexem(LexemType.NUMBER, expText.Substring(start, pos - start)));

                        break;
                }
            }
            Lexems.Add(new Lexem(LexemType.EOF, ""));
            return Lexems;
        }

        public string ParseExpression(LexemBuffer Lexems)
        {
            try
            {
                Lexem Lexem = Lexems.next();
                if (Lexem.type == LexemType.EOF)
                {
                    return "Пуста формула.";
                }
                else
                {
                    Lexems.Back();
                    return _PlusMinus(Lexems).ToString();
                }
            }
            catch(MyParserException ex)
            {
                return ex.Message;
            }
        }

        private double _PlusMinus(LexemBuffer Lexems)
        {
            double value = _MultDiv(Lexems);
            while (true)
            {
                Lexem Lexem = Lexems.next();
                switch (Lexem.type)
                {
                    case LexemType.OP_PLUS:
                        value += _MultDiv(Lexems);
                        break;
                    case LexemType.OP_MINUS:
                        value -= _MultDiv(Lexems);
                        break;
                    case LexemType.EOF:
                    case LexemType.RIGHT_BRACKET:
                        Lexems.Back();
                        return value;
                    default:
                        throw new MyParserException("Syntax error: " + Lexem.type
                                + " at position: " + Lexems.GetPos());
                }
            }
        }

        private double _MultDiv(LexemBuffer Lexems)
        {
            double value = _Pow(Lexems);
            while (true)
            {
                Lexem Lexem = Lexems.next();
                switch (Lexem.type)
                {
                    case LexemType.OP_MUL:
                        value *= _Pow(Lexems);
                        break;
                    case LexemType.OP_DIV:
                        double t = _Pow(Lexems);          
                        if (t != 0d) value /= t;
                        else throw new MyParserException("Division by zero! ");
                        break;
                    case LexemType.OP_MOD:
                        value %= _Pow(Lexems);
                        break;
                    case LexemType.EOF:
                    case LexemType.RIGHT_BRACKET:
                    case LexemType.OP_PLUS:
                    case LexemType.OP_MINUS:
                        Lexems.Back();
                        return value;
                    default:
                        throw new MyParserException("Syntax error: " + Lexem.type
                                + " at position: " + Lexems.GetPos());
                }
            }
        }
        private double _Pow(LexemBuffer Lexems)
        {
            double value = _Factor(Lexems);
            while (true)
            {
                Lexem Lexem = Lexems.next();
                switch (Lexem.type)
                {
                    case LexemType.OP_POW:
                        value = Math.Pow(value, _Factor(Lexems));
                        break;
                    case LexemType.EOF:                     
                    case LexemType.RIGHT_BRACKET:
                    case LexemType.OP_PLUS:
                    case LexemType.OP_MINUS:
                    case LexemType.OP_MUL:
                    case LexemType.OP_DIV:
                        Lexems.Back();
                        return value;
                    case LexemType.OP_MOD:
                        Lexems.Back();
                        return value;
                    default:
                        throw new MyParserException("Syntax error: " + Lexem.type
                                + " at position: " + Lexems.GetPos());
                }
            }
        }

        private double _Factor(LexemBuffer Lexems)
        {
            Lexem Lexem = Lexems.next();
            switch (Lexem.type)
            {
                case LexemType.NUMBER:
                    return double.Parse(Lexem.value);
                case LexemType.LEFT_BRACKET:
                    double value = _PlusMinus(Lexems);
                    Lexem = Lexems.next();
                    if (Lexem.type != LexemType.RIGHT_BRACKET)
                    {
                        throw new MyParserException("Syntax error: " + Lexem.type
                                + " at position: " + Lexems.GetPos());
                    }
                    return value;
                default:
                    throw new MyParserException("Syntax error: " + Lexem.type
                            + " at position: " + Lexems.GetPos());
            }
        }

    }
}


