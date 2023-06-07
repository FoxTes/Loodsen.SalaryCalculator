namespace Loodsen.SalaryCalculator.Models.Options;

public abstract record BaseOption
{
    /// <summary>
    /// Name section.
    /// </summary>
    public abstract string Name { get; }
}