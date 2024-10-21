namespace NextR_Compiler.Tokens;

public class NonLiteralToken(TokenType type, int position, string valueString)
	: Token(type, position, valueString);