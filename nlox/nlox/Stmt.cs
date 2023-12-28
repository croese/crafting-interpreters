namespace nlox;

public interface StmtVisitor {
    void VisitBlockStmt(Block stmt);
    void VisitExpressionStmt(Expression stmt);
    void VisitPrintStmt(Print stmt);
    void VisitVarStmt(Var stmt);
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
