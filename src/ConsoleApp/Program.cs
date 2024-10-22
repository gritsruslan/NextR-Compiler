using NextR_Compiler.LexicalAnalyzer;
using NextR_Compiler.Parser;

while (true)
{
	var code = Console.ReadLine();

	if(code is null)
		continue;

	var tokens = new LexicalAnalyzer(code).TokenizeCode();
	var ast = new Parser(tokens, []).Parse();
	var value = new Evaluator().Evaluate(ast.Root);

	Console.WriteLine(value);
}