namespace Loodsen.SalaryCalculator.Models.Options;

public record MetricsOption : BaseOption
{
    /// <inheritdoc/>
    public override string Name => "Metrics";

    /// <summary>
    /// Allowed host.
    /// </summary>
    [Required]
    public string[] AllowedHosts { get; init; } = null!;
}