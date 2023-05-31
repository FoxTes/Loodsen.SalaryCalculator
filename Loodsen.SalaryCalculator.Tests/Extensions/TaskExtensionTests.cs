namespace Loodsen.SalaryCalculator.Tests.Extensions;

public class TaskExtensionTests
{
    [Fact]
    public async void TryCatch_Should_Return_Result_If_Task_Completes_Successfully()
    {
        // arrange
        const int expected = 42;
        var task = ValueTask.FromResult(expected);

        // act
        var result = await task.TryCatch(null);

        // assert
        result.Should().Be(expected);
    }

    [Fact]
    public async void TryCatch_Should_Invoke_ErrorHandler_If_Task_Throws_Exception()
    {
        // arrange
        var expectedException = new InvalidOperationException("Test Exception");
        var task = ValueTask.FromException<int>(expectedException);
        var errorHandlerCalled = false;

        // act
        var result = await task.TryCatch(_ => errorHandlerCalled = true);

        // assert
        result.Should().Be(default);
        errorHandlerCalled.Should().BeTrue();
    }

    [Fact]
    public async void TryCatch_Should_Return_Default_Value_If_ErrorHandler_Is_Not_Provided()
    {
        // arrange
        var task = ValueTask.FromException<int>(new InvalidOperationException("Test Exception"));

        // act
        var result = await task.TryCatch(null);

        // assert
        result.Should().Be(default);
    }
}