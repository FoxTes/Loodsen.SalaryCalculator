namespace Loodsen.SalaryCalculator.Tests.Services;

[UseCulture("ru-RU")]
public class SalaryServiceTests
{
    private readonly SalaryService _salaryService;

    public SalaryServiceTests()
    {
        var isDayOffMock = new Mock<IIsDayOffService>();
        isDayOffMock
            .Setup(x => x.GetMonthAsync(new DateOnly(2022, 11, 1), It.IsAny<CancellationToken>()))
            .ReturnsAsync("002111000001100000110000011000");
        isDayOffMock
            .Setup(x => x.GetMonthAsync(new DateOnly(2022, 12, 1), It.IsAny<CancellationToken>()))
            .ReturnsAsync("0011000001100000110000011000001");
        isDayOffMock
            .Setup(x => x.GetMonthAsync(new DateOnly(2023, 01, 1), It.IsAny<CancellationToken>()))
            .ReturnsAsync("1111111100000110000011000001100");
        isDayOffMock
            .Setup(x => x.GetMonthAsync(new DateOnly(2023, 02, 1), It.IsAny<CancellationToken>()))
            .ReturnsAsync("0001100000110000011002111100");
        isDayOffMock
            .Setup(x => x.GetMonthAsync(new DateOnly(2023, 03, 1), It.IsAny<CancellationToken>()))
            .ReturnsAsync("0001102100110000011000001100000");
        _salaryService = new SalaryService(isDayOffMock.Object);
    }

    public static IEnumerable<object[]> DataWithFreeDays
    {
        get
        {
            return new List<object[]>
            {
                new object[]
                {
                    162_500, 5000, "2023-03", 59_329.17, 64_261.14, new DaysRange[]
                    {
                        new(Guid.Empty, new DateRange(new DateTime(2023, 2, 1), new DateTime(2023, 2, 3)))
                    }
                },
                new object[]
                {
                    162_500, 5000, "2023-03", 59_329.17, 51_409.09, new DaysRange[]
                    {
                        new(Guid.Empty, new DateRange(new DateTime(2023, 3, 1), new DateTime(2023, 3, 3)))
                    }
                }
            };
        }
    }

    [Theory]
    [InlineData(162_500, 5000, "2022-12", 78_403.58, 70_687.5)]
    [InlineData(162_500, 5000, "2023-01", 75_037.50, 41_581.12)]
    [InlineData(162_500, 5000, "2023-02", 10_4144.12, 86_396.06)]
    [InlineData(162_500, 5000, "2023-03", 59_329.17, 64_261.14)]
    public async Task Calculate_ReturnsCorrectSalaryValues(
        decimal brutto,
        decimal premium,
        string date,
        decimal expectedPayment,
        decimal expectedPrepayment)
    {
        // Act
        var salary = await _salaryService.CalculateAsync(brutto, premium, date, Array.Empty<DaysRange>());

        // Assert
        salary.Payment.Value.Should().BeApproximately(expectedPayment, 1m);
        salary.Prepayment.Value.Should().BeApproximately(expectedPrepayment, 1m);
    }

    [Theory]
    [MemberData(nameof(DataWithFreeDays))]
    public async Task Calculate_ReturnsCorrectSalaryValuesWithFreeDays(
        decimal brutto,
        decimal premium,
        string date,
        decimal expectedPayment,
        decimal expectedPrepayment,
        DaysRange[] range)
    {
        // Act
        var salary = await _salaryService.CalculateAsync(brutto, premium, date, range);

        // Assert
        salary.Payment.Value.Should().BeApproximately(expectedPayment, 1m);
        salary.Prepayment.Value.Should().BeApproximately(expectedPrepayment, 1m);
    }

    [Theory]
    [InlineData("2022-12", "06.12.2022", "21.12.2022")]
    [InlineData("2023-01", "30.12.2022", "20.01.2023")]
    [InlineData("2023-02", "06.02.2023", "21.02.2023")]
    [InlineData("2023-03", "06.03.2023", "21.03.2023")]
    public async Task Calculate_ReturnsCorrectSalaryDates(
        string date,
        string expectedPaymentDate,
        string expectedPrepaymentDate)
    {
        // Act
        var salary = await _salaryService.CalculateAsync(0, 0, date, Array.Empty<DaysRange>());

        // Assert
        salary.Payment.Date.Should().Be(DateOnly.Parse(expectedPaymentDate));
        salary.Prepayment.Date.Should().Be(DateOnly.Parse(expectedPrepaymentDate));
    }
}