using FluentAssertions;

namespace NextR_Compiler.Tests;

public class PreprocessorUnitTests
{
	private string PreprocessCode(string code)
	{
		var codePreprocessor = new Preprocessor.Preprocessor(code);
		return codePreprocessor.GetPreprocessedCode();
	}

	[Fact]
	public void Should_Remove_Single_Line_Comments()
	{
		// Arrange
		string code = "int num = 52;// this is a number";
		string expected = "int num = 52;";

		// Act
		string newCode = PreprocessCode(code);

		// Assert
		newCode.Should().Be(expected);
	}

	[Fact]
	public void Should_Remove_Multi_Line_Comments()
	{
		// Arrange
		string code = "int num = 52;/* this is a \n multiline comment */";
		string expected = "int num = 52;";

		// Act
		string newCode = PreprocessCode(code);

		// Assert
		newCode.Should().Be(expected);
	}

	[Fact]
	public void Should_Ignore_Single_Line_Comments_Inside_String_Literals()
	{
		// Arrange
		string code = "string strWithComment = \"hello world // this is not a comment\";";
		string expected = code;

		// Act
		string newCode = PreprocessCode(code);

		// Assert
		newCode.Should().Be(expected);
	}

	[Fact]
	public void Should_Ignore_Multi_Line_Comments_Inside_String_Literals()
	{
		// Arrange
		string code = "string strWithComment = \"hello /* not a \n comment */\";";
		string expected = code;

		// Act
		string newCode = PreprocessCode(code);

		// Assert
		newCode.Should().Be(expected);
	}
}