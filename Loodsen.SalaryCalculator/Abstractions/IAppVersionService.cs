namespace Loodsen.SalaryCalculator.Abstractions;

/// <summary>
/// A service that provides methods for getting information about app version.
/// </summary>
public interface IAppVersionService
{
    /// <summary>
    /// Get version app.
    /// </summary>
    string Version { get; }
}