using FluentAssertions;
using GnMessaging.Retry;
using Microsoft.Extensions.Logging;
using Moq;

namespace GnMessaging.Test;

public class RetryPolicyTests
{
    private readonly RetryPolicy _retryPolicy;

    public RetryPolicyTests()
    {
        var logger = new Mock<ILogger<RetryPolicy>>();
        _retryPolicy = new RetryPolicy(logger.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldSucceedOnFirstAttempt()
    {
        // Arrange
        var callCount = 0;

        // Act
        await _retryPolicy.ExecuteAsync(() =>
        {
            callCount++;
            return Task.CompletedTask;
        });

        // Assert
        callCount.Should().Be(1);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldRetryOnFailure()
    {
        // Arrange
        var callCount = 0;

        // Act
        await _retryPolicy.ExecuteAsync(() =>
        {
            callCount++;
            if (callCount < 3)
                throw new InvalidOperationException("Echec temporaire");
            return Task.CompletedTask;
        }, maxRetries: 5);

        // Assert
        callCount.Should().Be(3);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowAfterMaxRetries()
    {
        // Arrange
        var callCount = 0;

        // Act
        var act = () => _retryPolicy.ExecuteAsync(() =>
        {
            callCount++;
            throw new InvalidOperationException("Echec permanent");
        }, maxRetries: 3);

        // Assert
        await act.Should().ThrowAsync<AggregateException>()
            .WithMessage("*3 tentatives*");
        callCount.Should().Be(4); // 1 initial + 3 retries
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowArgumentNullForNullAction()
    {
        // Act
        var act = () => _retryPolicy.ExecuteAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Theory]
    [InlineData(0, 1)]    // 2^0 * 1s = 1s
    [InlineData(1, 2)]    // 2^1 * 1s = 2s
    [InlineData(2, 4)]    // 2^2 * 1s = 4s
    [InlineData(3, 8)]    // 2^3 * 1s = 8s
    [InlineData(4, 16)]   // 2^4 * 1s = 16s
    public void CalculateDelay_ShouldUseExponentialBackoff(int attempt, int expectedSeconds)
    {
        // Act
        var delay = RetryPolicy.CalculateDelay(attempt);

        // Assert
        delay.Should().Be(TimeSpan.FromSeconds(expectedSeconds));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldRespectCancellationToken()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        var act = () => _retryPolicy.ExecuteAsync(() =>
        {
            throw new InvalidOperationException("Echec");
        }, maxRetries: 5, cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
