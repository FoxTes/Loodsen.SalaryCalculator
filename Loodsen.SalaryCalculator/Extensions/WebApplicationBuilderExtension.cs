namespace Loodsen.SalaryCalculator.Extensions;

/// <summary>
/// Extensions for <see cref="WebApplicationBuilder"/>.
/// </summary>
public static class WebApplicationBuilderExtension
{
    /// <summary>
    /// Add logging.
    /// </summary>
    /// <param name="builder"><see cref="WebApplicationBuilder"/>.</param>
    public static WebApplicationBuilder AddAzureAppConfiguration(this WebApplicationBuilder builder)
    {
        builder.Configuration.AddAzureAppConfiguration(options =>
        {
            var connectionString = builder.Configuration.GetConnectionString("AppConfig");
            options
                .Connect(connectionString)
                .UseFeatureFlags(featureFlagOptions =>
                    featureFlagOptions.CacheExpirationInterval = TimeSpan.FromMinutes(5));
        });

        return builder;
    }

    /// <summary>
    /// Add logging.
    /// </summary>
    /// <param name="builder"><see cref="WebApplicationBuilder"/>.</param>
    public static WebApplicationBuilder AddLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
            loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));

        Locator.CurrentMutable.UseSerilogFullLogger();

        return builder;
    }

    /// <summary>
    /// Add MudBlazor.
    /// </summary>
    /// <param name="builder"><see cref="WebApplicationBuilder"/>.</param>
    public static WebApplicationBuilder AddMudBlazor(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();
        builder.Services.AddMudServices();

        return builder;
    }

    /// <summary>
    /// Add infrastructure.
    /// </summary>
    /// <param name="builder"><see cref="WebApplicationBuilder"/>.</param>
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddAzureAppConfiguration();
        builder.Services.AddFeatureManagement();
        builder.Services.AddMemoryCache();

        if (!builder.Environment.IsDevelopment())
            builder.Services.AddApplicationInsightsTelemetry();

        return builder;
    }

    /// <summary>
    /// Add services.
    /// </summary>
    /// <param name="builder"><see cref="WebApplicationBuilder"/>.</param>
    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IAppVersionService, AppVersionService>();
        builder.Services.AddSingleton<IIsDayOffService, IsDayOffService>();
        builder.Services.AddSingleton<ISalaryService, SalaryService>();

        builder.Services.AddSingleton<ExceptionHandler>();

        return builder;
    }

    /// <summary>
    /// Add view models.
    /// </summary>
    /// <param name="builder"><see cref="WebApplicationBuilder"/>.</param>
    public static WebApplicationBuilder AddViewModels(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<HomeViewModel>();

        return builder;
    }

    /// <summary>
    /// Add providers.
    /// </summary>
    /// <param name="builder"><see cref="WebApplicationBuilder"/>.</param>
    public static WebApplicationBuilder AddProviders(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddHttpClient<IIsDayOffService, IsDayOffService>()
            .ConfigureHttpClient((_, httpClient) => httpClient.BaseAddress = new Uri("https://isdayoff.ru/"))
            .AddPolicyHandler(GetRetryPolicy());

        return builder;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(result => !result.IsSuccessStatusCode)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
}