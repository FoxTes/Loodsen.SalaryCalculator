namespace Loodsen.SalaryCalculator.Services;

/// <inheritdoc />
public sealed class SalaryService : ISalaryService
{
    private const decimal TaxRate = 0.87m;
    private const int LastDayMonth = 31;

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
        var currentMonthDayOffInfo = await _isDayOffService.GetMonthAsync(dateOnly);

        var prepaymentCurrentMonth = await GetPrepayment(brutto, dateOnly, ranges);
        var prepaymentCurrentMonthOnlyWeekends = await GetPrepayment(brutto, dateOnly, ranges, true);
        var prepaymentLastMonth = await GetPrepayment(brutto, dateOnly.AddMonths(-1), Array.Empty<DaysRange>());
        var paymentLastMonth = (brutto + additional) * TaxRate - prepaymentLastMonth;

        return new Salary(
            new Payment(
                paymentLastMonth,
                0,
                await GetPayDay(currentMonthDayOffInfo, dateOnly, Salary.Payday)),
            new Payment(
                prepaymentCurrentMonth,
                100 - (float)prepaymentCurrentMonth.ToPercent(prepaymentCurrentMonthOnlyWeekends),
                await GetPayDay(currentMonthDayOffInfo, dateOnly, Salary.PrepaymentDay)));
    }

    private async ValueTask<decimal> GetPrepayment(
        decimal salaryBrutto,
        DateOnly dateOnly,
        IReadOnlyCollection<DaysRange> ranges,
        bool isOnlyWeekends = false)
    {
        var month = await _isDayOffService.GetMonthAsync(dateOnly);
        return LocalGetPrepayment();

        decimal LocalGetPrepayment()
        {
            var monthSpan = month.AsSpan();
            Span<int> span = stackalloc int[monthSpan.Length];

            for (var i = 0; i < monthSpan.Length; i++)
                span[i] = ParseDay(int.Parse(monthSpan.Slice(i, 1)), isOnlyWeekends);

            var workingHoursMonthCount = 0;
            var workingHoursMonthFirstHalfCount = 0;

            foreach (var t in span)
                workingHoursMonthCount += t;

            if (!isOnlyWeekends)
            {
                foreach (var range in ranges)
                {
                    if (range.DateRange.Start!.Value.Month != dateOnly.Month)
                        continue;
                    for (var i = range.DateRange.Start!.Value.Day - 1; i < range.DateRange.End!.Value.Day - 1; i++)
                        span[i] = 0;
                }
            }

            for (var i = 0; i <= 14; i++)
                workingHoursMonthFirstHalfCount += span[i];

            return (decimal)(int)(salaryBrutto /
                workingHoursMonthCount * workingHoursMonthFirstHalfCount * TaxRate * 100) / 100;

            int ParseDay(int data, bool weekends) =>
                (data, weekends) switch
                {
                    (0 or 2, _) => 8,
                    (1, _) => 0,
                    (8, false) => 0,
                    (8, true) => 8,
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
                    return GetPayDay(null, dateOnly.AddMonths(-1), LastDayMonth);
            }
        }
    }
}