namespace Loodsen.SalaryCalculator.Extensions;

/// <summary>
/// Extensions for <see cref="Task{TResult}"/>.
/// </summary>
public static class TaskExtension
{
    /// <summary>
    /// Executes a task and catches any exceptions that may occur. If an exception is caught, the specified
    /// error handler delegate is called and a default value for type T is returned.
    /// </summary>
    /// <typeparam name="T">The type of the result produced by the task.</typeparam>
    /// <param name="task">The task to be executed.</param>
    /// <param name="errorHandler">The delegate to be called in case of an exception being caught.</param>
    /// <returns>
    /// The result produced by the task, or a default value for type T if an exception occurred.
    /// </returns>
    public static async ValueTask<T> TryCatch<T>(this ValueTask<T> task, Action<Exception>? errorHandler)
    {
        try
        {
            return await task.ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            errorHandler?.Invoke(ex);
            return default!;
        }
    }
}