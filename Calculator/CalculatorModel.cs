using System;
using System.Collections.Generic;

public class CalculatorException : Exception
{
    public CalculatorException(string message) : base(message) { }
}

// Исключение для деления на ноль
public class DivisionByZeroException : CalculatorException
{
    public DivisionByZeroException() : base("Ошибка: Деление на ноль.") { }
}

// Исключение для некорректного ввода
public class InvalidInputException : CalculatorException
{
    public InvalidInputException(string message) : base(message) { }
}

public interface IOperation
{
    void Execute(Stack<double> stack);

}


public class Addition : IOperation
{
    public void Execute(Stack<double> stack)
    {
        if (stack.Count < 2) throw new InvalidInputException("Недостаточно операндов для операции."); 
        double a = stack.Pop();
        double b = stack.Pop();
        double result = a + b;
        stack.Push(result);
    }
}

public class Subtraction : IOperation
{
    public void Execute(Stack<double> stack)
    {
        if (stack.Count < 2) throw new InvalidInputException("Недостаточно операндов для операции.");
        double b = stack.Pop();
        double a = stack.Pop();
        double result = a - b;
        stack.Push(result);
    }
}

public class Multiplication : IOperation
{
    public void Execute(Stack<double> stack)
    {
        if (stack.Count < 2) throw new InvalidInputException("Недостаточно операндов для операции.");
        double b = stack.Pop();
        double a = stack.Pop();
        double result = a * b;
        stack.Push(result);
    }
}

public class Division : IOperation
{
    public void Execute(Stack<double> stack)
    {
        if (stack.Count < 2) throw new InvalidInputException("Недостаточно операндов для операции.");
        double b = stack.Pop();
        double a = stack.Pop();
        if (b == 0) throw new DivisionByZeroException();
        double result = a / b;
        stack.Push(result);
    }
}

public class SquareRoot : IOperation
{
    public void Execute(Stack<double> stack)
    {
        double a = stack.Pop();
        if (a < 0) throw new InvalidInputException("Нельзя извлечь корень из отрицательного числа.");
        double result = Math.Sqrt(a);
        stack.Push(result);
    }
}

public class Factorial : IOperation
{
    public void Execute(Stack<double> stack)
    {
        if (stack.Count < 1) throw new InvalidInputException("Недостаточно операндов для операции.");
        double a = stack.Pop();

        if (a < 0 || a % 1 != 0) throw new InvalidInputException("Факториал определен только для неотрицательных целых чисел.");

        double result = FactorialCalculate((int)a);
        stack.Push(result);
    }

    private double FactorialCalculate(int n)    
    {
        if (n == 0) return 1; 
        double result = 1;
        for (int i = 1; i <= n; i++)
        {
            result *= i;
        }
        return result;
    }
}


public static class OperationFactory
{
    public static IOperation CreateOperation(string op)
    {
        if (op == "+")
        {
            return new Addition();
        }
        else if (op == "-")
        {
            return new Subtraction();
        }
        else if (op == "*")
        {
            return new Multiplication();
        }
        else if (op == "/")
        {
            return new Division();
        }
        else if (op == "√")
        {
            return new SquareRoot();
        }
        else if (op == "!")
        {
            return new Factorial(); 
        }
        else
        {
            throw new InvalidInputException("Неверная операция");
        }
    }
}

public class StackCalculator
{
    private Stack<double> stack = new Stack<double>();

    public void StackClear()
    {
        stack.Clear();
    }
    public double Calculate(string expression)
    {
        StackClear(); // Сбрасываем стек перед новым вычислением
        var postfix = InfixToPostfix(expression);
        return EvaluatePostfix(postfix);
    }

    private List<string> InfixToPostfix(string infix)
    {
        var output = new List<string>();
        var operators = new Stack<string>();
        var tokens = Tokenize(infix);

        foreach (var token in tokens)
        {
            if (double.TryParse(token, out double number))
            {
                output.Add(token);
            }
            else if (IsOperator(token))
            {
                while (operators.Count > 0 && Precedence(operators.Peek()) >= Precedence(token))
                {
                    output.Add(operators.Pop());
                }
                operators.Push(token);
            }
            else if (token == "(")
            {
                operators.Push(token);
            }
            else if (token == ")")
            {
                while (operators.Count > 0 && operators.Peek() != "(")
                {
                    output.Add(operators.Pop());
                }
                if (operators.Count == 0) throw new InvalidInputException("Несоответствующая скобка.");
                operators.Pop(); // Удаляем '('
            }
            else
            {
                throw new InvalidInputException($"Некорректный токен: '{token}'");
            }
        }

        while (operators.Count > 0)
        {
            output.Add(operators.Pop());
        }


        return output;
    
}

    private List<string> Tokenize(string infix)
    {
        var tokens = new List<string>();
        string currentToken = "";

        foreach (char c in infix)
        {
            // Проверяем, является ли символ цифрой
            if (char.IsDigit(c) || c == ',')
            {
                currentToken += c; // Добавляем цифру к текущему числу
            }
            else
            {
                // Если текущий токен не пустой, добавляем его в список
                if (currentToken != "")
                {
                    tokens.Add(currentToken);
                    currentToken = ""; // Сбрасываем текущий токен
                }

                // Добавляем оператор в список
                tokens.Add(c.ToString()); // из char нужно в string конвертировать
            }
        }

        // Добавляем последний токен, если он существует
        if (currentToken != "")
        {
            tokens.Add(currentToken);
        }


        return tokens;
    }

    private double EvaluatePostfix(List<string> postfix)
    {
        foreach (var token in postfix)
        {
            if (double.TryParse(token, out double number))
            {
                stack.Push(number);
            }
            else if (IsOperator(token))
            {
                IOperation operation = OperationFactory.CreateOperation(token);
                operation.Execute(stack);
            }
        }

        if (stack.Count != 1) throw new InvalidInputException("Некорректное выражение.");

        return stack.Pop();
    }

    private bool IsOperator(string token)
    {
        return token == "+" || token == "-" || token == "*" || token == "/" || token == "√" || token == "!";
    }

    private int Precedence(string op)
    {
        if (op == "+" || op == "-")
        {
            return 1;
        }
        else if (op == "*" || op == "/")
        {
            return 2;
        }
        else if (op == "√" || op == "!")
        {
            return 3;
        }
        else
        {
            return 0;
        }
    }

}
