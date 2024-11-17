using System.Text;

namespace NLox;

public class AstPrinter : Expr.Visitor<string>
{
    public string VisitBinaryExpr(Expr.Binary expr)
    {
        return Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);
    }

    public string VisitGroupingExpr(Expr.Grouping expr)
    {
        return Parenthesize("group", expr.Expression);
    }

    public string VisitLiteralExpr(Expr.Literal expr)
    {
        return expr.Value?.ToString() ?? "nil";
    }

    public string VisitUnaryExpr(Expr.Unary expr)
    {
        return Parenthesize(expr.Operator.Lexeme, expr.Right);
    }

    public string VisitConditionalExpr(Expr.Conditional expr)
    {
        return Parenthesize("if", expr.Condition, expr.IfTrue, expr.IfFalse);
    }

    public string Print(Expr expr)
    {
        return expr.Accept(this);
    }

    private string Parenthesize(string name, params Expr[] exprs)
    {
        var builder = new StringBuilder();

        builder.Append('(').Append(name);
        foreach (var expr in exprs) builder.Append(' ').Append(expr.Accept(this));

        builder.Append(')');

        return builder.ToString();
    }
}