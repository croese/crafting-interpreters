namespace nlox;

public class Scanner {
    private static readonly Dictionary<string, TokenType> Keywords = new() {
        { "and", TokenType.AND },
        { "break", TokenType.BREAK },
        { "class", TokenType.CLASS },
        { "else", TokenType.ELSE },
        { "false", TokenType.FALSE },
        { "for", TokenType.FOR },
        { "fun", TokenType.FUN },
        { "if", TokenType.IF },
        { "nil", TokenType.NIL },
        { "or", TokenType.OR },
        { "print", TokenType.PRINT },
        { "return", TokenType.RETURN },
        { "super", TokenType.SUPER },
        { "this", TokenType.THIS },
        { "true", TokenType.TRUE },
        { "var", TokenType.VAR },
        { "while", TokenType.WHILE }
    };

    private readonly string _source;
    private readonly List<Token> _tokens = new();
    private int _current; // index of char currently being considered
    private int _currentColumn = 1;
    private int _line = 1;
    private int _start; // index of first char of lexeme being scanned
    private int _startColumn;

    public Scanner(string source) {
        _source = source;
    }

    public List<Token> ScanTokens() {
        while (!IsAtEnd()) {
            _start = _current;
            _startColumn = _currentColumn;
            ScanToken();
        }

        _tokens.Add(new Token(TokenType.EOF, string.Empty,
            null, _line, _currentColumn));
        return _tokens;
    }

    private void ScanToken() {
        var c = Advance();
        switch (c) {
            case '(':
                AddToken(TokenType.LEFT_PAREN);
                break;
            case ')':
                AddToken(TokenType.RIGHT_PAREN);
                break;
            case '{':
                AddToken(TokenType.LEFT_BRACE);
                break;
            case '}':
                AddToken(TokenType.RIGHT_BRACE);
                break;
            case ',':
                AddToken(TokenType.COMMA);
                break;
            case '.':
                AddToken(TokenType.DOT);
                break;
            case '-':
                AddToken(TokenType.MINUS);
                break;
            case '+':
                AddToken(TokenType.PLUS);
                break;
            case ';':
                AddToken(TokenType.SEMICOLON);
                break;
            case '*':
                AddToken(TokenType.STAR);
                break;
            case '!':
                AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                break;
            case '=':
                AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                break;
            case '<':
                AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                break;
            case '>':
                AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                break;
            case '/':
                if (Match('/')) {
                    while (Peek() != '\n' && !IsAtEnd()) {
                        Advance();
                    }
                } else if (Match('*')) {
                    BlockComment();
                } else {
                    AddToken(TokenType.SLASH);
                }

                break;
            case '?':
                AddToken(TokenType.QUESTION);
                break;
            case ':':
                AddToken(TokenType.COLON);
                break;
            case ' ':
            case '\r':
            case '\t':
                break;
            case '\n':
                _line++;
                _currentColumn = 1;
                break;
            case '"':
                String();
                break;
            default:
                if (IsDigit(c)) {
                    Number();
                } else if (IsAlpha(c)) {
                    Identifier();
                } else {
                    Lox.Error(_line, "unexpected character.");
                }

                break;
        }
    }

    private void BlockComment() {
        while (Peek() != '*' && PeekNext() != '/' && !IsAtEnd()) {
            if (Peek() == '\n') {
                _line++;
            }

            Advance();
        }

        if (IsAtEnd()) {
            Lox.Error(_line, "unterminated block comment.");
            return;
        }

        // eat the "*/" - make sure to check if we hit the end before reading both chars
        Advance();

        if (IsAtEnd()) {
            Lox.Error(_line, "unterminated block comment.");
            return;
        }

        Advance();
    }

    private void Identifier() {
        while (IsAlphaNumeric(Peek())) {
            Advance();
        }

        var text = _source.Substring(_start, _current - _start);
        var type = Keywords.GetValueOrDefault(text, TokenType.IDENTIFIER);

        AddToken(type);
    }

    private bool IsAlphaNumeric(char c) {
        return IsAlpha(c) || IsDigit(c);
    }

    private bool IsAlpha(char c) {
        return (c >= 'a' && c <= 'z') ||
               (c >= 'A' && c <= 'Z') ||
               c == '_';
    }

    private void Number() {
        while (IsDigit(Peek())) {
            Advance();
        }

        if (Peek() == '.' && IsDigit(PeekNext())) {
            // eat the "."
            Advance();

            while (IsDigit(Peek())) {
                Advance();
            }
        }

        AddToken(TokenType.NUMBER,
            double.Parse(_source.Substring(_start, _current - _start)));
    }

    private char PeekNext() {
        if (_current + 1 >= _source.Length) {
            return '\0';
        }

        return _source[_current + 1];
    }

    private bool IsDigit(char c) {
        return c >= '0' && c <= '9';
    }

    private char Peek() {
        if (IsAtEnd()) {
            return '\0';
        }

        return _source[_current];
    }

    private void AddToken(TokenType type) {
        AddToken(type, null);
    }

    private void AddToken(TokenType type, object? literal) {
        var text = _source.Substring(_start, _current - _start);
        _tokens.Add(new Token(type, text, literal, _line, _startColumn));
    }

    private bool IsAtEnd() {
        return _current >= _source.Length;
    }

    private char Advance() {
        _currentColumn++;
        return _source[_current++];
    }

    private bool Match(char expected) {
        if (IsAtEnd()) {
            return false;
        }

        if (_source[_current] != expected) {
            return false;
        }

        _current++;
        return true;
    }

    private void String() {
        while (Peek() != '"' && !IsAtEnd()) {
            if (Peek() == '\n') {
                _line++;
            }

            Advance();
        }

        if (IsAtEnd()) {
            Lox.Error(_line, "unterminated string.");
            return;
        }

        // the closing "
        Advance();

        var value = _source.Substring(_start + 1, _current - _start - 2);
        AddToken(TokenType.STRING, value);
    }
}
