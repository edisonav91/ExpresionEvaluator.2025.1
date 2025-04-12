using System.Text;

namespace Evaluator.Logic;

public class FunctionEvaluator
{
    public static double Evalute(string infix)
    {
        var tokens = Tokens(infix);
        var postfix = ToPostfix(tokens);
        return Calculate(postfix);
    }

    private static List<string> Tokens(string expression)
    {
        var tokens = new List<string>();
        var numberBuilder = new StringBuilder(); 

        foreach (char c in expression)
        {
            if (char.IsDigit(c) || c == '.')
            {
                numberBuilder.Append(c);
            }
            else if (IsOperator(c) || c == '(' || c == ')')
            {
                if (numberBuilder.Length > 0)
                {
                    tokens.Add(numberBuilder.ToString());
                    numberBuilder.Clear();
                }
                tokens.Add(c.ToString());
            }
            else if (!char.IsWhiteSpace(c))
            {
                throw new Exception($"Invalid text: {c}");
            }
        }

        if (numberBuilder.Length > 0)
        {
            tokens.Add(numberBuilder.ToString());
        }

        return tokens;
    }

    private static List<string> ToPostfix(List<string> tokens)
    {
        var stack = new Stack<string>();
        var output = new List<string>();

        foreach (var token in tokens)
        {
            if (double.TryParse(token, out _))
            {
                output.Add(token);
            }
            else if (token == "(")
            {
                stack.Push(token);
            }
            else if (token == ")")
            {
                while (stack.Peek() != "(")
                {
                    output.Add(stack.Pop());
                }
                stack.Pop();
            }
            else if (IsOperator(token[0]))
            {
                while (stack.Count > 0 && PriorityExpression(token[0]) <= PriorityStack(stack.Peek()[0]))
                {
                    output.Add(stack.Pop());
                }
                stack.Push(token);
            }
        }

        while (stack.Count > 0)
        {
            output.Add(stack.Pop());
        }

        return output;
    }

    private static double Calculate(List<string> postfix)
    {
        var stack = new Stack<double>();

        foreach (var token in postfix)
        {
            if (double.TryParse(token.Replace('.', ','), out double number))
            {
                stack.Push(number);
            }
            else if (IsOperator(token[0]))
            {
                var right = stack.Pop();
                var left = stack.Pop();
                stack.Push(Result(left, token[0], right));
            }
            else
            {
                throw new Exception($"Invalid token: {token}");
            }
        }

        return stack.Pop();
    }

    private static double Result(double operator1, char item, double operator2)
    {
        return item switch
        {
            '+' => operator1 + operator2,
            '-' => operator1 - operator2,
            '*' => operator1 * operator2,
            '/' => operator1 / operator2,
            '^' => Math.Pow(operator1, operator2),
            _ => throw new Exception("Invalid expresion"),
        };
    }

    private static int PriorityStack(char item)
    {
        return item switch
        {
            '^' => 3,
            '*' => 2,
            '/' => 2,
            '+' => 1,
            '-' => 1,
            '(' => 0,
            _ => throw new Exception("Invalid expression."),
        };
    }

    private static int PriorityExpression(char item)
    {
        return item switch
        {
            '^' => 4,
            '*' => 2,
            '/' => 2,
            '+' => 1,
            '-' => 1,
            '(' => 5,
            _ => throw new Exception("Invalid expression."),
        };
    }

    private static bool IsOperator(char c) => "+-*/^".IndexOf(c) >= 0;
}