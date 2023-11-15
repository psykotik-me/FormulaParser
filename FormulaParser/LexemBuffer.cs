
namespace FormulaParser
{
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
}