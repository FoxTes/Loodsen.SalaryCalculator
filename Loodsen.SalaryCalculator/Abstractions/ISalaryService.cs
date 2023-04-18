namespace Loodsen.SalaryCalculator.Abstractions;

/// <summary>
/// A service that provides methods for calculating salaries.
/// </summary>
public interface ISalaryService
{
    /// <summary>
    /// Salary calculation async.
    /// </summary>
    /// <param name="brutto">Salary in brutto.</param>
    /// <param name="additional">Salary additional.</param>
    /// <param name="date">Data calc salary.</param>
    /// <param name="ranges">Collection <see cref="DaysRange"/>.</param>
    public Task<Salary> CalculateAsync(
        decimal brutto,
        decimal additional,
        string date,
        IReadOnlyCollection<DaysRange> ranges);
}