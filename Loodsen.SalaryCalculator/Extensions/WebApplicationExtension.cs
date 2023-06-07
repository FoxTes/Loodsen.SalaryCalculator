namespace Loodsen.SalaryCalculator.Extensions;

/// <summary>
/// Extensions for <see cref="IApplicationBuilder"/>.
/// </summary>
public static class WebApplicationExtension
{
    /// <summary>
    /// Use the global RX handler.
    /// </summary>
    /// <param name="app"><see cref="IApplicationBuilder"/>.</param>
    public static WebApplication UseRxExceptionHandler(this WebApplication app)
    {
        RxApp.DefaultExceptionHandler = app.Services.GetService<ExceptionHandler>()!;

        return app;
    }

    /// <summary>
    /// Use to default russian culture.
    /// </summary>
    /// <param name="app"><see cref="IApplicationBuilder"/>.</param>
    public static WebApplication UseDefaultCulture(this WebApplication app)
    {
        var cultureInfo = new CultureInfo("ru-RU");

        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

        return app;
    }

    /// <summary>
    /// Use application version logging.
    /// </summary>
    /// <param name="app"><see cref="IApplicationBuilder"/>.</param>
    public static WebApplication UseAppVersionLogging(this WebApplication app)
    {
        var logger = app.Services.GetService<ILogger>()!;
        var versionService = app.Services.GetService<IAppVersionService>()!;

        logger.Information("Version application: {Version}", versionService.Version);

        return app;
    }

    /// <summary>
    /// Use application metrics.
    /// </summary>
    /// <param name="app"><see cref="IApplicationBuilder"/>.</param>
    public static WebApplication UseMetrics(this WebApplication app)
    {
        if (!app.Environment.IsProduction())
            return app;

        var metrics = app.GetOption<MetricsOption>();
        app.MapMetrics().RequireHost(metrics.AllowedHosts);

        return app;
    }

    private static TOption GetOption<TOption>(this WebApplication app)
        where TOption : BaseOption, new()
    {
        return app.Configuration
            .GetSection(new TOption().Name)
            .Get<TOption>()!;
    }
}