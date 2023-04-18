var builder = WebApplication.CreateBuilder(args)
    .AddLogging()
    .AddMudBlazor()
    .AddServices()
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
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.UseRxExceptionHandler();
app.UseDefaultCulture();
app.Run();