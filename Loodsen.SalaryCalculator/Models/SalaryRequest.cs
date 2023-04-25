namespace Loodsen.SalaryCalculator.Models;

public sealed record SalaryRequest(
    decimal SalaryBrutto,
    decimal SalaryAdditional,
    string? Date,
    IReadOnlyCollection<DaysRange>? Ranges);