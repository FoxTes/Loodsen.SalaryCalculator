namespace Loodsen.SalaryCalculator.Models;

public readonly record struct Payment(
    decimal Value,
    float LossesPercent,
    DateOnly Date);
