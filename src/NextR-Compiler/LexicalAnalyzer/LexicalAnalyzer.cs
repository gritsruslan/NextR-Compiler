using System.Globalization;
using System.Text;
using NextR_Compiler.Common;
using NextR_Compiler.ExtensionMethods;
using NextR_Compiler.Tokens;

namespace NextR_Compiler.LexicalAnalyzer;

#if DEBUG
public
#else
internal
#endif
class LexicalAnalyzer(string code)
{
	public IReadOnlyList<string> GetDiagnostics() => _diagnostics.AsReadOnly();

	private readonly IList<string> _diagnostics = [];

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
	private char Current => _position < code.Length ? code[_position] : '\0';

	private bool IsSeparator(char chr) => _separators.Contains(chr);

	// Add error to diagnostic if can't convert literal
	private void AddConvertErrorToDiagnostics(string type, string valueString, int position ) =>
		_diagnostics.Add($"ERROR : Can't cast token \"{valueString}\" to type {type} in position {position}");

	// Skip all current whitespaces
	private void SkipWhitespaces()
	{
		while(Current.IsWhitespace())
			Next();
	}

	public List<Token> TokenizeCode()
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

	// Get next token in code
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

	// Tokenize current code token if its string literal (like "hello")
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

				var currentEscapedToken = CheckCurrentEscapeSequence();

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

	// Tokenize current char token if its char literal
	private Option<LiteralToken> TokenizeIfCharLiteral()
	{
		const char singleQuote = '\'';
		const char backSlash = '\\';

		if (Current != singleQuote)
			return Option<LiteralToken>.None;

		var startPosition = _position;
		Next();

		char charValue;
		if (Current == backSlash)
		{
			Next();

			charValue = CheckCurrentEscapeSequence();
		}
		else if (Current != singleQuote)
		{
			charValue = Current;
		}
		else
		{
			_diagnostics.Add($"ERROR: Empty or invalid char literal at position {startPosition}.");
			return Option<LiteralToken>.None;
		}

		Next();

		if (Current != singleQuote)
		{
			Console.WriteLine($"Current : {Current}" );
			_diagnostics.Add($"ERROR: Char literal starting at position {startPosition} is missing a closing quote.");
			return Option<LiteralToken>.None;
		}

		Next();

		var charLiteral = code.Substring(startPosition, _position - startPosition);
		return new LiteralToken(TokenType.CharLiteral, startPosition, charLiteral, charValue);
	}

	private char CheckCurrentEscapeSequence()
	{
		HashSet<char> escapeSequences = ['\n', '\t', '\\', '\'', '\"', '\r', '\0'];

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

		if (!escapeSequences.Contains(currentEscapedToken))
		{
			_diagnostics.Add($"ERROR: Invalid escape sequence '\\{Current}' at position {_position}.");
		}

		return currentEscapedToken;
	}

	// Tokenize current token if its any number literal   (like 100.0f, 52u, 0)
	private Option<LiteralToken> TokenizeIfNumberLiteral()
	{
		const char uintMarker = 'u';
		const char floatMarker = 'f';
		const char dot = '.';

		if (!char.IsDigit(Current))
			return Option<LiteralToken>.None;

		bool hasDot = false;
		bool hasUintMarker = false;
		bool hasFloatMarker = false;

		var valueStringBuilder = new StringBuilder();
		int startPosition = _position;

		while (char.IsDigit(Current) || Current == dot ||
		       Current == uintMarker || Current == floatMarker)
		{
			if (Current == dot)
			{
				if (hasDot)
					break;
				hasDot = true;
			}
			else if (Current == uintMarker)
			{
				hasUintMarker = true;
				valueStringBuilder.Append(uintMarker);
				Next();
				break;
			}
			else if (Current == floatMarker)
			{
				hasFloatMarker = true;
				valueStringBuilder.Append(floatMarker);
				Next();
				break;
			}

			valueStringBuilder.Append(Current);
			Next();
		}

		string valueString = valueStringBuilder.ToString();

		if (!hasFloatMarker && hasDot)
			return TokenizeIfDoubleLiteral(valueString, startPosition);
		if (hasFloatMarker)
			return TokenizeFloatLiteral(valueString, startPosition);
		if (hasUintMarker)
			return TokenizeUintLiteral(valueString, startPosition);

		return TokenizeIntLiteral(valueString, startPosition);
	}

