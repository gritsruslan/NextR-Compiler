using FluentAssertions;
using NextR_Compiler.LexicalAnalyzer;
using NextR_Compiler.Tokens;
using Xunit.Abstractions;

namespace NextR_Compiler.Tests;

public class LexicalAnalyzerUnitTests
{
	private readonly ITestOutputHelper _testOutputHelper;

	public LexicalAnalyzerUnitTests(ITestOutputHelper testOutputHelper)
	{
		_testOutputHelper = testOutputHelper;
	}

	private (List<Token> tokens, IReadOnlyList<string> diagnostics) AnalyzeCode(string code)
	{
		var lexicalAnalyzer = new LexicalAnalyzer.LexicalAnalyzer(code);
		return (lexicalAnalyzer.TokenizeCode(), lexicalAnalyzer.GetDiagnostics());
	}

	[Theory]
	[MemberData(nameof(GetStringLiteralsData))]
	public void Should_Correct_Recognize_String_Literals(string stringString, string valueShouldBe)
	{
		//Act
		var (listTokens, diagnostics) = AnalyzeCode(stringString);

		//Assert
		listTokens.Count.Should().Be(1);

		var stringToken = listTokens.First() as LiteralToken;

		stringToken.Should().NotBeNull();
		diagnostics.Should().BeEmpty();
		stringToken!.Type.Should().Be(TokenType.StringLiteral);
		stringToken!.Value.Should().Be(valueShouldBe);
	}


	public static IEnumerable<object[]> GetStringLiteralsData()
	{
		yield return ["\"\"", ""];
		yield return ["\"Hello, world! \"", "Hello, world! "];
		yield return ["\"string // comment\"", "string // comment"];
		yield return ["\"string /* comment */\"", "string /* comment */"];
		yield return ["\"hello world! sep \\\" \"", "hello world! sep \" "];
		yield return ["\"Put the phone down!\\n\"", "Put the phone down!\n"];
		yield return ["\"This is for my safety!\\t\"", "This is for my safety!\t"];
		yield return ["\"Put the phone down!\\r\"", "Put the phone down!\r"];
		yield return ["\"This is for my safety!\\0\"", "This is for my safety!\0"];
	}


	[Theory]
	[MemberData(nameof(GetCharLiterals))]
	public void Should_Correct_Recognize_Char_Literals(string charCode, char value)
	{
		//Act
		var (listTokens, diagnostics) = AnalyzeCode(charCode);

		//Assert
		listTokens.Count.Should().Be(1);

		var charToken = listTokens.First() as LiteralToken;

		charToken.Should().NotBeNull();
		diagnostics.Should().BeEmpty();
		charToken!.ValueString.Should().Be(charCode);
		charToken!.Value.Should().Be(value);
	}

	public static IEnumerable<object[]> GetCharLiterals()
	{
		yield return ["\'h\'", 'h'];
		yield return ["\'3\'", '3'];
		yield return ["\'+\'", '+'];
		yield return ["\')\'", ')'];
		yield return ["\'0\'", '0'];
		yield return ["\'\\n\'", '\n'];
		yield return ["\'\\r\'", '\r'];
		yield return ["\'\\t\'", '\t'];
		yield return ["\'\\0\'", '\0']; //падает
	}

	[Theory]
	[MemberData(nameof(GetIntLiterals))]
	public void Should_Correct_Recognize_Int_Literals(string intString, int value)
	{
		//Act
		var (listTokens, diagnostics) = AnalyzeCode(intString);

		//Assert
		listTokens.Count.Should().Be(1);

		var token = listTokens.First() as LiteralToken;

		token.Should().NotBeNull();
		diagnostics.Should().BeEmpty();
		token!.Type.Should().Be(TokenType.IntLiteral);
		token!.Value.Should().Be(value);
	}

	public static IEnumerable<object[]> GetIntLiterals()
	{
		yield return ["100", 100];
		yield return ["9999999", 9999999];
		yield return ["52", 52];
		yield return ["0", 0];
	}


	[Theory]
	[MemberData(nameof(GetFloatLiterals))]
	public void Should_Correct_Recognize_Float_Literals(string floatString, float value)
	{
		//Act
		var (listTokens, diagnostics) = AnalyzeCode(floatString);

		//Assert
		listTokens.Count.Should().Be(1);

		var token = listTokens.First() as LiteralToken;

		token.Should().NotBeNull();
		diagnostics.Should().BeEmpty();
		token!.Type.Should().Be(TokenType.FloatLiteral);
		token!.Value.Should().Be(value);
	}

