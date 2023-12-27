namespace nlox.tests;

public class ScannerTests {
    public ScannerTests() {
        Lox.ClearError();
    }

    [Fact]
    public void ScanTokensIgnoresBlockComments() {
        var s = new Scanner("/* block comment */");
        var tokens = s.ScanTokens();

        Assert.False(Lox.HadError);
        Assert.Collection(tokens,
            t => Assert.Equal(TokenType.EOF, t.Type));
    }

    [Fact]
    public void ScanTokensIgnoresMultilineBlockComments() {
        var s = new Scanner(@"/* block comment
over multiple
lines */");
        var tokens = s.ScanTokens();

        Assert.False(Lox.HadError);
        Assert.Collection(tokens,
            t => Assert.Equal(TokenType.EOF, t.Type));
    }

    [Theory]
    [InlineData("/* comment")]
    [InlineData("/* comment *")]
    public void ScanTokensReportsErrorForUnterminatedBlockComments(string source) {
        var s = new Scanner(source);
        var unused = s.ScanTokens();

        Assert.True(Lox.HadError);
    }
}
