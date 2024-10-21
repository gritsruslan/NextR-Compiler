using System.Text;

namespace NextR_Compiler.Preprocessor;

/// <summary>
/// A basic preprocessor for code that removes single-line and multi-line comments.
/// </summary>
/// <param name="sourceCode">Source code</param>
#if DEBUG
	public
#else
	internal
#endif
class Preprocessor(string sourceCode)
{
	/// <summary>
	/// Preprocess source code
	/// </summary>
	/// <returns>Preprocessed code</returns>
	public string GetPreprocessedCode()
	{
		string preprocessedCode = RemoveMultiLineComments(sourceCode);
		preprocessedCode = RemoveSingleLineComments(preprocessedCode);

		return preprocessedCode;
	}

	//Returns current researched char in source code
	private char GetChar(int position) => position < sourceCode.Length ? sourceCode[position] : '\0';

	//Removes single-line comments from code
	private string RemoveSingleLineComments(string code)
	{
		const char slash = '/';
		const char endOfLine = '\n';
		const char doubleQuote = '"';

		var codeSb = new StringBuilder(); // preprocessed code StringBuilder

		bool isInStringLiteral = false;
		bool isInSingleLineComment = false;

		for (int pos = 0; pos < code.Length; pos++)
		{
			var current = GetChar(pos);
			var next = GetChar(pos + 1);

			//If in string literal (ignore all comments)
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

	//Removes multi-line comments from code
	private string RemoveMultiLineComments(string code)
	{
		const char slash = '/';
		const char doubleQuote = '"';
		const char star = '*';

		var codeSb = new StringBuilder(); // preprocessed code StringBuilder

		bool isInMultiLineComment = false;
		bool isInStringLiteral = false;

		for (int pos = 0; pos < code.Length; pos++)
		{
			var current = GetChar(pos);
			var next = GetChar(pos + 1);

			//If in string literal (ignore all comments)
			if (current == doubleQuote && !isInMultiLineComment &&
			    (pos == 0 || code[pos - 1] != '\\')) // skip if escape character
				isInStringLiteral = !isInStringLiteral;

			// start of multi-line comment
			if (!isInMultiLineComment && !isInStringLiteral && current == slash && next == star)
			{
				pos++;
				isInMultiLineComment = true;
				continue;
			}

			// end of multi-line comment
			if (isInMultiLineComment && !isInStringLiteral && current == star && next == slash)
			{
				pos++;
				isInMultiLineComment = false;
				continue;
			}

			if(!isInMultiLineComment)
				codeSb.Append(current);
		}

		return codeSb.ToString();
	}
}
