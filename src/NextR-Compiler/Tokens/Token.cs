namespace NextR_Compiler.Tokens;

public abstract class Token(TokenType type, int position, string valueString)
{
	public TokenType Type => type;
	public int Position => position;
	public string ValueString => valueString;
}