using System;

public class CalculatorController
{
    private readonly StackCalculator _calculator;

    public CalculatorController()
    {
        _calculator = new StackCalculator();
    }

    public double Calculate(string expression)
    {
        try
        {
            return _calculator.Calculate(expression);
        }
        catch (CalculatorException ex)
        {
            throw new Exception($"Ошибка: {ex.Message}");
        }
    }
}
