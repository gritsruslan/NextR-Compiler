namespace NextR_Compiler.Preprocessor;

#if DEBUG
	public
#else
	internal
#endif
class Preprocessor(string sourceCode)
{

	public string GetPreprocessedCode()
	{
		return sourceCode;
	}

	private char GetChar(int position) => position < sourceCode.Length ? sourceCode[position] : '\0';

	//TODO
	private string RemoveSingleLineComments(string code)
	{
		throw new NotImplementedException();
	}

	//TODO
	private string RemoveMultiLineComments(string code)
	{
		throw new NotImplementedException();
	}
}
