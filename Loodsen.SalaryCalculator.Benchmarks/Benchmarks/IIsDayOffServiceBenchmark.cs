namespace Loodsen.SalaryCalculator.Benchmarks.Benchmarks;

[MemoryDiagnoser]
[ShortRunJob]
public class IsDayOffServiceBenchmark
{
    private IIsDayOffService _isDayOffService = null!;

    [GlobalSetup]
    public void Setup()
    {
        var factory = new CustomWebApplicationFactory<IMarker>();
        _isDayOffService = factory.Services.GetService<IIsDayOffService>()!;
    }

    [Benchmark]
    public async Task<string> GetMonth()
    {
        return await _isDayOffService.GetMonthAsync(new DateOnly(2023, 2, default));
    }
}