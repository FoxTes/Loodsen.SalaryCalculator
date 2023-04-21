namespace Loodsen.SalaryCalculator.Pages.Views;

public sealed partial class Home
{
    /// <summary>
    /// <see cref="HomeViewModel"/>.
    /// </summary>
    [Inject]
    public HomeViewModel HomeViewModel { set => ViewModel = value; }
}