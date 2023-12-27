namespace nlox;

internal class ParseError : Exception;

public class Parser(IReadOnlyList<Token> tokens) {
    private int _current;

    public Expr? Parse() {
        try {
            return Expression();
        }
        catch (ParseError e) {
            return null;
        }
    }

    private Expr Expression() {
        return Comma();
    }

    private Expr Comma() {
        var expr = TernaryCond();

        while (Match(TokenType.COMMA)) {
            var op = Previous();
            var right = TernaryCond();
            expr = new Binary(expr, op, right);
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
