namespace Loodsen.SalaryCalculator.Benchmarks.Factories;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        #pragma warning disable CS0618
        builder.UseSerilog((_, _) => { });
        #pragma warning restore CS0618

        builder.ConfigureServices(collection =>
        {
            collection.AddLazyCache();

            collection.RemoveAll(typeof(IHostedService));
            collection.AddMemoryCache();
        });
        builder.UseEnvironment("Development");
    }
}