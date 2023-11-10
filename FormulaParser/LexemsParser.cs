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
    public class Lexem
    {
        public LexemType type;
        public string value;

        public Lexem(LexemType type, string value)
        {
            this.type = type;
            this.value = value;
        }
        public Lexem(LexemType type, char value)
        {
            this.type = type;
            this.value = new String(value, 1);
        }

        override public string ToString()
        {
            return "Lexem{" +
                    "type=" + type +
                    ", value='" + value + '\'' +
                    '}';
        }
    }

    public class LexemBuffer
    {
        private int _pos;

        public List<Lexem> Lexems;

        public LexemBuffer(List<Lexem> Lexems)
        {
            this.Lexems = Lexems;
        }

        public Lexem next()
        {
            return Lexems[_pos++];
        }

        public void Back()
        {
            _pos--;
        }

        public int GetPos()
        {
            return _pos;
        }
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
                            throw new ArgumentException("Неправильний формат виразу.");

                        Lexems.Add(new Lexem(LexemType.NUMBER, expText.Substring(start, pos - start)));

                        break;
                }
            }
            Lexems.Add(new Lexem(LexemType.EOF, ""));
            return Lexems;
        }

        public double ParseExpression(LexemBuffer Lexems)
        {
            Lexem Lexem = Lexems.next();
            if (Lexem.type == LexemType.EOF)
            {
                return 0;
            }
            else
            {
                Lexems.Back();
                return PlusMinus(Lexems);
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
                        throw new Exception("Unexpected token: " + Lexem.value
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
                        throw new Exception("Unexpected token: " + Lexem.value
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
                        throw new Exception("Unexpected token: " + Lexem.value
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
                        throw new Exception("Unexpected token: " + Lexem.value
                                + " at position: " + Lexems.GetPos());
                    }
                    return value;
                default:
                    throw new Exception("Unexpected token: " + Lexem.value
                            + " at position: " + Lexems.GetPos());
            }
        }

    }
}


