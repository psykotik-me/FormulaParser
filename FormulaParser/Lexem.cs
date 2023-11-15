using FormulaParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormulaParser
{
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

}


