var builder = WebApplication.CreateBuilder(args)
    .AddLogging()
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

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseRxExceptionHandler();
app.UseDefaultCulture();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();