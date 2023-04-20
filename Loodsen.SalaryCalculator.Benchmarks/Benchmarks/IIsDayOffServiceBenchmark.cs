namespace Loodsen.SalaryCalculator.Benchmarks.Benchmarks;

[MemoryDiagnoser]
[ShortRunJob]
public class IsDayOffServiceBenchmark
{
    private IIsDayOffService _isDayOffService = null!;

    [GlobalSetup]
    public void Setup()
    {
        using var factory = new CustomWebApplicationFactory<IMarker>();
        _isDayOffService = factory.Services.GetService<IIsDayOffService>()!;
    }

    [Benchmark]
    public async Task<string> GetMonth() => await _isDayOffService.GetMonthAsync(new DateOnly(2023, 2, default));
}