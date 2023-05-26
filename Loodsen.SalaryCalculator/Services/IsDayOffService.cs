namespace Loodsen.SalaryCalculator.Services;

/// <inheritdoc />
public sealed class IsDayOffService : IIsDayOffService
{
    private readonly ILogger _logger;
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _memoryCache;

    /// <summary>
    /// Initializes a new instance of the <see cref="IsDayOffService"/> class.
    /// </summary>
    /// <param name="logger"><see cref="ILogger"/>.</param>
    /// <param name="httpClient"><see cref="HttpClient"/>.</param>
    /// <param name="memoryCache"><see cref="IMemoryCache"/>.</param>
    public IsDayOffService(
        ILogger logger,
        HttpClient httpClient,
        IMemoryCache memoryCache)
    {
        _logger = logger.ForContext<IsDayOffService>();
        _httpClient = httpClient;
        _memoryCache = memoryCache;
    }

    /// <inheritdoc/>
    public async ValueTask<string> GetMonthAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        var cacheResult = _memoryCache.Get<string?>(date);
        if (cacheResult is not null)
            return cacheResult;

        var url = $"api/getdata?year={date.Year}&month={date.Month}&holiday=1&pre=1";
        try
        {
            var result = await _httpClient.GetStringAsync(url, cancellationToken);
            if (string.IsNullOrEmpty(result))
                throw new HttpRequestException("Ответ от сервера пуст");

            _memoryCache.Set(date, result, DateTimeOffset.UtcNow.AddDays(1));
            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger.Error(ex, "Ошибка при выполнении запроса на удаленный сервер");
            throw;
        }
        catch (OperationCanceledException ex)
        {
            _logger.Error(ex, "Операция отменена");
            throw;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Неизвестная ошибка");
            throw;
        }
    }
}