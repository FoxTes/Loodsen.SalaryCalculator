namespace Loodsen.SalaryCalculator.Models;

public readonly record struct Payment(
    decimal Value,
    decimal LossesPercent,
    DateOnly Date);
