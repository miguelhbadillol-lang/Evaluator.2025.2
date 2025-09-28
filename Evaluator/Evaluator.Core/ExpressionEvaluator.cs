
using System.Globalization;

namespace Evaluator.Core;

public class ExpressionEvaluator
{
    public static double Evaluate(string infix)
    {
        var postfix = InfixToPostfix(infix);
        return Calulate(postfix);
    }

    private static string InfixToPostfix(string infix)
    {
        var s = infix.Replace(" ", "").Replace(',', '.'); 

        var stack = new Stack<char>();
        var postfix = string.Empty;

        
        string number = "";              
        bool numberHasDot = false;       

        foreach (char item in s)
        {
            
            if (char.IsDigit(item) || item == '.')        
            {
                if (item == '.')                         
                {
                    if (numberHasDot) throw new Exception("Invalid number format."); 
                    numberHasDot = true;                 
                }
                number += item;                          
                continue;                                 
            }

           
            if (number.Length > 0)                        
            {
                postfix += number + " ";                 
                number = "";                              
                numberHasDot = false;                     
            }

            if (IsOperator(item))
            {
                if (item == ')')
                {
                    do
                    {
                        postfix += stack.Pop() + " ";     
                    } while (stack.Peek() != '(');
                    stack.Pop();
                }
                else
                {
                    if (stack.Count > 0)
                    {
                        if (PriorityInfix(item) > PriorityStack(stack.Peek()))
                        {
                            stack.Push(item);
                        }
                        else
                        {
                            postfix += stack.Pop() + " "; 
                            stack.Push(item);
                        }
                    }
                    else
                    {
                        stack.Push(item);
                    }
                }
            }
            else
            {
                throw new Exception("Invalid expression.");
            }
        }

        
        if (number.Length > 0)                           
        {
            postfix += number + " ";                     
        }

        
        while (stack.Count > 0)
        {
            postfix += stack.Pop() + " ";                
        }

        return postfix.Trim();                           
    }

    private static bool IsOperator(char item) => item is '^' or '/' or '*' or '%' or '+' or '-' or '(' or ')';

    private static int PriorityInfix(char op) => op switch
    {
        '^' => 4,
        '*' or '/' or '%' => 2,
        '-' or '+' => 1,
        '(' => 5,
        _ => throw new Exception("Invalid expression."),
    };

    private static int PriorityStack(char op) => op switch
    {
        '^' => 3,
        '*' or '/' or '%' => 2,
        '-' or '+' => 1,
        '(' => 0,
        _ => throw new Exception("Invalid expression."),
    };

    private static double Calulate(string postfix)
    {
        var stack = new Stack<double>();

       
        foreach (var token in postfix.Split(' ', StringSplitOptions.RemoveEmptyEntries)) 
        {
            
            if (token.Length == 1 && IsOperator(token[0]) && token[0] != '(' && token[0] != ')') 
            {
                var op2 = stack.Pop();
                var op1 = stack.Pop();
                stack.Push(Calulate(op1, token[0], op2));
            }
            else
            {
                
                stack.Push(double.Parse(token, NumberStyles.Float, CultureInfo.InvariantCulture)); 
            }
        }
        return stack.Peek();
    }

    private static double Calulate(double op1, char item, double op2) => item switch
    {
        '*' => op1 * op2,
        '/' => op1 / op2,
        '^' => Math.Pow(op1, op2),
        '+' => op1 + op2,
        '-' => op1 - op2,
        '%' => op1 % op2,
        _ => throw new Exception("Invalid expression."),
    };
}
