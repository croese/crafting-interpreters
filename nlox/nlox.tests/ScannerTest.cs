namespace NLox.Tests;

[TestFixture]
[TestOf(typeof(Scanner))]
public class ScannerTest
{
    [SetUp]
    public void Init()
    {
        Lox.HadError = false;
    }

    [Test]
    public void ScanTokensReturnsTokensList()
    {
        var source = """
                     // this is a comment
                     (( )) {} // grouping stuff
                     !*+-,.;/=<> <= >= == != : ? // operators
                     "this is a string" 123.5 42
                     and class else false for fun if nil
                     or print return super this true var while break
                     foo bar
                     /*
                      block comment
                     */
                     """;
        var expected = new List<Token>
        {
            new(TokenType.LEFT_PAREN, "(", null, 2),
            new(TokenType.LEFT_PAREN, "(", null, 2),
            new(TokenType.RIGHT_PAREN, ")", null, 2),
            new(TokenType.RIGHT_PAREN, ")", null, 2),
            new(TokenType.LEFT_BRACE, "{", null, 2),
            new(TokenType.RIGHT_BRACE, "}", null, 2),
            new(TokenType.BANG, "!", null, 3),
            new(TokenType.STAR, "*", null, 3),
            new(TokenType.PLUS, "+", null, 3),
            new(TokenType.MINUS, "-", null, 3),
            new(TokenType.COMMA, ",", null, 3),
            new(TokenType.DOT, ".", null, 3),
            new(TokenType.SEMICOLON, ";", null, 3),
            new(TokenType.SLASH, "/", null, 3),
            new(TokenType.EQUAL, "=", null, 3),
            new(TokenType.LESS, "<", null, 3),
            new(TokenType.GREATER, ">", null, 3),
            new(TokenType.LESS_EQUAL, "<=", null, 3),
            new(TokenType.GREATER_EQUAL, ">=", null, 3),
            new(TokenType.EQUAL_EQUAL, "==", null, 3),
            new(TokenType.BANG_EQUAL, "!=", null, 3),
            new(TokenType.COLON, ":", null, 3),
            new(TokenType.QUESTION, "?", null, 3),
            new(TokenType.STRING, "\"this is a string\"", "this is a string", 4),
            new(TokenType.NUMBER, "123.5", 123.5, 4),
            new(TokenType.NUMBER, "42", (double)42, 4),
            new(TokenType.AND, "and", null, 5),
            new(TokenType.CLASS, "class", null, 5),
            new(TokenType.ELSE, "else", null, 5),
            new(TokenType.FALSE, "false", null, 5),
            new(TokenType.FOR, "for", null, 5),
            new(TokenType.FUN, "fun", null, 5),
            new(TokenType.IF, "if", null, 5),
            new(TokenType.NIL, "nil", null, 5),
            new(TokenType.OR, "or", null, 6),
            new(TokenType.PRINT, "print", null, 6),
            new(TokenType.RETURN, "return", null, 6),
            new(TokenType.SUPER, "super", null, 6),
            new(TokenType.THIS, "this", null, 6),
            new(TokenType.TRUE, "true", null, 6),
            new(TokenType.VAR, "var", null, 6),
            new(TokenType.WHILE, "while", null, 6),
            new(TokenType.BREAK, "break", null, 6),
            new(TokenType.IDENTIFIER, "foo", null, 7),
            new(TokenType.IDENTIFIER, "bar", null, 7),
            new(TokenType.EOF, "", null, 10)
        };

        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens();

        Assert.False(Lox.HadError);
        Assert.That(tokens, Is.EquivalentTo(expected));
    }

    [Test]
    public void ScanTokensErrorOnUnexpectedCharacter()
    {
        var scanner = new Scanner("&");
        scanner.ScanTokens();

        Assert.True(Lox.HadError);
    }
}