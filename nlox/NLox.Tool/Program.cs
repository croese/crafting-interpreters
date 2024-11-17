if (args.Length != 1)
{
    Console.Error.WriteLine("Usage: generate_ast <output_directory>");
    Environment.Exit(64);
}

var outputDir = args[0];
DefineAst(outputDir, "Expr",
    "Binary : Expr Left, Token Operator, Expr Right",
    "Grouping : Expr Expression",
    "Literal  : object? Value",
    "Unary    : Token Operator, Expr Right",
    "Conditional  : Expr Condition, Expr IfTrue, Expr IfFalse");

void DefineAst(string outputDir, string baseName, params string[] types)
{
    var path = Path.Join(outputDir, baseName + ".cs");
    using var writer = File.CreateText(path);
    writer.WriteLine("namespace NLox;");
    writer.WriteLine();
    writer.WriteLine($"public abstract record {baseName} {{");

    DefineVisitor(writer, baseName, types);

    foreach (var type in types)
    {
        var className = type.Split(":")[0].Trim();
        var fields = type.Split(":")[1].Trim();
        DefineType(writer, baseName, className, fields);
    }

    writer.WriteLine();
    writer.WriteLine("  public abstract R Accept<R>(Visitor<R> visitor);");

    writer.WriteLine("}");
}

void DefineType(StreamWriter writer, string baseName, string className, string fields)
{
    writer.WriteLine($"  public record {className}({fields}) : {baseName} {{");

    writer.WriteLine("    public override R Accept<R>(Visitor<R> visitor) {");
    writer.WriteLine($"      return visitor.Visit{className}{baseName}(this);");
    writer.WriteLine("    }");

    writer.WriteLine("  }");
}

void DefineVisitor(StreamWriter writer, string baseName, string[] types)
{
    writer.WriteLine("  public interface Visitor<R> {");

    foreach (var type in types)
    {
        var typeName = type.Split(":")[0].Trim();
        writer.WriteLine($"    R Visit{typeName}{baseName}({typeName} {baseName.ToLower()});");
    }

    writer.WriteLine("  }");
}