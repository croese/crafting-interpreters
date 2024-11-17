// namespace NLox;
//
// public class RpnPrinter : Expr.Visitor<string>
// {
//     public string VisitBinaryExpr(Expr.Binary expr)
//     {
//         return $"{expr.Left.Accept(this)} {expr.Right.Accept(this)} {expr.Operator.Lexeme}";
//     }
//
//     public string VisitGroupingExpr(Expr.Grouping expr)
//     {
//         return expr.Expression.Accept(this);
//     }
//
//     public string VisitLiteralExpr(Expr.Literal expr)
//     {
//         return expr.Value?.ToString() ?? "nil";
//     }
//
//     public string VisitUnaryExpr(Expr.Unary expr)
//     {
//         return $"{expr.Right.Accept(this)} {expr.Operator.Lexeme}";
//     }
//
//     public string Print(Expr expr)
//     {
//         return expr.Accept(this);
//     }
// }

