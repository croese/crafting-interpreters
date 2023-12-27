if (args.Length != 1) {
    Console.Error.WriteLine("usage: astgen <output directory>");
    Environment.Exit(64);
}

var outputDir = args[0];

DefineAst(outputDir, "Expr",
    "Binary : Expr Left, Token Operator, Expr Right",
    "Grouping : Expr Expression",
    "Literal : object? Value",
    "Unary : Token Operator, Expr Right",
    "TernaryCond : Expr Condition, Expr IfTrue, Expr IfFalse");

void DefineAst(string outputDir, string baseName, params string[] types) {
    var path = Path.Combine(outputDir, $"{baseName}.cs");
    using (var sw = new StreamWriter(path)) {
        sw.WriteLine("namespace nlox;");
        sw.WriteLine();

        DefineVisitor(sw, baseName, types);

        sw.WriteLine($"public abstract record {baseName} {{");
        sw.WriteLine($"    public abstract R Accept<R>({baseName}Visitor<R> visitor);");
        sw.WriteLine("}");

        foreach (var type in types) {
            var className = type.Split(':')[0].Trim();
            var fields = type.Split(':')[1].Trim();
            sw.WriteLine($"public sealed record {className}({fields}) : {baseName} {{");
            sw.WriteLine($"    public override R Accept<R>({baseName}Visitor<R> visitor) {{");
            sw.WriteLine($"        return visitor.Visit{className}{baseName}(this);");
            sw.WriteLine("    }");
            sw.WriteLine("}");
        }
    }
}

void DefineVisitor(StreamWriter sw, string baseName, string[] types) {
    sw.WriteLine($"public interface {baseName}Visitor<R> {{");
    foreach (var type in types) {
        var typeName = type.Split(':')[0].Trim();
        sw.WriteLine($"    R Visit{typeName}{baseName}({typeName} {baseName.ToLower()});");
    }

    sw.WriteLine("}");
}
