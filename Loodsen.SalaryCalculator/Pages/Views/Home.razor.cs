namespace Loodsen.SalaryCalculator.Pages.Views;

public partial class Home
{
    /// <summary>
    /// <see cref="HomeViewModel"/>.
    /// </summary>
    [Inject]
    public HomeViewModel HomeViewModel { set => ViewModel = value; }
}