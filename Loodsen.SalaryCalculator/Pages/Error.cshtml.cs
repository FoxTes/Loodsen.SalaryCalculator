namespace Loodsen.SalaryCalculator.Pages;

/// <inheritdoc />
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public sealed class ErrorModel : PageModel
{
    /// <summary>
    /// Request id.
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// Availability of results.
    /// </summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    /// <summary>
    /// Get request id.
    /// </summary>
    public void OnGet() => RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
}