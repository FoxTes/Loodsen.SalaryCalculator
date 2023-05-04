namespace Loodsen.SalaryCalculator.Services;

/// <inheritdoc/>
public class AppVersionService : IAppVersionService
{
    /// <inheritdoc/>
    public string Version =>
        Assembly.GetEntryAssembly()!.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;
}