namespace Loodsen.SalaryCalculator.Services;

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

/// <inheritdoc />
public class IsDayOffService : IIsDayOffService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _memoryCache;

    /// <summary>
    /// Initializes a new instance of the <see cref="IsDayOffService"/> class.
    /// </summary>
    /// <param name="httpClient"><see cref="HttpClient"/>.</param>
    /// <param name="memoryCache"><see cref="IMemoryCache"/>.</param>
    public IsDayOffService(HttpClient httpClient, IMemoryCache memoryCache)
    {
        _httpClient = httpClient;
        _memoryCache = memoryCache;
    }

    /// <inheritdoc/>
    public async ValueTask<string> GetMonthAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        var cacheResult = _memoryCache.Get<string?>(date);
        if (cacheResult is not null)
            return cacheResult;

        var url = QueryHelpers.AddQueryString(
            "api/getdata",
            new List<KeyValuePair<string, StringValues>>
            {
                new("year", new StringValues(date.Year.ToString())),
                new("month", new StringValues(date.Month.ToString())),
                new("pre", new StringValues("1"))
            });

        var result = await _httpClient.GetStringAsync(url, cancellationToken);
        if (string.IsNullOrEmpty(result))
            throw new Exception();

        _memoryCache.Set(date, result, DateTimeOffset.Now.AddDays(1));
        return result;
    }
}