using NextR_Compiler.LexicalAnalyzer;
using NextR_Compiler.Parser;
using NextR_Compiler.Tokens;

while (true)
{
	Console.Write("> ");
	var code = Console.ReadLine();

	if(code is null)
		continue;

	var tokens = new LexicalAnalyzer(code).TokenizeCode();
	var ast = new Parser(tokens, []).Parse();
	PrintSyntaxTree(ast.Root);
	var value = new Evaluator().EvaluateNum(ast.Root);

	Console.WriteLine(value);
}

void PrintSyntaxTree(Expression expression, string indent = "", bool isRoot = true)
{
	if (expression is NumberExpression numberExpression)
	{
		Console.WriteLine($"{indent}{(isRoot ? "" : "NumberExpression")}");
		Console.WriteLine($"{indent}   NumberToken {numberExpression.Token.ValueString}");
	}
	else if (expression is BinaryExpression binaryExpression)
	{
		Console.WriteLine($"{indent}{(isRoot ? "" : "BinaryExpression")}");

		PrintSyntaxTree(binaryExpression.Left, indent + "   ", false);

		Console.WriteLine($"{indent}   {binaryExpression.Operator.Type}Token");

		PrintSyntaxTree(binaryExpression.Right, indent + "   ", false);
	}
}

