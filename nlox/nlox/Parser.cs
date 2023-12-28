namespace nlox;

internal class ParseError : Exception;

public class Parser(IReadOnlyList<Token> tokens) {
    private int _current;

    public List<Stmt> Parse() {
        var statements = new List<Stmt>();
        while (!IsAtEnd()) {
            var decl = Declaration();
            if (decl != null) {
                statements.Add(decl);
            }
        }

        return statements;
    }

    private Stmt? Declaration() {
        try {
            return Match(TokenType.VAR) ? VarDeclaration() : Statement();
        } catch (ParseError e) {
            Synchronize();
            return null;
        }
    }

    private Stmt VarDeclaration() {
        var name = Consume(TokenType.IDENTIFIER, "expect variable name");

        Expr? initializer = null;
        if (Match(TokenType.EQUAL)) {
            initializer = Expression();
        }

        Consume(TokenType.SEMICOLON, "expect ';' after variable declaration");
        return new Var(name, initializer);
    }

    private Stmt Statement() {
        if (Match(TokenType.PRINT)) {
            return PrintStatement();
        }

        if (Match(TokenType.LEFT_BRACE)) {
            return new Block(Block());
        }

        return ExpressionStatement();
    }

    private List<Stmt> Block() {
        var statements = new List<Stmt>();

        while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd()) {
            var decl = Declaration();
            if (decl != null) {
                statements.Add(decl);
            }
        }

        Consume(TokenType.RIGHT_BRACE, "expect '}' after block");
        return statements;
    }

    private Stmt ExpressionStatement() {
        var value = Expression();
        Consume(TokenType.SEMICOLON, "expect ';' after value");
        return new Expression(value);
    }

    private Stmt PrintStatement() {
        var value = Expression();
        Consume(TokenType.SEMICOLON, "expect ';' after value");
        return new Print(value);
    }

    private Expr Expression() {
        return Comma();
    }

    private Expr Comma() {
        var expr = Assignment();

        while (Match(TokenType.COMMA)) {
            var op = Previous();
            var right = Assignment();
            expr = new Binary(expr, op, right);
        }

        return expr;
    }

    private Expr Assignment() {
        var expr = TernaryCond();

        if (Match(TokenType.EQUAL)) {
            var equals = Previous();
            var value = Assignment();

            if (expr is Variable v) {
                var name = v.Name;
                return new Assign(name, value);
            }

            Error(equals, "invalid assignment target");
        }

        return expr;
    }

    private Expr TernaryCond() {
        var expr = Equality();

        if (Match(TokenType.QUESTION)) {
            var ifTrue = Expression();
            Consume(TokenType.COLON, "missing ':' for ternary conditional.");
            var ifFalse = Expression();
            expr = new TernaryCond(expr, ifTrue, ifFalse);
        }

        return expr;
    }

    private Expr Equality() {
        var expr = Comparison();

        while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL)) {
            var op = Previous();
            var right = Comparison();
            expr = new Binary(expr, op, right);
        }

        return expr;
    }

    private Expr Comparison() {
        var expr = Term();

        while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL)) {
            var op = Previous();
            var right = Term();
            expr = new Binary(expr, op, right);
        }

        return expr;
    }

    private Expr Term() {
        var expr = Factor();

        while (Match(TokenType.MINUS, TokenType.PLUS)) {
            var op = Previous();
            var right = Factor();
            expr = new Binary(expr, op, right);
        }

        return expr;
    }

    private Expr Factor() {
        var expr = Unary();

        while (Match(TokenType.SLASH, TokenType.STAR)) {
            var op = Previous();
            var right = Unary();
            expr = new Binary(expr, op, right);
        }

        return expr;
    }

    private Expr Unary() {
        if (Match(TokenType.BANG, TokenType.MINUS)) {
            var op = Previous();
            var right = Unary();
            return new Unary(op, right);
        }

        return Primary();
    }

    private Expr Primary() {
        if (Match(TokenType.FALSE)) {
            return new Literal(false);
        }

        if (Match(TokenType.TRUE)) {
            return new Literal(true);
        }

        if (Match(TokenType.NIL)) {
            return new Literal(null);
        }

        if (Match(TokenType.NUMBER, TokenType.STRING)) {
            return new Literal(Previous().Literal);
        }

        if (Match(TokenType.IDENTIFIER)) {
            return new Variable(Previous());
        }

        if (Match(TokenType.LEFT_PAREN)) {
            var expr = Expression();
            Consume(TokenType.RIGHT_PAREN, "expect ')' after expression.");
            return new Grouping(expr);
        }

        throw Error(Peek(), "expect expression.");
    }

    private Token Consume(TokenType type, string message) {
        if (Check(type)) {
            return Advance();
        }

        throw Error(Peek(), message);
    }

    private ParseError Error(Token token, string message) {
        Lox.Error(token, message);
        return new ParseError();
    }

    private bool Match(params TokenType[] types) {
        foreach (var type in types) {
            if (Check(type)) {
                Advance();
                return true;
            }
        }

        return false;
    }

    private Token Advance() {
        if (!IsAtEnd()) {
            _current++;
        }

        return Previous();
    }

    private bool Check(TokenType type) {
        if (IsAtEnd()) {
            return false;
        }

        return Peek().Type == type;
    }

    private bool IsAtEnd() {
        return Peek().Type == TokenType.EOF;
    }

    private Token Peek() {
        return tokens[_current];
    }

    private Token Previous() {
        return tokens[_current - 1];
    }

    private void Synchronize() {
        Advance();

        while (!IsAtEnd()) {
            if (Previous().Type == TokenType.SEMICOLON) {
                return;
            }

            switch (Peek().Type) {
                case TokenType.CLASS:
                case TokenType.FUN:
                case TokenType.VAR:
                case TokenType.FOR:
                case TokenType.IF:
                case TokenType.WHILE:
                case TokenType.PRINT:
                case TokenType.RETURN:
                    return;
            }

            Advance();
        }
    }
}
