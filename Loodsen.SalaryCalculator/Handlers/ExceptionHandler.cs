namespace Loodsen.SalaryCalculator.Handlers;

/// <summary>
/// Global exception handler.
/// </summary>
public sealed class ExceptionHandler : IObserver<Exception>
{
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionHandler"/> class.
    /// </summary>
    /// <param name="logger"><see cref="ILogger"/>.</param>
    public ExceptionHandler(ILogger logger) => _logger = logger;

    /// <inheritdoc/>
    public void OnCompleted()
    {
        if (Debugger.IsAttached)
            Debugger.Break();
    }

    /// <inheritdoc/>
    public void OnError(Exception error)
    {
        if (Debugger.IsAttached)
            Debugger.Break();

        _logger.Error("{ErrorSource}: {ErrorMessage}", error.Source, error.Message);
    }

    /// <inheritdoc/>
    public void OnNext(Exception value)
    {
        if (Debugger.IsAttached)
            Debugger.Break();

        _logger.Error("{ValueSource}: {ValueMessage}", value.Source, value.Message);
    }
}