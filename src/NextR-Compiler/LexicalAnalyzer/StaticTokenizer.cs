using NextR_Compiler.Common;
using NextR_Compiler.Tokens;

namespace NextR_Compiler.LexicalAnalyzer;

public class StaticTokenizer
{
	public static readonly Dictionary<char, TokenType> SeparatorsTypesDictionary = new()
    {
        { '+', TokenType.Plus },
        { '-', TokenType.Minus },
        { '*', TokenType.Multiply },
        { '/', TokenType.Divide },
        { '(', TokenType.OpenParenthesis },
        { ')', TokenType.CloseParenthesis },
        { '[', TokenType.OpenSquareBracket },
        { ']', TokenType.CloseSquareBracket },
        { '=', TokenType.Equals },
        { '%', TokenType.RemainderDiv },
        { '{', TokenType.OpenCurlyBracket},
        { '}', TokenType.CloseCurlyBracket},
        { '.', TokenType.Dot},
        { ',', TokenType.Comma },
        { ';', TokenType.Semicolon },
        { ':', TokenType.Colon},
        { '!', TokenType.BoolNo},
        { '>', TokenType.Greater},
        { '<', TokenType.Less},
    };

    public static readonly Dictionary<string, TokenType> KeywordsTypesDictionary = new()
    {
        { "int", TokenType.IntKeyword },
        { "uint", TokenType.UintKeyword },
        { "float", TokenType.FloatKeyword },
        { "string", TokenType.StringKeyword },
        { "char", TokenType.CharKeyword },
        { "break", TokenType.BreakKeyword },
        { "continue", TokenType.ContinueKeyword },
        { "const", TokenType.ConstKeyword },
        { "cast", TokenType.CastKeyword },
        { "if", TokenType.IfKeyword },
        { "else", TokenType.ElseKeyword },
        { "func", TokenType.FuncKeyword },
        { "for", TokenType.ForKeyword },
        { "while", TokenType.WhileKeyword },
        { "loop", TokenType.LoopKeyword },
        { "var", TokenType.VarKeyword },
        { "true", TokenType.TrueKeyword },
        { "false", TokenType.FalseKeyword },
        { "and", TokenType.And},
        { "or", TokenType.Or},
        { "is", TokenType.Is},
        {"struct", TokenType.StructKeyword}
    };

    public static Option<TokenType> GetTokenTypeIfKeyword(string tokenString)
    {
        if (KeywordsTypesDictionary.TryGetValue(tokenString, out var tokenType))
            return tokenType;

        return Option<TokenType>.None;
    }

    public static TokenType GetTokenTypeIfSeparator(char separator)
    {
        if (SeparatorsTypesDictionary.TryGetValue(separator, out var tokenType))
            return tokenType;

        throw new Exception($"ERROR : Undefined separator : {separator} !");
    }
}