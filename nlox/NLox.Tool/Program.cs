if (args.Length != 1)
{
    Console.Error.WriteLine("Usage: generate_ast <output_directory>");
    Environment.Exit(64);
}

var outputDir = args[0];
DefineAst(outputDir, true, "Expr",
    "Binary : Expr Left, Token Operator, Expr Right",
    "Grouping : Expr Expression",
    "Literal  : object? Value",
    "Logical : Expr Left, Token Op, Expr Right",
    "Unary    : Token Operator, Expr Right",
    "Conditional  : Expr Condition, Expr IfTrue, Expr IfFalse",
    "Variable : Token Name",
    "Assign : Token Name, Expr Value");

DefineAst(outputDir, false, "Stmt",
    "Expression : Expr Value",
    "If : Expr Condition, Stmt ThenBranch, Stmt? ElseBranch",
    "Print : Expr Value",
    "Var : Token Name, Expr? Initializer",
    "Block : List<Stmt> Statements",
    "While : Expr Condition, Stmt Body",
    "Break : ");

void DefineAst(string outputDir, bool isVisitorGeneric, string baseName, params string[] types)
{
    var path = Path.Join(outputDir, baseName + ".cs");
    using var writer = File.CreateText(path);
    writer.WriteLine("namespace NLox;");
    writer.WriteLine();
    writer.WriteLine($"public abstract record {baseName} {{");

    DefineVisitor(writer, isVisitorGeneric, baseName, types);

    foreach (var type in types)
    {
        var className = type.Split(":")[0].Trim();
        var fields = type.Split(":")[1].Trim();
        DefineType(writer, isVisitorGeneric, baseName, className, fields);
    }

    writer.WriteLine();
    writer.WriteLine(isVisitorGeneric
        ? "  public abstract R Accept<R>(Visitor<R> visitor);"
        : "  public abstract void Accept(Visitor visitor);");

    writer.WriteLine("}");
}

void DefineType(StreamWriter writer, bool isVisitorGeneric, string baseName, string className, string fields)
{
    writer.WriteLine($"  public record {className}({fields}) : {baseName} {{");

    writer.WriteLine(isVisitorGeneric
        ? "    public override R Accept<R>(Visitor<R> visitor) {"
        : "    public override void Accept(Visitor visitor) {");
    writer.WriteLine(isVisitorGeneric
        ? $"      return visitor.Visit{className}{baseName}(this);"
        : $"      visitor.Visit{className}{baseName}(this);");
    writer.WriteLine("    }");

    writer.WriteLine("  }");
}

void DefineVisitor(StreamWriter writer, bool isGeneric, string baseName, string[] types)
{
    writer.WriteLine(isGeneric ? "  public interface Visitor<R> {" : "  public interface Visitor {");

    foreach (var type in types)
    {
        var typeName = type.Split(":")[0].Trim();
        writer.WriteLine(isGeneric
            ? $"    R Visit{typeName}{baseName}({typeName} {baseName.ToLower()});"
            : $"    void Visit{typeName}{baseName}({typeName} {baseName.ToLower()});");
    }

    writer.WriteLine("  }");
}