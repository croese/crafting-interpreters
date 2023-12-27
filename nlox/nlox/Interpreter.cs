namespace nlox;

public class Interpreter : ExprVisitor<object?> {
    public object? VisitBinaryExpr(Binary expr) {
        var left = Evaluate(expr.Left);
        var right = Evaluate(expr.Right);
        switch (expr.Operator.Type) {
            case TokenType.MINUS:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left - (double)right;
            case TokenType.SLASH:
                CheckNumberOperands(expr.Operator, left, right);
                var denom = (double)right;
                if (denom == 0) {
                    throw new RuntimeError(expr.Operator, "denominator cannot equal zero");
                }

                return (double)left / denom;
            case TokenType.STAR:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left * (double)right;
            case TokenType.PLUS:
                return left switch {
                    double ld when right is double rd => ld + rd,
                    double ld when right is string rs => ld + rs,
                    string ls when right is string rs => ls + rs,
                    string ls when right is double rd => ls + rd,
                    _ => throw new RuntimeError(expr.Operator, "operands must be two numbers or strings")
                };

            case TokenType.GREATER:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left > (double)right;
            case TokenType.GREATER_EQUAL:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left >= (double)right;
            case TokenType.LESS:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left < (double)right;
            case TokenType.LESS_EQUAL:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left <= (double)right;
            case TokenType.BANG_EQUAL: return !IsEqual(left, right);
            case TokenType.EQUAL_EQUAL: return IsEqual(left, right);
        }

        return null;
    }

    public object? VisitGroupingExpr(Grouping expr) {
        return Evaluate(expr.Expression);
    }

    public object? VisitLiteralExpr(Literal expr) {
        return expr.Value;
    }

    public object? VisitUnaryExpr(Unary expr) {
        var right = Evaluate(expr.Right);

        switch (expr.Operator.Type) {
            case TokenType.MINUS:
                CheckNumberOperand(expr.Operator, right);
                return -(double)right;
            case TokenType.BANG:
                return !IsTruthy(right);
        }

        return null;
    }

    public object? VisitTernaryCondExpr(TernaryCond expr) {
        var cond = Evaluate(expr.Condition);

        return IsTruthy(cond) ? Evaluate(expr.IfTrue) : Evaluate(expr.IfFalse);
    }

    public void Interpret(Expr expression) {
        try {
            var value = Evaluate(expression);
            Console.WriteLine(Stringify(value));
        } catch (RuntimeError e) {
            Lox.RuntimeError(e);
        }
    }

    private string Stringify(object? value) {
        switch (value) {
            case null:
                return "nil";
            case double d: {
                var text = d.ToString();
                if (text.EndsWith(".0")) {
                    text = text.Substring(0, text.Length - 2);
                }

                return text;
            }
            default:
                return value.ToString();
        }
    }

    private void CheckNumberOperand(Token op, object? operand) {
        if (operand is double) {
            return;
        }

        throw new RuntimeError(op, "operand must be a number");
    }

    private void CheckNumberOperands(Token op, object? left, object? right) {
        if (left is double && right is double) {
            return;
        }

        throw new RuntimeError(op, "operands must be numbers");
    }

    private bool IsEqual(object? a, object? b) {
        return a switch {
            null when b == null => true,
            null => false,
            _ => a.Equals(b)
        };
    }

    private bool IsTruthy(object? o) {
        if (o == null) {
            return false;
        }

        if (o is bool b) {
            return b;
        }

        return true;
    }

    private object? Evaluate(Expr expr) {
        return expr.Accept(this);
    }
}