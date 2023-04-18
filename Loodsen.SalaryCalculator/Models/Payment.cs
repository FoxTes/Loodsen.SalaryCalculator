namespace Loodsen.SalaryCalculator.Models;

public readonly record struct Payment(decimal Value, DateOnly Date);
