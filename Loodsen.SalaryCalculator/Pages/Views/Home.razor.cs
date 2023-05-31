namespace Loodsen.SalaryCalculator.Pages.Views;

public sealed partial class Home
{
    private bool _featureFreeDays;

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

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync() =>
        _featureFreeDays = await FeatureManager.IsEnabledAsync(FeatureFlags.FreeDays);

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var result = await ProtectedLocalStore
                .GetAsync<SalaryRequest>(CacheKeys.Input)
                .TryCatch(ex =>
                {
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopRight;
                    Snackbar.Add(
                        "Не удалось загрузить предустановленные параметры." +
                        "Пожалуйста, очистите кеш и удалите файлы cookie.",
                        Severity.Error);
                    Log.ForContext<Home>().Warning(ex, "Failed to load preset settings");
                });

            if (result is { Success: true, Value: not null })
            {
                var value = result.Value;

                ViewModel!.SalaryBrutto = value.SalaryBrutto;
                ViewModel!.SalaryAdditional = value.SalaryAdditional;
                ViewModel!.Date = value.Date;

                if (value.Ranges!.Count != 0)
                    await ViewModel!.AddOrUpdateDaysRanges.Execute(value.Ranges!).ToTask();
            }

            await ViewModel!.AddOrUpdateDaysRange.Execute(DaysRange.Empty).ToTask();
        }
    }

    private async void SetDateStore()
    {
        await ProtectedLocalStore.SetAsync(
            CacheKeys.Input,
            new SalaryRequest(
                ViewModel!.SalaryBrutto,
                ViewModel!.SalaryAdditional,
                ViewModel!.Date,
                ViewModel.DaysRanges.Where(x => x.DateRange.Start != default).ToArray()));
    }
}