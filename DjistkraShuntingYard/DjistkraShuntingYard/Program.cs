using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DjistkraShuntingYard;


EquationRearranger equation = new EquationRearranger();
equation.RearrangeEquation("2*x + 3 =7", "x");

public class EquationRearranger
{
    public string RearrangeEquation(string equation, string targetVariable)
    {
        var tokens = Tokenize(equation);
        var outputQueue = new Queue<string>();
        var operatorStack = new Stack<string>();

        foreach (var token in tokens)
        {
            if (IsVariable(token) || IsConstant(token))
            {
                outputQueue.Enqueue(token);
            }
            else if (IsOperator(token))
            {
                while (operatorStack.Count > 0 && ShouldPopOperator(operatorStack.Peek(), token))
                {
                    outputQueue.Enqueue(operatorStack.Pop());
                }
                operatorStack.Push(token);
            }
            else if (IsFunction(token))
            {
                operatorStack.Push(token);
            }
            else if (token == "(")
            {
                operatorStack.Push(token);
            }
            else if (token == ")")
            {
                while (operatorStack.Count > 0 && operatorStack.Peek() != "(")
                {
                    outputQueue.Enqueue(operatorStack.Pop());
                }
                if (operatorStack.Count > 0 && operatorStack.Peek() == "(")
                {
                    operatorStack.Pop();
                }
                else
                {
                    throw new ArgumentException("Mismatched parentheses");
                }
            }
        }

        while (operatorStack.Count > 0)
        {
            outputQueue.Enqueue(operatorStack.Pop());
        }

        var rearrangedEquation = string.Join(" ", outputQueue);

        if (!string.IsNullOrWhiteSpace(targetVariable))
        {
            rearrangedEquation = IsolateVariable(rearrangedEquation, targetVariable);
        }

        return rearrangedEquation;
    }

    private List<string> Tokenize(string expression)
    {
        return Regex.Split(expression, @"([+\-*/=()])")
            .Where(token => !string.IsNullOrWhiteSpace(token))
            .Select(token => token.Trim())
            .ToList();
    }

    private bool IsVariable(string token)
    {
        // Implement logic to check if a token is a variable
        // Example: Check if the token is a single letter or a valid variable name
        return token.Length == 1 && Char.IsLetter(token[0]);
    }

    private bool IsConstant(string token)
    {
        // Implement logic to check if a token is a constant (e.g., number)
        double value;
        return double.TryParse(token, out value);
    }

    private bool IsOperator(string token)
    {
        // Implement logic to check if a token is an operator
        return token == "+" || token == "-" || token == "*" || token == "/";
    }

    private bool ShouldPopOperator(string op1, string op2)
    {
        // Implement logic to determine if op1 should be popped from the stack
        // based on precedence and associativity compared to op2
        // Example: Handle operator precedence and associativity rules
        return GetPrecedence(op1) > GetPrecedence(op2) ||
               (GetPrecedence(op1) == GetPrecedence(op2) && IsLeftAssociative(op1));
    }

    private int GetPrecedence(string op)
    {
        // Implement logic to determine the precedence of an operator
        // Example: Return a higher number for higher precedence operators
        switch (op)
        {
            case "+":
            case "-":
                return 1;
            case "*":
            case "/":
                return 2;
            default:
                return 0; // Default precedence for non-operators
        }
    }

    private bool IsLeftAssociative(string op)
    {
        // Implement logic to check if an operator is left-associative
        // Example: Return true for left-associative operators, false otherwise
        return op == "+" || op == "-" || op == "*" || op == "/";
    }

    private bool IsFunction(string token)
    {
        // Implement logic to check if a token is a function
        // Example: Check if the token matches known function names
        return token == "sin" || token == "cos" || token == "log";
    }

    private string IsolateVariable(string equation, string targetVariable)
    {
        // Implement logic to isolate the target variable on one side of the equation
        // Example: If the equation is "2 * x + 3 = 7" and targetVariable is "x",
        // the result should be "x = (7 - 3) / 2"
        // This is a simplified implementation and may not cover all cases.
        var parts = equation.Split('=');
        if (parts.Length == 2)
        {
            var leftSide = parts[0].Trim();
            var rightSide = parts[1].Trim();
            return $"{targetVariable} = {rightSide} / {leftSide}";
        }
        else
        {
            throw new ArgumentException("Invalid equation format");
        }
    }
}