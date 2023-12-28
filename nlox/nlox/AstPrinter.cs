using System.Text;

namespace nlox;

public class AstPrinter : ExprVisitor<string> {
    public string VisitAssignExpr(Assign expr) {
        throw new NotImplementedException();
    }

    public string VisitBinaryExpr(Binary expr) {
        return Parenthesize(expr.Operator.Lexeme,
            expr.Left, expr.Right);
    }

    public string VisitGroupingExpr(Grouping expr) {
        return Parenthesize("group", expr.Expression);
    }

    public string VisitLiteralExpr(Literal expr) {
        return expr.Value?.ToString() ?? "nil";
    }

    public string VisitUnaryExpr(Unary expr) {
        return Parenthesize(expr.Operator.Lexeme, expr.Right);
    }

    public string VisitTernaryCondExpr(TernaryCond expr) {
        return Parenthesize("if", expr.Condition, expr.IfTrue, expr.IfFalse);
    }

    public string VisitVariableExpr(Variable expr) {
        throw new NotImplementedException();
    }

    public string Print(Expr expr) {
        return expr.Accept(this);
    }

    private string Parenthesize(string name, params Expr[] exprs) {
        var builder = new StringBuilder();

        builder.Append('(').Append(name);
        foreach (var expr in exprs) {
            builder.Append(' ');
            builder.Append(expr.Accept(this));
        }

        builder.Append(')');

        return builder.ToString();
    }
}
