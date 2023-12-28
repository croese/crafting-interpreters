namespace nlox;

public class BreakException(Token Break) : Exception {
    public Token Break { get; } = Break;
}

public class Interpreter : ExprVisitor<object?>, StmtVisitor {
    private Environment _environment = new();

    public object? VisitAssignExpr(Assign expr) {
        var value = Evaluate(expr.Value);
        _environment.Assign(expr.Name, value);
        return value;
    }

    public object? VisitBinaryExpr(Binary expr) {
        var left = Evaluate(expr.Left);
        var right = Evaluate(expr.Right);
        switch (expr.Operator.Type) {
            case TokenType.MINUS:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left! - (double)right!;
            case TokenType.SLASH:
                CheckNumberOperands(expr.Operator, left, right);
                var denom = (double)right!;
                if (denom == 0) {
                    throw new RuntimeError(expr.Operator, "denominator cannot equal zero");
                }

                return (double)left! / denom;
            case TokenType.STAR:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left! * (double)right!;
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
                return (double)left! > (double)right!;
            case TokenType.GREATER_EQUAL:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left! >= (double)right!;
            case TokenType.LESS:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left! < (double)right!;
            case TokenType.LESS_EQUAL:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left! <= (double)right!;
            case TokenType.BANG_EQUAL: return !IsEqual(left, right);
            case TokenType.EQUAL_EQUAL: return IsEqual(left, right);
            case TokenType.COMMA:
                var unused = Evaluate(expr.Left);
                return Evaluate(expr.Right);
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
                return -(double)right!;
            case TokenType.BANG:
                return !IsTruthy(right);
        }

        return null;
    }

    public object? VisitTernaryCondExpr(TernaryCond expr) {
        var cond = Evaluate(expr.Condition);

        return IsTruthy(cond) ? Evaluate(expr.IfTrue) : Evaluate(expr.IfFalse);
    }

    public object? VisitVariableExpr(Variable expr) {
        return _environment.Get(expr.Name);
    }

    public object? VisitLogicalExpr(Logical expr) {
        var left = Evaluate(expr.Left);

        if (expr.Operator.Type == TokenType.OR) {
            if (IsTruthy(left)) {
                return left;
            }
        } else {
            if (!IsTruthy(left)) {
                return left;
            }
        }

        return Evaluate(expr.Right);
    }

    public void VisitBlockStmt(Block stmt) {
        ExecuteBlock(stmt.Statements, new Environment(_environment));
    }

    public void VisitExpressionStmt(Expression stmt) {
        Evaluate(stmt.Value);
    }

    public void VisitPrintStmt(Print stmt) {
        var value = Evaluate(stmt.Value);
        Console.WriteLine(Stringify(value));
    }

    public void VisitVarStmt(Var stmt) {
        object? value = null;
        if (stmt.Initializer != null) {
            value = Evaluate(stmt.Initializer);
        }

        _environment.Define(stmt.Name.Lexeme, value);
    }

    public void VisitIfStmt(If stmt) {
        if (IsTruthy(Evaluate(stmt.Condition))) {
            Execute(stmt.ThenBranch);
        } else if (stmt.ElseBranch != null) {
            Execute(stmt.ElseBranch);
        }
    }

    public void VisitWhileStmt(While stmt) {
        while (IsTruthy(Evaluate(stmt.Condition))) {
            try {
                Execute(stmt.Body);
            } catch (BreakException b) {
                break;
            }
        }
    }

    public void VisitBreakStmt(Break stmt) {
        throw new BreakException(stmt.Token);
    }

    private void ExecuteBlock(List<Stmt> statements,
        Environment environment) {
        var previous = _environment;
        try {
            _environment = environment;

            foreach (var stmt in statements) {
                Execute(stmt);
            }
        } finally {
            _environment = previous;
        }
    }

    public void Interpret(List<Stmt> statements) {
        try {
            foreach (var stmt in statements) {
                Execute(stmt);
            }
        } catch (RuntimeError e) {
            Lox.RuntimeError(e);
        } catch (BreakException b) {
            Lox.Error(b.Break, "invalid 'break' used outside of loop");
        }
    }

    private void Execute(Stmt stmt) {
        stmt.Accept(this);
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
