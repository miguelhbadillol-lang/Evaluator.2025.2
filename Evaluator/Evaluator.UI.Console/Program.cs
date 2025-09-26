using Evaluator.Core;

Console.WriteLine("Hello, Evaluator");

var infix1 = "4+5*(4+6)/2";
Console.WriteLine($"{infix1}={ExpressionEvaluator.Evaluate(infix1)}");

var infix2 = "4*(5+6-(8/2^3)-7)-1";
Console.WriteLine($"{infix2}={ExpressionEvaluator.Evaluate(infix2)}");

var infix3 = "123.89*(1.6/2.789)";
Console.WriteLine($"{infix3}={ExpressionEvaluator.Evaluate(infix3)}");

