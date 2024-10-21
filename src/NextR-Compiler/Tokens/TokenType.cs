namespace NextR_Compiler.Tokens;

public enum TokenType
{
	//Operations
	Plus,
	Minus,
	Multiply,
	Divide,
	PlusEquals,
	MinusEquals,
	MultiplyEquals,
	DivideEquals,
	RemainderDiv,
	RemainderDivEquals,
	Equals,

	//Bool Operations
	Less,
	LessOrEqual,
	Greater,
	GreaterOrEqual,
	BoolEquals,
	BoolNoEquals,
	BoolNo,
	And,
	Or,
	Is,

	//Values
	IntLiteral,
	UintLiteral,
	FloatLiteral,
	StringLiteral,
	CharLiteral,

	//TypeKeywords
	IntKeyword,
	UintKeyword,
	FloatKeyword,
	StringKeyword,
	CharKeyword,

	//Keywords
	FuncKeyword,
	ForKeyword,
	WhileKeyword,
	LoopKeyword,
	VarKeyword,
	BreakKeyword,
	ContinueKeyword,
	ConstKeyword,
	IfKeyword,
	ElseKeyword,
	TrueKeyword,
	FalseKeyword,
	CastKeyword,

	//Symbols
	Semicolon,
	OpenParenthesis,
	CloseParenthesis,
	OpenSquareBracket,
	CloseSquareBracket,
	OpenCurlyBracket,
	CloseCurlyBracket,
	DoubleQuote,
	SingleQuote,
	Comma,
	Dot,
	Colon,

	// Other
	Identifier,
	EndOfFile,
	StructKeyword
}