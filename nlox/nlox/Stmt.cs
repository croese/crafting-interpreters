namespace nlox;

public interface StmtVisitor {
    void VisitBlockStmt(Block stmt);
    void VisitExpressionStmt(Expression stmt);
    void VisitPrintStmt(Print stmt);
    void VisitVarStmt(Var stmt);
    void VisitIfStmt(If stmt);
    void VisitWhileStmt(While stmt);
    void VisitBreakStmt(Break stmt);
}
public abstract record Stmt {
    public abstract void Accept(StmtVisitor visitor);
}
public sealed record Block(List<Stmt> Statements) : Stmt {
    public override void Accept(StmtVisitor visitor) {
        visitor.VisitBlockStmt(this);
    }
}
public sealed record Expression(Expr Value) : Stmt {
    public override void Accept(StmtVisitor visitor) {
        visitor.VisitExpressionStmt(this);
    }
}
public sealed record Print(Expr Value) : Stmt {
    public override void Accept(StmtVisitor visitor) {
        visitor.VisitPrintStmt(this);
    }
}
public sealed record Var(Token Name, Expr? Initializer) : Stmt {
    public override void Accept(StmtVisitor visitor) {
        visitor.VisitVarStmt(this);
    }
}
public sealed record If(Expr Condition, Stmt ThenBranch, Stmt? ElseBranch) : Stmt {
    public override void Accept(StmtVisitor visitor) {
        visitor.VisitIfStmt(this);
    }
}
public sealed record While(Expr Condition, Stmt Body) : Stmt {
    public override void Accept(StmtVisitor visitor) {
        visitor.VisitWhileStmt(this);
    }
}
public sealed record Break(Token Token) : Stmt {
    public override void Accept(StmtVisitor visitor) {
        visitor.VisitBreakStmt(this);
    }
}
