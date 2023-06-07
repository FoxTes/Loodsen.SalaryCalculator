var builder = WebApplication.CreateBuilder(args)
    .AddAzureAppConfiguration()
    .AddLogging()
    .AddOptions()
    .AddMudBlazor()
    .AddInfrastructure()
    .AddServices()
    .AddViewModels()
    .AddProviders();

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseAzureAppConfiguration();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseHttpMetrics();
app.UseRxExceptionHandler();
app.UseDefaultCulture();
app.UseAppVersionLogging();
app.UseMetrics();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();