namespace FormulaParser.Tests
{
    public class LexemParserTests
    {

        [TestFixture]
        public class LexemsParserTests
        {
            [Test]
            public void LexAnalyze_ValidExpression()
            {
                // Arrange
                string expression = "(2+3)*4";
                LexemsParser lexemsParser = new LexemsParser();

                // Act
                List<Lexem> lexems = lexemsParser.LexAnalyze(expression);
    
                // Assert
                Assert.IsNotNull(lexems);
                Assert.That(lexems, Has.Count.EqualTo(8));
                Assert.That(lexems[0].type, Is.EqualTo(LexemType.LEFT_BRACKET));
                Assert.That(lexems[1].type, Is.EqualTo(LexemType.NUMBER));
                Assert.That(lexems[2].type, Is.EqualTo(LexemType.OP_PLUS));
                Assert.That(lexems[3].type, Is.EqualTo(LexemType.NUMBER));
                Assert.That(lexems[4].type, Is.EqualTo(LexemType.RIGHT_BRACKET));
                Assert.That(lexems[5].type, Is.EqualTo(LexemType.OP_MUL));
                Assert.That(lexems[6].type, Is.EqualTo(LexemType.NUMBER));
            }

            [Test]
            public void ParseExpression_ValidExpression()
            {
                // Arrange
                string expression = "2+3*4";
                LexemsParser lexemsParser = new LexemsParser();
                List<Lexem> lexems = lexemsParser.LexAnalyze(expression);
                LexemBuffer lexemBuffer = new LexemBuffer(lexems);

                // Act
                string result = lexemsParser.ParseExpression(lexemBuffer);

                // Assert
                Assert.That(result, Is.EqualTo("14"));
            }

            [Test]
            public void ParseExpression_ComplexExpression()
            {
                // Arrange
                string expression = "2*(3+4)/(3+4*5)^2";
                LexemsParser lexemsParser = new LexemsParser();
                List<Lexem> lexems = lexemsParser.LexAnalyze(expression);
                LexemBuffer lexemBuffer = new LexemBuffer(lexems);

                // Act
                string result = lexemsParser.ParseExpression(lexemBuffer);

                // Assert
                Assert.That("0,026465028355387523", Is.EqualTo(result));
            }
            [Test]
            public void ParseExpression_EmptyExpression()
            {
                // Arrange
                string expression = "";
                LexemsParser lexemsParser = new LexemsParser();
                List<Lexem> lexems = lexemsParser.LexAnalyze(expression);
                LexemBuffer lexemBuffer = new LexemBuffer(lexems);

                // Act
                string result = lexemsParser.ParseExpression(lexemBuffer);

                // Assert
                Assert.That(result, Is.EqualTo("Пуста формула."));
            }

            [Test]
            public void ParseExpression_ThrowsMyParserException()
            {
                // Arrange
                string expression = "2*(3+4)/(3+4*5)^2*";
                LexemsParser lexemsParser = new LexemsParser();
                List<Lexem> lexems = lexemsParser.LexAnalyze(expression);
                LexemBuffer lexemBuffer = new LexemBuffer(lexems);

                // Act
                string result = lexemsParser.ParseExpression(lexemBuffer);

                // Assert
                Assert.That("Syntax error: EOF at position: 19", Is.EqualTo(result));

            }

            [Test]
            public void ParseFile_ReturnsCorrectResults()
            {
                // Arrange
                string fname = "formulas.txt";
                File.WriteAllLines(fname, new[] { "1+2*(3+2)", "5-2", "1+x+4", "2*(3+4)/(3+4*5)^2" });

                // Act
                LexemsParser lexemsParser = new LexemsParser();
                List<string> results = lexemsParser.ParseFile(fname);

                // Assert
                Assert.That(4, Is.EqualTo(results.Count));
                Assert.That("1+2*(3+2) = 11", Is.EqualTo(results[0]));
                Assert.That("5-2 = 3", Is.EqualTo(results[1]));
                Assert.That("1+x+4 = Syntax error", Is.EqualTo(results[2]));
                Assert.That("2*(3+4)/(3+4*5)^2 = 0,026465028355387523", Is.EqualTo(results[3]));
                // Cleanup
                File.Delete(fname);
            }

        }
    }
}