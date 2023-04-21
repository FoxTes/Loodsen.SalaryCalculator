namespace Loodsen.SalaryCalculator.Benchmarks.Benchmarks;

public struct Test
{
    public int Value { get; set; }
}

[MemoryDiagnoser]
[ShortRunJob]
public class CollectionBenchmark
{
    private readonly IEnumerable<Test> _range = Enumerable
        .Range(0, 3)
        .Select(_ => default(Test));
    private readonly IReadOnlyCollection<Test> _rangeCollection = Enumerable
        .Range(0, 3)
        .Select(_ => default(Test))
        .ToArray();

    [Benchmark]
    public int Test1()
    {
        var count = 0;
        foreach (var range in _range)
        {
            count++;
        }

        return count;
    }

    [Benchmark]
    public int Test12()
    {
        var count = 0;
        for (var i = 0; i < _range.Count(); i++)
        {
            count++;
        }

        return count;
    }

    [Benchmark]
    public int Test2()
    {
        var count = 0;
        foreach (var range in _rangeCollection)
        {
            count += range.Value;
        }

        return count;
    }
}