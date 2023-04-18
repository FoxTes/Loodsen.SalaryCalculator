namespace Loodsen.SalaryCalculator.Pages.ViewModels;

/// <summary>
/// View model for <see cref="Home"/>.
/// </summary>
public class HomeViewModel : ReactiveObject
{
    [SuppressMessage("ReSharper", "PrivateFieldCanBeConvertedToLocalVariable", Justification = "Memory leak")]
    private readonly ISalaryService _salaryService;

    private readonly ReadOnlyObservableCollection<DaysRange> _daysRanges;

    /// <summary>
    /// Initializes a new instance of the <see cref="HomeViewModel"/> class.
    /// </summary>
    /// <param name="salaryService"><see cref="ISalaryService"/>.</param>
    public HomeViewModel(ISalaryService salaryService)
    {
        _salaryService = salaryService;

        var daysRanges = new SourceCache<DaysRange, Guid>(daysRange => daysRange.Id);
        daysRanges.AddOrUpdate(DaysRange.Empty);
        daysRanges.Connect()
            .Sort(SortExpressionComparer<DaysRange>.Ascending(t => t.DateRange.Start ?? DateTime.MaxValue))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _daysRanges)
            .DisposeMany()
            .Subscribe();

        var propsObservable = this
            .WhenAnyValue(x => x.SalaryBrutto, v => v.SalaryAdditional)
            .Throttle(TimeSpan.FromSeconds(0.8), RxApp.TaskpoolScheduler)
            .DistinctUntilChanged()
            .CombineLatest(
                this.WhenValueChanged(z => z.Date)
                    .DistinctUntilChanged(),
                _daysRanges.ToObservableChangeSet()
                    .DistinctUntilChanged()
                    .Select(x => x.First())
                    .Where(x => x.Item.Current is { DateRange.Start: not null })
                    .StartWith(default(Change<DaysRange>)),
                (salary, date, _) => new
                {
                    Salary = new { salary.Item1, salary.Item2 },
                    Date = date,
                    FreeDaysRange = _daysRanges
                        .Where(x => x.DateRange.Start is not null)
                        .ToArray()
                });

        propsObservable
            .Where(x => (x.Salary.Item1 != default || x.Salary.Item2 != default) && !string.IsNullOrEmpty(x.Date))
            .SelectMany(x => _salaryService.CalculateAsync(x.Salary.Item1, x.Salary.Item2, x.Date!, x.FreeDaysRange))
            .ToPropertyEx(this, z => z.Salary);

        propsObservable
            .Select(x => (x.Salary.Item1 != default || x.Salary.Item2 != default) && !string.IsNullOrEmpty(x.Date))
            .ToPropertyEx(this, z => z.IsShow);

        AddOrUpdateDaysRange = ReactiveCommand.Create(new Action<DaysRange>(range => daysRanges.AddOrUpdate(range)));
        RemoveDaysRange = ReactiveCommand.Create(new Action<Guid>(guid => daysRanges.Remove(DaysRange.FromGuid(guid))));
    }

    /// <summary>
    /// Salary in brutto.
    /// </summary>
    [Reactive]
    public decimal SalaryBrutto { get; set; }

    /// <summary>
    /// Salary additional.
    /// </summary>
    [Reactive]
    public decimal SalaryAdditional { get; set; }

    /// <summary>
    /// Data calc salary.
    /// </summary>
    [Reactive]
    public string? Date { get; set; }

    /// <summary>
    /// Availability of results.
    /// </summary>
    [ObservableAsProperty]
    public bool IsShow { get; }

    /// <summary>
    /// Salary result.
    /// </summary>
    [ObservableAsProperty]
    public Salary Salary { get; }

    /// <summary>
    /// Add or update a range of non-working days.
    /// </summary>
    public ReactiveCommand<DaysRange, Unit> AddOrUpdateDaysRange { get; }

    /// <summary>
    /// Delete a range of non-working days.
    /// </summary>
    public ReactiveCommand<Guid, Unit> RemoveDaysRange { get; }

    /// <summary>
    /// Non-working days.
    /// </summary>
    public ReadOnlyObservableCollection<DaysRange> DaysRanges => _daysRanges;
}