namespace nlox;

public class Scanner {
    private readonly string _source;
    private readonly List<Token> _tokens = new();

    public Scanner(string source) {
        _source = source;
    }

    public List<Token> ScanTokens() {
        return _tokens;
    }
}
