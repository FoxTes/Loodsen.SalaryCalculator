namespace Loodsen.SalaryCalculator.Services;

/// <inheritdoc />
public sealed class SalaryService : ISalaryService
{
    private const decimal TaxRate = 0.87m;

    private readonly IIsDayOffService _isDayOffService;

    /// <summary>
    /// Initializes a new instance of the <see cref="SalaryService"/> class.
    /// </summary>
    /// <param name="isDayOffService"><see cref="IsDayOffService"/>.</param>
    public SalaryService(IIsDayOffService isDayOffService) => _isDayOffService = isDayOffService;

    /// <inheritdoc/>
    public async Task<Salary> CalculateAsync(
        decimal brutto,
        decimal additional,
        string date,
        IReadOnlyCollection<DaysRange> ranges)
    {
        var dateOnly = DateOnly.Parse(date);

        var prepaymentCurrentMonth = await GetPrepayment(brutto, dateOnly, ranges);
        var prepaymentLastMonth = await GetPrepayment(brutto, dateOnly.AddMonths(-1), Array.Empty<DaysRange>());
        var currentMonthDayOffInfo = await _isDayOffService.GetMonthAsync(dateOnly);

        var paymentLastMonth = (brutto + additional) * TaxRate - prepaymentLastMonth;
        return new Salary(
            new Payment(
                paymentLastMonth,
                await GetPayDay(currentMonthDayOffInfo, dateOnly, Salary.Payday)),
            new Payment(
                prepaymentCurrentMonth,
                await GetPayDay(currentMonthDayOffInfo, dateOnly, Salary.PrepaymentDay)));
    }

    private async ValueTask<decimal> GetPrepayment(
        decimal salaryBrutto,
        DateOnly dateOnly,
        IReadOnlyCollection<DaysRange> ranges)
    {
        var month = await _isDayOffService.GetMonthAsync(dateOnly);
        return LocalGetPrepayment();

        decimal LocalGetPrepayment()
        {
            var monthSpan = month.AsSpan();
            Span<int> span = stackalloc int[monthSpan.Length];

            for (var i = 0; i < monthSpan.Length; i++)
                span[i] = ParseDay(int.Parse(monthSpan.Slice(i, 1)));

            var workingHoursMonthCount = 0;
            var workingHoursMonthFirstHalfCount = 0;

            foreach (var t in span)
                workingHoursMonthCount += t;

            foreach (var range in ranges)
            {
                if (range.DateRange.Start!.Value.Month != dateOnly.Month)
                    continue;
                for (var i = range.DateRange.Start!.Value.Day - 1; i < range.DateRange.End!.Value.Day - 1; i++)
                    span[i] = 0;
            }

            for (var i = 0; i <= 14; i++)
                workingHoursMonthFirstHalfCount += span[i];

            return (decimal)(int)(salaryBrutto /
                workingHoursMonthCount * workingHoursMonthFirstHalfCount * TaxRate * 100) / 100;

            int ParseDay(int data) =>
                data switch
                {
                    0 or 2 => 8,
                    1 => 0,
                    _ => throw new ArgumentOutOfRangeException(nameof(data), data, null)
                };
        }
    }

    private async ValueTask<DateOnly> GetPayDay(
        string? month,
        DateOnly dateOnly,
        int startDay)
    {
        month ??= await _isDayOffService.GetMonthAsync(dateOnly);
        return await LocalGetPayDay();

        ValueTask<DateOnly> LocalGetPayDay()
        {
            var monthSpan = month.AsSpan();
            Span<int> span = stackalloc int[monthSpan.Length];

            for (var i = 0; i < monthSpan.Length; i++)
                span[i] = int.Parse(monthSpan.Slice(i, 1));

            startDay--;
            while (true)
            {
                var daySalary = span[startDay];
                if (daySalary is 0 or 2)
                    return new ValueTask<DateOnly>(new DateOnly(dateOnly.Year, dateOnly.Month, startDay + 1));
                startDay -= 1;

                if (startDay < 0)
                    return GetPayDay(null, dateOnly.AddMonths(-1), 31);
            }
        }
    }
}