using System.Text;

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
		string preprocessedCode = RemoveSingleLineComments(sourceCode);
		preprocessedCode = RemoveMultiLineComments(preprocessedCode);

		return preprocessedCode;
	}

	private char GetChar(int position) => position < sourceCode.Length ? sourceCode[position] : '\0';

	//TODO
	private string RemoveSingleLineComments(string code)
	{
		const char slash = '/';
		const char endOfLine = '\n';
		const char doubleQuote = '"';

		var codeSb = new StringBuilder();

		bool isInStringLiteral = false;
		bool isInSingleLineComment = false;

		for (int pos = 0; pos < code.Length; pos++)
		{
			var current = GetChar(pos);
			var next = GetChar(pos + 1);

			if (current == doubleQuote && !isInSingleLineComment &&
			    (pos == 0 || GetChar(pos - 1) != '\\')) // skip if escape character
			{
				isInStringLiteral = !isInStringLiteral;
			}

			if (current == slash && next == slash && !isInStringLiteral)
			{
				isInSingleLineComment = true;
			}

			if (current == endOfLine)
			{
				isInSingleLineComment = false;
			}

			if (!isInSingleLineComment)
			{
				codeSb.Append(current);
			}
		}

		return codeSb.ToString();
	}

	//TODO
	private string RemoveMultiLineComments(string code)
	{
		throw new NotImplementedException();
	}
}
