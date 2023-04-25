namespace Loodsen.SalaryCalculator.Pages.Views;

public sealed partial class Home
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Home"/> class.
    /// </summary>
    public Home()
    {
        this.WhenActivated(disposable =>
        {
            ViewModel!.InputChange
                .Subscribe(_ => SetDateStore())
                .DisposeWith(disposable);
        });
    }

    /// <summary>
    /// <see cref="HomeViewModel"/>.
    /// </summary>
    [Inject]
    public HomeViewModel HomeViewModel { set => ViewModel = value; }
}