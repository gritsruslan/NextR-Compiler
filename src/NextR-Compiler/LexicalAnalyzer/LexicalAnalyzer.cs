using System.Text;
using NextR_Compiler.Common;
using NextR_Compiler.ExtensionMethods;
using NextR_Compiler.Tokens;

namespace NextR_Compiler.LexicalAnalyzer;

public class LexicalAnalyzer(string code)
{
	private readonly string _code = code;

	public IReadOnlyList<string> GetDiagnostics() => _diagnostics.AsReadOnly();

	private IList<string> _diagnostics = [];

	// Index of current position in code
	private int _position;

	private readonly HashSet<char> _separators =
	[
		':', ';', ',', '.', '{', '}', '(', ')', '[', ']', '*', '/', '+',
		'-', '=', '!', '<', '>', '%', '\'', '"', '\\', ' ', '\r', '\n'
	];

	// Next char
	private void Next() => _position++;

	// Next char
	private void Next(int count)
	{
		for(int i = 0; i < count; i++)
			Next();
	}

	//Returns current researched char in source code
	private char Current => _position < _code.Length ? _code[_position] : '\0';

	private bool IsSeparator(char chr) => _separators.Contains(chr);

	// Add error to diagnostic if can't convert literal
	private void AddConvertErrorToDiagnostics(string type, string valueString, int position ) =>
		_diagnostics.Add($"ERROR : Can't cast token \"{valueString}\" to type {type} in position {position}");

	private void SkipWhitespaces()
	{
		while(Current.IsWhitespace())
			Next();
	}

	public IEnumerable<Token> TokenizeCode()
	{
		var resultTokens = new List<Token>();

		while (true)
		{
			var token = NextToken();

			if (token.Type == TokenType.EndOfFile)
				break;

			resultTokens.Add(token);

			SkipWhitespaces();
		}

		return resultTokens;
	}

	private Token NextToken()
	{
		SkipWhitespaces();

		if (Current == '\0')
			return new NonLiteralToken(TokenType.EndOfFile, Current, '\0'.ToString());


		//If current token is string literal (like "Hello, world!")
		var stringTokenOpt = TokenizeIfStringLiteral();
		if (stringTokenOpt.IsSome)
			return stringTokenOpt.Unwrap();

		//If current token is char literal (like 'n', '\t')
		var charTokenOpt = TokenizeIfCharLiteral();
		if (charTokenOpt.IsSome)
			return charTokenOpt.Unwrap();

		//If current token is double operator (like "+=" "-=" etc)
		var doubleOperatorOpt = TokenizeIfDoubleOperator();
		if (doubleOperatorOpt.IsSome)
			return doubleOperatorOpt.Unwrap();

		//If current token is any number token (int, uint or float)
		var numberTokenOpt = TokenizeIfNumberLiteral();
		if (numberTokenOpt.IsSome)
			return numberTokenOpt.Unwrap();

		//If current token is solo separator (like '+', '-', '=', '*', '{', '}' etc)
		if (IsSeparator(Current))
		{
			var separator = Current;
			var separatorPosition = _position;
			var separatorTokenType = StaticTokenizer.GetTokenTypeIfSeparator(separator);
			Next();
			return new NonLiteralToken(separatorTokenType, separatorPosition, separator.ToString());
		}

		var tokenString = string.Empty;
		var startPosition = _position;

		while (!IsSeparator(Current) && Current != '\0')
		{
			tokenString += Current;
			Next();
		}

		//if token is keyword
		var keywordTokenOpt = TokenizeIfKeyword(tokenString, startPosition);
		if (keywordTokenOpt.IsSome)
			return keywordTokenOpt.Unwrap();

		Next();

		//other tokens marked as identifiers
		return new NonLiteralToken(TokenType.Identifier, startPosition, tokenString);
	}

	private Option<LiteralToken> TokenizeIfStringLiteral()
	{
		const char doubleQuote = '"';
		const char backSlash = '\\';

		if (Current != doubleQuote)
			return Option<LiteralToken>.None;

		var strTokenSb = new StringBuilder();
		var startPosition = _position;

		Next();

		while (Current != '\0' && Current != '\n' && Current != doubleQuote)
		{
			if (Current == backSlash)
			{
				Next();

				var currentEscapedToken = Current switch
				{
					'n' => '\n',
					't' => '\t',
					'r' => '\r',
					'0' => '\0',
					'\\' => '\\',
					'\'' => '\'',
					'\"' => '\"',
					_ => Current
				};

				if (currentEscapedToken is not ('\n' or '\t' or '\\' or  '\'' or '\"' or '\r' or '\0'))
				{
					_diagnostics.Add($"ERROR: Invalid escape sequence '\\{Current}' at position {_position}.");
				}

				strTokenSb.Append(currentEscapedToken);

				Next();
				continue;
			}

			strTokenSb.Append(Current);
			Next();
		}

		if (Current is '\0' or '\n')
		{
			_diagnostics.Add($"ERROR: String token at position {startPosition} has no closing double quote!");
			return Option<LiteralToken>.None;
		}

		Next();

		var strTokenString = strTokenSb.ToString();
		return new LiteralToken(TokenType.StringLiteral, startPosition, strTokenString, strTokenString);
	}

	private Option<LiteralToken> TokenizeIfCharLiteral()
	{

	}

	private Option<LiteralToken> TokenizeIfNumberLiteral()
	{

	}

	/*
	private Option<LiteralToken> TokenizeIfDoubleLiteral()
	{ }
	*/

	private Option<LiteralToken> TokenizeIfFloatLiteral()
	{

	}

	private Option<LiteralToken> TokenizeIfUintLiteral()
	{

	}

	private Option<LiteralToken> TokenizeIfIntLiteral()
	{

	}

	private Option<NonLiteralToken> TokenizeIfKeyword(string tokenString, int startPosition)
	{

	}

	private Option<NonLiteralToken> TokenizeIfDoubleOperator()
	{

	}
}