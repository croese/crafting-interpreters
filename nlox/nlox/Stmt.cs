namespace NLox;

public abstract record Stmt {
  public interface Visitor {
    void VisitExpressionStmt(Expression stmt);
    void VisitIfStmt(If stmt);
    void VisitPrintStmt(Print stmt);
    void VisitVarStmt(Var stmt);
    void VisitBlockStmt(Block stmt);
    void VisitWhileStmt(While stmt);
    void VisitBreakStmt(Break stmt);
  }
  public record Expression(Expr Value) : Stmt {
    public override void Accept(Visitor visitor) {
      visitor.VisitExpressionStmt(this);
    }
  }
  public record If(Expr Condition, Stmt ThenBranch, Stmt? ElseBranch) : Stmt {
    public override void Accept(Visitor visitor) {
      visitor.VisitIfStmt(this);
    }
  }
  public record Print(Expr Value) : Stmt {
    public override void Accept(Visitor visitor) {
      visitor.VisitPrintStmt(this);
    }
  }
  public record Var(Token Name, Expr? Initializer) : Stmt {
    public override void Accept(Visitor visitor) {
      visitor.VisitVarStmt(this);
    }
  }
  public record Block(List<Stmt> Statements) : Stmt {
    public override void Accept(Visitor visitor) {
      visitor.VisitBlockStmt(this);
    }
  }
  public record While(Expr Condition, Stmt Body) : Stmt {
    public override void Accept(Visitor visitor) {
      visitor.VisitWhileStmt(this);
    }
  }
  public record Break() : Stmt {
    public override void Accept(Visitor visitor) {
      visitor.VisitBreakStmt(this);
    }
  }

  public abstract void Accept(Visitor visitor);
}
