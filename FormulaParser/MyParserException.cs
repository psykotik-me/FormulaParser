using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormulaParser
{
    public class MyParserException : Exception
    {
        // Конструктор без параметрів (потрібний для серіалізації)
        public MyParserException() { }

        // Конструктор з повідомленням про помилку
        public MyParserException(string message) : base(message) { }

        // Конструктор з повідомленням про помилку та внутрішнім винятком
        public MyParserException(string message, Exception innerException) : base(message, innerException) { }
    }
}
