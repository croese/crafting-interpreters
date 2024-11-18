namespace NLox;

public class RuntimeError(Token token, string message) : Exception(message)
{
    public Token Token { get; } = token;
}