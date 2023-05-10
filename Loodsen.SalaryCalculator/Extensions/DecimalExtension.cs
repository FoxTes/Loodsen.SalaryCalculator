namespace Loodsen.SalaryCalculator.Extensions;

/// <summary>
/// Extensions for <see cref="decimal"/>.
/// </summary>
public static class DecimalExtension
{
    /// <summary>
    /// Converts a number to a percentage.
    /// </summary>
    /// <param name="value">Value.</param>
    /// <param name="total">Total value.</param>
    public static float ToPercent(this decimal value, decimal total) => (float)Math.Round(value * 100 / total, 2);
}