	public static IEnumerable<object[]> GetFloatLiterals()
	{
		yield return ["200f", 200.0];
		yield return ["52.05f", 52.05];
		yield return ["0f", 0.0];
	}

	[Theory]
	[MemberData(nameof(GetDoubleLiterals))]
	public void Should_Correct_Recognize_Double_Literals(string doubleString, double value)
	{
		var (listTokens, diagnostics) = AnalyzeCode(doubleString);

		listTokens.Count.Should().Be(1);

		var token = listTokens.First() as LiteralToken;

		diagnostics.Should().BeEmpty();
		token!.Type.Should().Be(TokenType.DoubleLiteral);
		token!.Value.Should().Be(value);
	}

	public static IEnumerable<object[]> GetDoubleLiterals()
	{
		yield return ["0.0", 0.0];
		yield return ["152.0", 152.0];
		yield return ["52.52", 52.52];
		yield return ["9999999.9999999", 9999999.9999999];
	}

	[Fact]
	public void Should_Correct_Recognize_Uint_Literals()
	{
		var uintString = "100u";

		var (listTokens, diagnostics) = AnalyzeCode(uintString);

		//Assert
		listTokens.Count.Should().Be(1);

		var token = listTokens.First() as LiteralToken;

		token.Should().NotBeNull();
		diagnostics.Should().BeEmpty();
		token!.Type.Should().Be(TokenType.UintLiteral);
		token!.Value.Should().Be(100);
	}

	[Theory]
	[MemberData(nameof(GetDoubleOperatorsData))]
	public void Should_Correct_Recognize_Double_Operators(string doubleOperatorString, TokenType type)
	{
		//Act
		var (listTokens, diagnostics) = AnalyzeCode(doubleOperatorString);

		//Assert
		diagnostics.Count.Should().Be(0);
		listTokens.Count.Should().Be(1);
		listTokens.First().Type.Should().Be(type);
		listTokens.First().ValueString.Should().Be(doubleOperatorString);
	}

	public static IEnumerable<object[]> GetDoubleOperatorsData()
	{
		return new object[][]
		{
			["+=", TokenType.PlusEquals],
			["-=", TokenType.MinusEquals],
			["*=", TokenType.MultiplyEquals],
			["/=", TokenType.DivideEquals],
			["%=", TokenType.RemainderDivEquals],
			["<=", TokenType.LessOrEqual],
			[">=", TokenType.GreaterOrEqual],
			["==", TokenType.BoolEquals],
			["!=", TokenType.BoolNoEquals]
		};
	}

	[Theory]
	[MemberData(nameof(GetSeparatorsData))]
	public void Should_Correct_Recognize_Separators(char separator, TokenType tokenType)
	{
		//Act
		var (listTokens, diagnostics) = AnalyzeCode(separator.ToString());

		//Assert
		diagnostics.Count.Should().Be(0);
		listTokens.Count.Should().Be(1);
		listTokens.First().Type.Should().Be(tokenType);
		listTokens.First().ValueString.Should().Be(separator.ToString());
	}

	public static IEnumerable<object[]> GetSeparatorsData()
	{
		return StaticTokenizer.SeparatorsTypesDictionary.Select(kvp =>
			new object[] { kvp.Key, kvp.Value });
	}

	[Theory]
	[MemberData(nameof(GetKeywordData))]
	public void Should_Correct_Recognize_Keywords(string keyword, TokenType expectedType)
	{
		//Act
		var (listTokens, diagnostics) = AnalyzeCode(keyword);

		//Assert
		diagnostics.Count.Should().Be(0);
		listTokens.Count.Should().Be(1);
		listTokens.First().Type.Should().Be(expectedType);
		listTokens.First().ValueString.Should().Be(keyword);
	}

	public static IEnumerable<object[]> GetKeywordData()
	{
		return StaticTokenizer.KeywordsTypesDictionary.Select(kvp =>
			new object[] { kvp.Key, kvp.Value });
	}

	[Theory]
	[MemberData(nameof(GetWhitespacesData))]
	public void Should_Ignore_Whitespaces(char whitespace)
	{
		//Act
		var (listTokens, diagnostics) = AnalyzeCode(whitespace.ToString());

		//Assert
		diagnostics.Count.Should().Be(0);
		listTokens.Count.Should().Be(0);
	}

	public static IEnumerable<object[]> GetWhitespacesData()
	{
		yield return ['\n'];
		yield return ['\t'];
		yield return ['\r'];
		yield return [' '];
	}
}