namespace NextR_Compiler.Tokens;

public class LiteralToken(TokenType type, int position, string valueString, object value)
	: Token(type, position, valueString)
{
	public object Value { get; } = value;
}