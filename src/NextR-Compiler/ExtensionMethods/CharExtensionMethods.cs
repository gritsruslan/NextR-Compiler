namespace NextR_Compiler.ExtensionMethods;

public static class CharExtensionMethods
{
	public static bool IsSpace(this char chr) => chr.Equals(' ');

	public static bool IsWhitespace(this char chr) => chr is ' ' or '\t' or '\n' or '\r';
}