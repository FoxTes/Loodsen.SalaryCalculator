namespace Loodsen.SalaryCalculator.Models;

public sealed record DaysRange(Guid Id, DateRange DateRange)
{
    /// <summary>
    /// Create empty object.
    /// </summary>
    public static DaysRange Empty => new(Guid.NewGuid(), new DateRange());

    /// <summary>
    /// Create object from <see cref="Guid"/>.
    /// </summary>
    /// <param name="guid"><see cref="Guid"/>.</param>
    public static DaysRange FromGuid(Guid guid) => new(guid, new DateRange());

    /// <inheritdoc/>
    public override string ToString()
    {
        if (DateRange.Start is null || DateRange.End is null)
            return base.ToString()!;

        return $"Дата начала: {DateRange.Start.Value.Date:dd/MM/yyyy}. " +
               $"Длительность: {DateRange.End.Value.Date.Day - DateRange.Start.Value.Date.Day + 1}";
    }
}