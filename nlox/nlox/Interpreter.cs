namespace NLox;

public class Interpreter : Expr.Visitor<object?>, Stmt.Visitor
{
    private Environment _environment = new();

    public void VisitExpressionStmt(Stmt.Expression stmt)
    {
        Evaluate(stmt.Value);
    }

    public void VisitIfStmt(Stmt.If stmt)
    {
        if (IsTruthy(Evaluate(stmt.Condition)))
            Execute(stmt.ThenBranch);
        else if (stmt.ElseBranch != null) Execute(stmt.ElseBranch);
    }

    public void VisitPrintStmt(Stmt.Print stmt)
    {
        var value = Evaluate(stmt.Value);
        Console.WriteLine(Stringify(value));
    }

    public void VisitVarStmt(Stmt.Var stmt)
    {
        object? value = null;
        if (stmt.Initializer != null) value = Evaluate(stmt.Initializer);
        _environment.Define(stmt.Name.Lexeme, value);
    }

    public void VisitBlockStmt(Stmt.Block stmt)
    {
        ExecuteBlock(stmt.Statements, new Environment(_environment));
    }

    public void VisitWhileStmt(Stmt.While stmt)
    {
        while (IsTruthy(Evaluate(stmt.Condition)))
            try
            {
                Execute(stmt.Body);
            }
            catch (BreakException)
            {
                break;
            }
    }

    public void VisitBreakStmt(Stmt.Break stmt)
    {
        throw new BreakException();
    }

    public object? VisitBinaryExpr(Expr.Binary expr)
    {
        var left = Evaluate(expr.Left);
        var right = Evaluate(expr.Right);

        switch (expr.Operator.Type)
        {
            case TokenType.MINUS:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left - (double)right;
            case TokenType.SLASH:
                CheckNumberOperands(expr.Operator, left, right);
                if (right is double and 0) throw new RuntimeError(expr.Operator, "Division by zero.");
                return (double)left / (double)right;
            case TokenType.STAR:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left * (double)right;
            case TokenType.PLUS:
                return left switch
                {
                    double ld when right is double rd => ld + rd,
                    string ls when right is string rs => ls + rs,
                    _ => throw new RuntimeError(expr.Operator, "Operands must be two numbers or two strings.")
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
            case TokenType.BANG_EQUAL:
                return !IsEqual(left, right);
            case TokenType.EQUAL_EQUAL:
                return IsEqual(left, right);
            case TokenType.COMMA:
                return right;
        }

        return null;
    }

    public object? VisitGroupingExpr(Expr.Grouping expr)
    {
        return Evaluate(expr.Expression);
    }

    public object? VisitLiteralExpr(Expr.Literal expr)
    {
        return expr.Value;
    }

    public object? VisitLogicalExpr(Expr.Logical expr)
    {
        var left = Evaluate(expr.Left);

        if (expr.Op.Type == TokenType.OR)
        {
            if (IsTruthy(left)) return left;
        }
        else
        {
            if (!IsTruthy(left)) return left;
        }

        return Evaluate(expr.Right);
    }

    public object? VisitUnaryExpr(Expr.Unary expr)
    {
        var right = Evaluate(expr.Right);

        switch (expr.Operator.Type)
        {
            case TokenType.BANG:
                return !IsTruthy(right);
            case TokenType.MINUS:
                CheckNumberOperand(expr.Operator, right);
                return -(double)right;
        }

        return null;
    }

    public object? VisitConditionalExpr(Expr.Conditional expr)
    {
        var condition = Evaluate(expr.Condition);
        return IsTruthy(condition) ? Evaluate(expr.IfTrue) : Evaluate(expr.IfFalse);
    }

    public object? VisitVariableExpr(Expr.Variable expr)
    {
        return _environment.Get(expr.Name);
    }

    public object? VisitAssignExpr(Expr.Assign expr)
    {
        var value = Evaluate(expr.Value);
        _environment.Assign(expr.Name, value);
        return value;
    }

    private void ExecuteBlock(List<Stmt> statements, Environment environment)
    {
        var previous = _environment;
        try
        {
            _environment = environment;

            foreach (var statement in statements) Execute(statement);
        }
        finally
        {
            _environment = previous;
        }
    }

    private void CheckNumberOperands(Token op, object? left, object? right)
    {
        if (left is double && right is double) return;
        throw new RuntimeError(op, "Operands must be numbers.");
    }

    private void CheckNumberOperand(Token op, object? operand)
    {
        if (operand is double) return;
        throw new RuntimeError(op, "Operand must be a number.");
    }

    private bool IsEqual(object? a, object? b)
    {
        return a switch
        {
            null when b == null => true,
            null => false,
            _ => a.Equals(b)
        };
    }

    private bool IsTruthy(object? o)
    {
        return o switch
        {
            null => false,
            bool b => b,
            _ => true
        };
    }

    public void Interpret(List<Stmt> statements)
    {
        try
        {
            foreach (var statement in statements) Execute(statement);
        }
        catch (RuntimeError e)
        {
            Lox.RuntimeError(e);
        }
    }

    private void Execute(Stmt statement)
    {
        statement.Accept(this);
    }

    private string Stringify(object? value)
    {
        switch (value)
        {
            case null:
                return "nil";
            case double:
            {
                var text = value.ToString();
                if (text!.EndsWith(".0")) text = text[..^2];

                return text;
            }
            default:
                return value.ToString()!;
        }
    }

    private object? Evaluate(Expr expr)
    {
        return expr.Accept(this);
    }
}