	// Tokenize double token literal (like 100.0)
	private Option<LiteralToken> TokenizeIfDoubleLiteral(string doubleString, int startPosition)
	{
		if(double.TryParse(doubleString,NumberStyles.Float, CultureInfo.InvariantCulture, out var intValue))
			return new LiteralToken(TokenType.DoubleLiteral, startPosition, doubleString, intValue);

		return Option<LiteralToken>.None;
	}

	// Tokenize double token literal (like 100.0f)
	private Option<LiteralToken> TokenizeFloatLiteral(string floatString, int startPosition)
	{
		string floatStringLiteral = floatString;

		if (floatString.EndsWith('f'))
			floatStringLiteral = floatStringLiteral[0..^1];

		if (float.TryParse(floatStringLiteral,NumberStyles.Float, CultureInfo.InvariantCulture, out var floatValue))
			return new LiteralToken(TokenType.FloatLiteral, startPosition, floatString, floatValue);

		AddConvertErrorToDiagnostics("float", floatString, startPosition);
		return Option<LiteralToken>.None;
	}

	// Tokenize uint token literal (like 100u)
	private Option<LiteralToken> TokenizeUintLiteral(string uintString, int startPosition)
	{
		string uintStringLiteral = uintString;

		if (uintString.EndsWith('u'))
			uintStringLiteral = uintStringLiteral[0..^1];

		if (uint.TryParse(uintStringLiteral, out var uintValue))
			return new LiteralToken(TokenType.UintLiteral, startPosition, uintString, uintValue);

		AddConvertErrorToDiagnostics("uint", uintString, startPosition);
		return Option<LiteralToken>.None;
	}

	// Tokenize int token literal (like 100)
	private Option<LiteralToken> TokenizeIntLiteral(string intString, int startPosition)
	{
		if(int.TryParse(intString, out var intValue))
			return new LiteralToken(TokenType.IntLiteral, startPosition, intString, intValue);

		return Option<LiteralToken>.None;
	}

	private Option<NonLiteralToken> TokenizeIfKeyword(string tokenString, int startPosition)
	{
		var tokenType = StaticTokenizer.GetTokenTypeIfKeyword(tokenString);

		if (tokenType.IsSome)
			return new NonLiteralToken(tokenType.Unwrap(), startPosition, tokenString);

		return Option<NonLiteralToken>.None;
	}

	// Tokenize if current token is double operator token (like += -= %=)
	private Option<NonLiteralToken> TokenizeIfDoubleOperator()
	{
		var startPosition = _position;
		var currentChar = Current;

		HashSet<char> operators = ['+','-','*' ,'/', '%' ,'!' , '>' , '<', '='];

		if (!operators.Contains(currentChar)||
		    startPosition + 1 >= code.Length)
			return Option<NonLiteralToken>.None;

		if(!code[startPosition + 1].Equals('='))
			return Option<NonLiteralToken>.None;

		var tokenType = Current switch
		{
			'+' => TokenType.PlusEquals,
			'-' => TokenType.MinusEquals,
			'*' => TokenType.MultiplyEquals,
			'/' => TokenType.DivideEquals,
			'%' => TokenType.RemainderDivEquals,
			'>' => TokenType.GreaterOrEqual,
			'<' => TokenType.LessOrEqual,
			'!' => TokenType.BoolNoEquals,
			'=' => TokenType.BoolEquals,
			_ => throw new Exception("Unexpected operator") // unreachable code
		};

		Next(2);
		return new NonLiteralToken(tokenType, startPosition, currentChar + "=");
	}
}