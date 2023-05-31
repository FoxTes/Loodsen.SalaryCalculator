namespace Loodsen.SalaryCalculator.Extensions;

/// <summary>
/// Расширение для <see cref="Task{TResult}"/>.
/// </summary>
public static class TaskExtension
{
    /// <summary>
    /// Выполняет задачу и перехватывает любые исключения, которые могут возникнуть.
    /// Если исключение было перехвачено, то вызывается указанный делегат error handler
    /// и возвращается значение по умолчанию для типа T.
    /// </summary>
    /// <typeparam name="T">Тип результата, производимого задачей.</typeparam>
    /// <param name="task">Выполняемая задача.</param>
    /// <param name="errorHandler">Делегат, который будет вызван в случае перехвата исключения.</param>
    /// <returns>Результат, произведенный задачей,
    /// или значение по умолчанию для типа T, если произошло исключение.</returns>
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