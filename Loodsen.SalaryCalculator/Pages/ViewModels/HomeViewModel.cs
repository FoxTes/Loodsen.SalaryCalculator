namespace Loodsen.SalaryCalculator.Pages.ViewModels;

/// <summary>
/// View model for <see cref="Home"/>.
/// </summary>
public sealed class HomeViewModel : ReactiveObject, IActivatableViewModel, IDisposable
{
    [SuppressMessage("ReSharper", "PrivateFieldCanBeConvertedToLocalVariable", Justification = "Memory leak")]
    private readonly ISalaryService _salaryService;
    private readonly SourceCache<DaysRange, Guid> _sourceCache = new(daysRange => daysRange.Id);

    private ReadOnlyObservableCollection<DaysRange> _daysRanges = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="HomeViewModel"/> class.
    /// </summary>
    /// <param name="salaryService"><see cref="ISalaryService"/>.</param>
    public HomeViewModel(ISalaryService salaryService)
    {
        _salaryService = salaryService;

        AddOrUpdateDaysRange = ReactiveCommand
            .Create(new Action<DaysRange>(range => _sourceCache.AddOrUpdate(range)));
        AddOrUpdateDaysRanges = ReactiveCommand
            .Create(new Action<IReadOnlyCollection<DaysRange>>(ranges => _sourceCache.AddOrUpdate(ranges)));
        RemoveDaysRange = ReactiveCommand
            .Create(new Action<Guid>(guid => _sourceCache.Remove(DaysRange.FromGuid(guid))));

        this.WhenActivated(disposable =>
        {
            _sourceCache.Connect()
                .Sort(SortExpressionComparer<DaysRange>.Ascending(t => t.DateRange.Start ?? DateTime.MaxValue))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _daysRanges)
                .DisposeMany()
                .Subscribe()
                .DisposeWith(disposable);

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
                    (salary, date, _) => new SalaryRequest(
                        salary.Item1,
                        salary.Item2,
                        date,
                        _daysRanges
                            .Where(x => x.DateRange.Start is not null)
                            .ToArray()
                            .AsReadOnly()));

            propsObservable
                .Where(x => (x.SalaryBrutto != default || x.SalaryAdditional != default) && !string.IsNullOrEmpty(x.Date))
                .Do(_ => InputChange.OnNext(Unit.Default))
                .SelectMany(x => _salaryService.CalculateAsync(x.SalaryBrutto, x.SalaryAdditional, x.Date!, x.Ranges!))
                .ToPropertyEx(this, z => z.Salary)
                .DisposeWith(disposable);

            propsObservable
                .Select(x => (x.SalaryBrutto != default || x.SalaryAdditional != default) && !string.IsNullOrEmpty(x.Date))
                .ToPropertyEx(this, z => z.IsShow)
                .DisposeWith(disposable);
        });
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="HomeViewModel"/> class.
    /// </summary>
    ~HomeViewModel() => Dispose(false);

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
    /// Event about change of input data.
    /// </summary>
    public Subject<Unit> InputChange { get; } = new();

    /// <summary>
    /// Add or update a range of non-working days.
    /// </summary>
    public ReactiveCommand<DaysRange, Unit> AddOrUpdateDaysRange { get; }

    /// <summary>
    /// Add or update a ranges of non-working days.
    /// </summary>
    public ReactiveCommand<IReadOnlyCollection<DaysRange>, Unit> AddOrUpdateDaysRanges { get; }

    /// <summary>
    /// Delete a range of non-working days.
    /// </summary>
    public ReactiveCommand<Guid, Unit> RemoveDaysRange { get; }

    /// <summary>
    /// Non-working days.
    /// </summary>
    public ReadOnlyObservableCollection<DaysRange> DaysRanges => _daysRanges;

    /// <inheritdoc/>
    public ViewModelActivator Activator { get; } = new();

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="HomeViewModel.Dispose()" />
    private void Dispose(bool disposing)
    {
        if (!disposing)
            return;

        AddOrUpdateDaysRanges.Dispose();
        AddOrUpdateDaysRange.Dispose();
        RemoveDaysRange.Dispose();
        InputChange.Dispose();
    }
}