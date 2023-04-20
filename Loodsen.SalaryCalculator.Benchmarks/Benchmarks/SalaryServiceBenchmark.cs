namespace Loodsen.SalaryCalculator.Benchmarks.Benchmarks;

[MemoryDiagnoser]
[ShortRunJob]
public class SalaryServiceBenchmark
{
    private ISalaryService _salaryService = null!;

    [GlobalSetup]
    public void Setup()
    {
        using var factory = new CustomWebApplicationFactory<IMarker>();
        _salaryService = factory.Services.GetService<ISalaryService>()!;
    }

    [Benchmark]
    public async Task<Salary> CalculateTest() =>
        await _salaryService.CalculateAsync(1000, 500, "06.01.2022", Array.Empty<DaysRange>());
}