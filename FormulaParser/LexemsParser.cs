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
                            //throw new MyParserException("Неправильний формат виразу - "+expText);
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
                    return PlusMinus(Lexems).ToString();
                }
            }
            catch(MyParserException ex)
            {
                return ex.Message;
            }
        }

        private double PlusMinus(LexemBuffer Lexems)
        {
            double value = MultDiv(Lexems);
            while (true)
            {
                Lexem Lexem = Lexems.next();
                switch (Lexem.type)
                {
                    case LexemType.OP_PLUS:
                        value += MultDiv(Lexems);
                        break;
                    case LexemType.OP_MINUS:
                        value -= MultDiv(Lexems);
                        break;
                    case LexemType.EOF:
                    case LexemType.RIGHT_BRACKET:
                        Lexems.Back();
                        return value;
                    default:
                        throw new MyParserException("Unexpected token: " + Lexem.value
                                + " at position: " + Lexems.GetPos());
                }
            }
        }

        private double MultDiv(LexemBuffer Lexems)
        {
            double value = Pow(Lexems);
            while (true)
            {
                Lexem Lexem = Lexems.next();
                switch (Lexem.type)
                {
                    case LexemType.OP_MUL:
                        value *= Pow(Lexems);
                        break;
                    case LexemType.OP_DIV:
                        value /= Pow(Lexems);
                        break;
                    case LexemType.OP_MOD:
                        value %= Pow(Lexems);
                        break;
                    case LexemType.EOF:
                    case LexemType.RIGHT_BRACKET:
                    case LexemType.OP_PLUS:
                    case LexemType.OP_MINUS:
                        Lexems.Back();
                        return value;
                    default:
                        throw new MyParserException("Unexpected token: " + Lexem.value
                                + " at position: " + Lexems.GetPos());
                }
            }
        }
        private double Pow(LexemBuffer Lexems)
        {
            double value = Factor(Lexems);
            while (true)
            {
                Lexem Lexem = Lexems.next();
                switch (Lexem.type)
                {
                    case LexemType.OP_POW:
                        value = Math.Pow(value, Factor(Lexems));
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
                        throw new MyParserException("Unexpected token: " + Lexem.value
                                + " at position: " + Lexems.GetPos());
                }
            }
        }

        private double Factor(LexemBuffer Lexems)
        {
            Lexem Lexem = Lexems.next();
            switch (Lexem.type)
            {
                case LexemType.NUMBER:
                    return double.Parse(Lexem.value);
                case LexemType.LEFT_BRACKET:
                    double value = PlusMinus(Lexems);
                    Lexem = Lexems.next();
                    if (Lexem.type != LexemType.RIGHT_BRACKET)
                    {
                        throw new MyParserException("Unexpected token: " + Lexem.value
                                + " at position: " + Lexems.GetPos());
                    }
                    return value;
                default:
                    throw new MyParserException("Unexpected token: " + Lexem.value
                            + " at position: " + Lexems.GetPos());
            }
        }

    }
}


