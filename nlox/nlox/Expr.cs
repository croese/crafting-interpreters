namespace NLox;

public abstract record Expr {
  public interface Visitor<R> {
    R VisitBinaryExpr(Binary expr);
    R VisitGroupingExpr(Grouping expr);
    R VisitLiteralExpr(Literal expr);
    R VisitUnaryExpr(Unary expr);
    R VisitConditionalExpr(Conditional expr);
  }
  public record Binary(Expr Left, Token Operator, Expr Right) : Expr {
    public override R Accept<R>(Visitor<R> visitor) {
      return visitor.VisitBinaryExpr(this);
    }
  }
  public record Grouping(Expr Expression) : Expr {
    public override R Accept<R>(Visitor<R> visitor) {
      return visitor.VisitGroupingExpr(this);
    }
  }
  public record Literal(object? Value) : Expr {
    public override R Accept<R>(Visitor<R> visitor) {
      return visitor.VisitLiteralExpr(this);
    }
  }
  public record Unary(Token Operator, Expr Right) : Expr {
    public override R Accept<R>(Visitor<R> visitor) {
      return visitor.VisitUnaryExpr(this);
    }
  }
  public record Conditional(Expr Condition, Expr IfTrue, Expr IfFalse) : Expr {
    public override R Accept<R>(Visitor<R> visitor) {
      return visitor.VisitConditionalExpr(this);
    }
  }

  public abstract R Accept<R>(Visitor<R> visitor);
}
