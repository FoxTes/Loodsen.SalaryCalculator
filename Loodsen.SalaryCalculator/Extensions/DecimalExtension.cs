namespace Loodsen.SalaryCalculator.Extensions;

/// <summary>
/// Extensions for <see cref="decimal"/>.
/// </summary>
public static class DecimalExtension
{
    /// <summary>
    /// Converts a number to a percentage with the specified number of decimal places.
    /// </summary>
    /// <param name="value">Value.</param>
    /// <param name="total">Total value.</param>
    /// <param name="decimalPlaces">The number of decimal places to include in the result.</param>
    public static decimal ToPercent(this decimal value, decimal total, int decimalPlaces = 2)
    {
        if (total == 0)
            throw new ArgumentException("Total value cannot be zero.");

        return Math.Round(value * 100 / total, decimalPlaces);
    }
}