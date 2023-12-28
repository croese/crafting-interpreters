namespace nlox;

public class Lox {
    private static readonly Interpreter Interpreter = new();
    public static bool HadError { get; private set; }
    public static bool HadRuntimeError { get; private set; }

    public static void Main(string[] args) {
        switch (args.Length) {
            case > 1:
                Console.WriteLine("usage: nlox [script]");
                System.Environment.Exit(64);
                break;
            case 1:
                RunFile(args[0]);
                break;
            default:
                RunPrompt();
                break;
        }
    }

    private static void RunPrompt() {
        while (true) {
            Console.Write("> ");
            var line = Console.ReadLine();
            if (line == null) {
                break;
            }

            Run(line);
            HadError = false;
        }
    }

    private static void RunFile(string path) {
        var source = File.ReadAllText(path);
        Run(source);

        if (HadError) {
            System.Environment.Exit(65);
        }

        if (HadRuntimeError) {
            System.Environment.Exit(70);
        }
    }

    private static void Run(string source) {
        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens();

        var parser = new Parser(tokens);
        var statements = parser.Parse();

        if (HadError) {
            return;
        }

        //Console.WriteLine(new AstPrinter().Print(expr!));
        Interpreter.Interpret(statements!);
    }

    public static void Error(int line, string message) {
        Report(line, string.Empty, message);
    }

    public static void Error(Token token, string message) {
        if (token.Type == TokenType.EOF) {
            Report(token.Line, " at end", message);
        } else {
            Report(token.Line, $" at '{token.Lexeme}'", message);
        }
    }

    private static void Report(int line, string where, string message) {
        Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
        HadError = true;
    }

    public static void ClearError() {
        HadError = false;
    }

    public static void RuntimeError(RuntimeError error) {
        Console.Error.WriteLine($"{error.Message}\n[line {error.Token.Line}]");
        HadRuntimeError = true;
    }
}
