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
        var factory = new CustomWebApplicationFactory<IMarker>();
        _logger = factory.Services.GetService<ILogger>()!;
    }

    [Benchmark]
    public void WriteLog()
    {
        _logger.Information(@"Test value");
    }

    [Benchmark]
    public void WriteStructuredLog()
    {
        _logger.Information(@"Test value - {Value} or {String}", 1, "Test");
    }

    [Benchmark]
    public void WriteLogWithParam()
    {
        _logger.Information(
            "Расчет: брутто {Brutto}, премия {Premium}, дата {Date}, неучтенные дни: {FreeDays}",
            1,
            2,
            _dateTime,
            _array);
    }
}