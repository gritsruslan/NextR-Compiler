using NextR_Compiler.Tokens;

namespace NextR_Compiler.Parser;

public abstract class Expression
{
	public abstract TokenType Type { get; }
}

public class NumberExpression(LiteralToken number) : Expression
{
	public LiteralToken Token { get; } = number;
	public override TokenType Type => Token.Type;
}

public sealed class BinaryExpression(
	Expression left,
	NonLiteralToken operation,
	Expression right ) : Expression
{

	public Expression Left { get; } = left;
	public NonLiteralToken Operator { get; } = operation;
	public Expression Right { get; } = right;

	public override TokenType Type => TokenType.BinaryExpression;
}

public sealed class SyntaxTree(Expression root)
{
	public Expression Root { get; } = root;
}

public sealed class Parser(List<Token> tokens, List<string> diagnostics)
{
	private List<Token> _tokens = tokens;

	private List<string> _diagnostics = diagnostics;

	private int _position = 0;

	private Token Current => Peek(0);

	private Token NextToken()
	{
		var current = Current;
		_position++;
		return current;
	}

	private Token Match(TokenType type)
	{
		if (Current.Type == type)
			return NextToken();

		return new NonLiteralToken(type, _position, string.Empty);
	}

	private Token Peek(int offset)
	{
		var index = _position + offset;

		if(index >= _tokens.Count)
			return _tokens[^1];

		return _tokens[index];
	}

	public SyntaxTree Parse()
	{
		var expression = ParseExpression();
		return new SyntaxTree(expression);
	}

	private Expression ParseExpression()
	{
		var left = ParseTerm();

		while (Current.Type is TokenType.Plus or TokenType.Minus)
		{
			var operatorToken = NextToken();
			var right = ParseTerm();
			left = new BinaryExpression(left, (NonLiteralToken) operatorToken, right);
		}

		return left;
	}

	private Expression ParseTerm()
	{
		var left = ParsePrimaryExpression();

		while (Current.Type is TokenType.Multiply or TokenType.Divide)
		{
			var operatorToken = NextToken();
			var right = ParsePrimaryExpression();
			left = new BinaryExpression(left, (NonLiteralToken)operatorToken, right);
		}

		return left;
	}

	private Expression ParsePrimaryExpression()
	{
		if (Current.Type == TokenType.OpenParenthesis)
		{
			NextToken();

			var expression = ParseExpression();

			if (Current.Type == TokenType.CloseParenthesis)
			{
				NextToken();
			}
			else
			{
				_diagnostics.Add($"Error: Expected ')' but found {Current.Type}");
			}

			return expression;
		}

		var intToken = Match(TokenType.IntLiteral);
		return new NumberExpression((LiteralToken)intToken);
	}

}

public sealed class Evaluator
{
	public int EvaluateNum(Expression expression)
	{
		if(expression is NumberExpression numberExpression)
			return (int) numberExpression.Token.Value;

		var binaryExpression = expression as BinaryExpression;

		var left = EvaluateNum(binaryExpression!.Left);
		var operatorToken = binaryExpression!.Operator;
		var right = EvaluateNum(binaryExpression!.Right);

		int result = operatorToken.Type switch
		{
			TokenType.Plus => left + right,
			TokenType.Minus => left - right,
			TokenType.Divide => left / right,
			TokenType.Multiply => left * right,
			_ => throw new Exception($"Undefined operation {nameof(operatorToken.Type)} {operatorToken.ValueString}")
		};

		return result;
	}
}