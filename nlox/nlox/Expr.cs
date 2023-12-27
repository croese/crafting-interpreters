namespace nlox;

public interface ExprVisitor<R> {
    R VisitBinaryExpr(Binary expr);
    R VisitGroupingExpr(Grouping expr);
    R VisitLiteralExpr(Literal expr);
    R VisitUnaryExpr(Unary expr);
    R VisitTernaryCondExpr(TernaryCond expr);
}
public abstract record Expr {
    public abstract R Accept<R>(ExprVisitor<R> visitor);
}
public sealed record Binary(Expr Left, Token Operator, Expr Right) : Expr {
    public override R Accept<R>(ExprVisitor<R> visitor) {
        return visitor.VisitBinaryExpr(this);
    }
}
public sealed record Grouping(Expr Expression) : Expr {
    public override R Accept<R>(ExprVisitor<R> visitor) {
        return visitor.VisitGroupingExpr(this);
    }
}
public sealed record Literal(object? Value) : Expr {
    public override R Accept<R>(ExprVisitor<R> visitor) {
        return visitor.VisitLiteralExpr(this);
    }
}
public sealed record Unary(Token Operator, Expr Right) : Expr {
    public override R Accept<R>(ExprVisitor<R> visitor) {
        return visitor.VisitUnaryExpr(this);
    }
}
public sealed record TernaryCond(Expr Condition, Expr IfTrue, Expr IfFalse) : Expr {
    public override R Accept<R>(ExprVisitor<R> visitor) {
        return visitor.VisitTernaryCondExpr(this);
    }
}
