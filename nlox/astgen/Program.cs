if (args.Length != 1) {
    Console.Error.WriteLine("usage: astgen <output directory>");
    Environment.Exit(64);
}

var outputDir = args[0];

DefineAst(outputDir, true, "Expr",
    "Assign : Token Name, Expr Value",
    "Binary : Expr Left, Token Operator, Expr Right",
    "Grouping : Expr Expression",
    "Literal : object? Value",
    "Unary : Token Operator, Expr Right",
    "TernaryCond : Expr Condition, Expr IfTrue, Expr IfFalse",
    "Variable : Token Name",
    "Logical : Expr Left, Token Operator, Expr Right");

DefineAst(outputDir, false, "Stmt",
    "Block : List<Stmt> Statements",
    "Expression : Expr Value",
    "Print : Expr Value",
    "Var : Token Name, Expr? Initializer",
    "If : Expr Condition, Stmt ThenBranch, Stmt? ElseBranch",
    "While : Expr Condition, Stmt Body",
    "Break : Token Token");

void DefineAst(string outputDir, bool isVisitorGeneric, string baseName, params string[] types) {
    var path = Path.Combine(outputDir, $"{baseName}.cs");
    using (var sw = new StreamWriter(path)) {
        sw.WriteLine("namespace nlox;");
        sw.WriteLine();

        DefineVisitor(sw, isVisitorGeneric, baseName, types);


        sw.WriteLine($"public abstract record {baseName} {{");
        sw.WriteLine(isVisitorGeneric
            ? $"    public abstract R Accept<R>({baseName}Visitor<R> visitor);"
            : $"    public abstract void Accept({baseName}Visitor visitor);");

        sw.WriteLine("}");


        foreach (var type in types) {
            var className = type.Split(':')[0].Trim();
            var fields = type.Split(':')[1].Trim();
            sw.WriteLine($"public sealed record {className}({fields}) : {baseName} {{");
            sw.WriteLine(isVisitorGeneric
                ? $"    public override R Accept<R>({baseName}Visitor<R> visitor) {{"
                : $"    public override void Accept({baseName}Visitor visitor) {{");
            sw.WriteLine(isVisitorGeneric
                ? $"        return visitor.Visit{className}{baseName}(this);"
                : $"        visitor.Visit{className}{baseName}(this);");
            sw.WriteLine("    }");
            sw.WriteLine("}");
        }
    }
}

void DefineVisitor(StreamWriter sw, bool isGeneric, string baseName, string[] types) {
    sw.WriteLine(isGeneric
        ? $"public interface {baseName}Visitor<R> {{"
        : $"public interface {baseName}Visitor {{");
    foreach (var type in types) {
        var typeName = type.Split(':')[0].Trim();
        sw.WriteLine(isGeneric
            ? $"    R Visit{typeName}{baseName}({typeName} {baseName.ToLower()});"
            : $"    void Visit{typeName}{baseName}({typeName} {baseName.ToLower()});");
    }

    sw.WriteLine("}");
}
