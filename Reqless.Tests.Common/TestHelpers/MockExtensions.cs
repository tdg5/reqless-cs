using Microsoft.Extensions.Logging;
using Moq;

namespace Reqless.Tests.Common.TestHelpers;

/// <summary>
/// Extensions for <see cref="Mock{T}"/>.
/// </summary>
public static class MockExtensions
{
    /// <summary>
    /// Prepare the given <paramref name="mock"/> to receive verifyable calls
    /// via the <see
    /// cref="LoggerExtensions.LogError(ILogger, Exception?, string?, object?[])"/>
    /// extension method was invoked with the given exception and message.
    /// </summary>
    /// <typeparam name="T">The type of the logger.</typeparam>
    /// <param name="mock">The mock instance to prepare.</param>
    /// <param name="exception">The exception that should be logged.</param>
    /// <param name="message">The message that should be logged.</param>
    /// <returns>The given mock instance.</returns>
    public static Mock<ILogger<T>> VerifyLogError<T>(
        this Mock<ILogger<T>> mock,
        Exception exception,
        string message)
    {
        mock.Setup(
            _ => _.Log(
                LogLevel.Error,
                0,
                It.Is<It.IsAnyType>((instance, instanceType) =>
                    instance.ToString() == message
                    && instanceType.Name == "FormattedLogValues"),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
        .Verifiable();

        return mock;
    }
}
