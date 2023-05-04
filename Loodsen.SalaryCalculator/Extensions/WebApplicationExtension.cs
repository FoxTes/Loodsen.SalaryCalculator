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
    public static IApplicationBuilder UseRxExceptionHandler(this IApplicationBuilder app)
    {
        RxApp.DefaultExceptionHandler = app.ApplicationServices.GetService<ExceptionHandler>()!;

        return app;
    }

    /// <summary>
    /// Use to default russian culture.
    /// </summary>
    /// <param name="app"><see cref="IApplicationBuilder"/>.</param>
    public static IApplicationBuilder UseDefaultCulture(this IApplicationBuilder app)
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
    public static IApplicationBuilder UseAppVersionLogging(this IApplicationBuilder app)
    {
        var logger = app.ApplicationServices.GetService<ILogger>()!;
        var versionService = app.ApplicationServices.GetService<IAppVersionService>()!;

        logger.Information("Version application: {Version}", versionService.Version);

        return app;
    }
}