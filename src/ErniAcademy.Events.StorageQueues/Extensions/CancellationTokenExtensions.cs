namespace ErniAcademy.Events.StorageQueues.Extensions;

/// <summary>
///   The set of extensions for the <see cref="CancellationToken" />
///   struct.
/// </summary>
internal static class CancellationTokenExtensions
{
    /// <summary>
    ///   Throws an exception of the requested type if cancellation has been requested
    ///   of the <paramref name="instance" />.
    /// </summary>
    /// <typeparam name="TException">The type of exception to throw; the type must have a parameterless constructor.</typeparam>
    /// <param name="instance">The instance that this method was invoked on.</param>
    public static void ThrowIfCancellationRequested<TException>(this CancellationToken instance) 
        where TException : Exception, new()
    {
        if (instance.IsCancellationRequested)
        {
            throw new TException();
        }
    }
}
