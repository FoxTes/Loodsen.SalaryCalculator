namespace Loodsen.SalaryCalculator.Benchmarks.Benchmarks;

[MemoryDiagnoser]
[ShortRunJob]
public class LoggerBenchmark
{
    private readonly DateTime _dateTime = DateTime.Now;
    private readonly int[] _array = new int[1];

    private ILogger _logger = null!;

    [GlobalSetup]
    public void Setup()
    {
        using var factory = new CustomWebApplicationFactory<IMarker>();
        _logger = factory.Services.GetService<ILogger>()!;
    }

    [Benchmark]
    public void WriteLog() => _logger.Information(@"Test value");

    [Benchmark]
    public void WriteStructuredLog() => _logger.Information(@"Test value - {Value} or {String}", 1, "Test");

    [Benchmark]
    public void WriteLogWithParam() =>
        _logger.Information(
            "Value1 {Value1}, Value2 {Value2}, Value3 {Value3}, Value4 {Value4}", 1, 2, _dateTime, _array);
}