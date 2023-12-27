namespace nlox;

internal class Lox {
    private static bool HadError;

    public static void Main(string[] args) {
        switch (args.Length) {
            case > 1:
                Console.WriteLine("usage: nlox [script]");
                Environment.Exit(64);
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
            Environment.Exit(65);
        }
    }

    private static void Run(string source) {
        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens();

        foreach (var token in tokens) {
            Console.WriteLine(token);
        }
    }

    public static void Error(int line, string message) {
        Report(line, string.Empty, message);
    }

    public static void Report(int line, string where, string message) {
        Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
        HadError = true;
    }
}
