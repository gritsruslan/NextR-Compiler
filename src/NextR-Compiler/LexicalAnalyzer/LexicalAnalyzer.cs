using NextR_Compiler.Common;
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

	public IEnumerable<Token> TokenizeCode()
	{

	}

	private Token NextToken()
	{

	}

	private Option<LiteralToken> TokenizeIfStringLiteral()
	{

	}

	private Option<LiteralToken> TokenizeIfCharLiteral()
	{

	}

	private Option<LiteralToken> TokenizeIfNumberLiteral()
	{

	}

	private Option<LiteralToken> TokenizeIfDoubleLiteral()
	{

	}

	private Option<LiteralToken> TokenizeIfFloatLiteral()
	{

	}

	private Option<LiteralToken> TokenizeIfUintLiteral()
	{

	}

	private Option<LiteralToken> TokenizeIfIntLiteral()
	{

	}

	private Option<NonLiteralToken> TokenizeIfKeyword()
	{

	}

	private Option<NonLiteralToken> TokenizeIfDoubleOperator()
	{

	}
}