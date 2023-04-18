namespace Loodsen.SalaryCalculator.Abstractions;

/// <summary>
/// A service that provides methods for getting information about the working calendar.
/// </summary>
public interface IIsDayOffService
{
    /// <summary>
    /// Get information for the specified month.
    /// </summary>
    /// <param name="date">Date calculation.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    ValueTask<string> GetMonthAsync(DateOnly date, CancellationToken cancellationToken = default);
}