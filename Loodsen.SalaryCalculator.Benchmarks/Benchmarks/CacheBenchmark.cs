namespace Loodsen.SalaryCalculator.Benchmarks.Benchmarks;

[MemoryDiagnoser]
public class CacheBenchmark
{
    private readonly DateOnly _date = new(2023, 2, 2);

    private IAppCache _cache = null!;
    private IMemoryCache _memoryCache = null!;

    [GlobalSetup]
    public void Setup()
    {
        var factory = new CustomWebApplicationFactory<IMarker>();
        _cache = factory.Services.GetService<IAppCache>()!;
        _memoryCache = factory.Services.GetService<IMemoryCache>()!;
    }

    [Benchmark]
    public int SetMemoryCache() => _memoryCache.Set(_date, 1);

    [Benchmark]
    public void SetCache() => _cache.Add(_date.ToString(), 1);
}