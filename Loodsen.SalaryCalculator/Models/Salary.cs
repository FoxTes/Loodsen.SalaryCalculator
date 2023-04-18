namespace Loodsen.SalaryCalculator.Models;

public readonly record struct Salary(Payment Payment, Payment Prepayment)
{
    /// <summary>
    /// Number of the payday.
    /// </summary>
    public const int Payday = 6;

    /// <summary>
    /// Number of the prepayment.
    /// </summary>
    public const int PrepaymentDay = 21;
}