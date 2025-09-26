// [CHANGED] Necesario para parsear decimales con punto de forma estable
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
        // [CHANGED] Normalizar: quitar espacios y convertir coma a punto (por si el usuario escribe 3,14)
        var s = infix.Replace(" ", "").Replace(',', '.'); // [CHANGED]

        var stack = new Stack<char>();
        var postfix = string.Empty;

        // [CHANGED] Acumulador para números multi-dígito y con punto decimal
        string number = "";              // [CHANGED]
        bool numberHasDot = false;       // [CHANGED]

        foreach (char item in s)
        {
            // [CHANGED] Construir el número mientras vengan dígitos o '.'
            if (char.IsDigit(item) || item == '.')        // [CHANGED]
            {
                if (item == '.')                          // [CHANGED]
                {
                    if (numberHasDot) throw new Exception("Invalid number format."); // [CHANGED]
                    numberHasDot = true;                 // [CHANGED]
                }
                number += item;                           // [CHANGED]
                continue;                                 // [CHANGED]
            }

            // [CHANGED] Si veníamos armando un número, cerrarlo y agregarlo como token
            if (number.Length > 0)                        // [CHANGED]
            {
                postfix += number + " ";                  // [CHANGED] separar tokens con espacio
                number = "";                              // [CHANGED]
                numberHasDot = false;                     // [CHANGED]
            }

            if (IsOperator(item))
            {
                if (item == ')')
                {
                    do
                    {
                        postfix += stack.Pop() + " ";     // [CHANGED] añadir espacio tras operador
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
                            postfix += stack.Pop() + " "; // [CHANGED] añadir espacio tras operador
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

        // [CHANGED] Último número pendiente (si la expresión termina en número)
        if (number.Length > 0)                            // [CHANGED]
        {
            postfix += number + " ";                      // [CHANGED]
        }

        // Vaciar pila
        while (stack.Count > 0)
        {
            postfix += stack.Pop() + " ";                 // [CHANGED] añadir espacio tras operador
        }

        return postfix.Trim();                            // [CHANGED]
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

        // [CHANGED] En lugar de leer char a char, procesamos tokens separados por espacio
        foreach (var token in postfix.Split(' ', StringSplitOptions.RemoveEmptyEntries)) // [CHANGED]
        {
            // [CHANGED] Un token de 1 char que sea operador se procesa como operador binario
            if (token.Length == 1 && IsOperator(token[0]) && token[0] != '(' && token[0] != ')') // [CHANGED]
            {
                var op2 = stack.Pop();
                var op1 = stack.Pop();
                stack.Push(Calulate(op1, token[0], op2));
            }
            else
            {
                // [CHANGED] Parseo robusto de números (incluye decimales con '.')
                stack.Push(double.Parse(token, NumberStyles.Float, CultureInfo.InvariantCulture)); // [CHANGED]
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
