namespace Loodsen.SalaryCalculator.Tests.Extensions;

public class DecimalExtensionsTests
{
    [Fact]
    public void ToPercent_ShouldThrowArgumentException_WhenTotalIsZero()
    {
        // Arrange
        const decimal value = 25;
        const decimal total = 0;

        // Act
        Action act = () => value.ToPercent(total);

        // Assert
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("Total value cannot be zero.");
    }

    [Theory]
    [InlineData(25, 100, 2, 25.00)]
    [InlineData(50, 200, 1, 25.0)]
    [InlineData(3, 5, 0, 60)]
    [InlineData(0, 10, 1, 0)]
    public void ToPercent_ShouldReturnCorrectValue_WhenCalledWithDifferentInput(
        decimal value,
        decimal total,
        int decimalPlaces,
        decimal expected)
    {
        // Act
        var result = value.ToPercent(total, decimalPlaces);

        // Assert
        result.Should().Be(expected);
    }
}