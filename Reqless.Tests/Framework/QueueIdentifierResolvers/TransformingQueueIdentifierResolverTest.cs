using Reqless.Framework.QueueIdentifierResolvers;
using Reqless.Tests.Common.TestHelpers;

namespace Reqless.Tests.Framework.QueueIdentifierResolvers;

/// <summary>
/// Unit tests for the <see cref="TransformingQueueIdentifierResolver"/> class.
/// </summary>
public class TransformingQueueIdentifierResolverTest
{
    /// <summary>
    /// <see cref="TransformingQueueIdentifierResolver"/> constructor should
    /// throw when the queue identifiers transformers argument is null.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenQueueIdentifiersTransformersIsNull()
    {
        Scenario.ThrowsWhenArgumentIsNull(
            () => new TransformingQueueIdentifierResolver(null!),
            "queueIdentifiersTransformers"
        );
    }

    /// <summary>
    /// <see cref="TransformingQueueIdentifierResolver"/> constructor should set
    /// the expected properties.
    /// </summary>
    [Fact]
    public async Task Constructor_SetsExpectedProperties()
    {
        List<string> queueIdentifiers = ["queue-1", "@bloop"];
        List<string> expectedResolvedQueueNames = [.. queueIdentifiers];
        expectedResolvedQueueNames.Reverse();
        List<IQueueIdentifiersTransformer> queueIdentifiersTransformers = [
            new OrderReversingQueueIdentifiersTransformer(),
        ];
        TransformingQueueIdentifierResolver subject = new(queueIdentifiersTransformers);

        // Roundabout way to check that queueIdentifiersTransformers is set correctly.
        var queueNames = await subject.ResolveQueueNamesAsync(queueIdentifiers.ToArray());
        Assert.Equal(expectedResolvedQueueNames, queueNames);
    }

    /// <summary>
    /// <see cref="TransformingQueueIdentifierResolver.ResolveQueueNamesAsync"/>
    /// should throw if queue identifiers is null.
    /// </summary>
    [Fact]
    public async Task ResolveQueueNamesAsync_ThrowsWhenQueueIdentifiersIsNull()
    {
        string[] queueIdentifiers = null!;
        TransformingQueueIdentifierResolver subject = new([]);

        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => subject.ResolveQueueNamesAsync(queueIdentifiers)
        );
        Assert.Equal("Value cannot be null. (Parameter 'queueIdentifiers')", exception.Message);
        Assert.Equal("queueIdentifiers", exception.ParamName);
    }

    /// <summary>
    /// <see cref="TransformingQueueIdentifierResolver.ResolveQueueNamesAsync"/>
    /// should return the transformed queue names.
    /// </summary>
    [Fact]
    public async Task ResolveQueueNamesAsync_ReturnsTransformedQueueNames()
    {
        List<string> queueIdentifiers = ["queue-1", "@bloop"];
        List<string> expectedResolvedQueueNames = [.. queueIdentifiers];
        expectedResolvedQueueNames.Reverse();
        List<IQueueIdentifiersTransformer> queueIdentifiersTransformers = [
            new OrderReversingQueueIdentifiersTransformer(),
        ];
        TransformingQueueIdentifierResolver subject = new(queueIdentifiersTransformers);

        var queueNames = await subject.ResolveQueueNamesAsync([.. queueIdentifiers]);
        Assert.Equal(expectedResolvedQueueNames, queueNames);
    }

    /// <summary>
    /// Implementation of <see cref="IQueueIdentifiersTransformer"/> that
    /// transforms queue identifiers by reversing their order.
    /// </summary>
    class OrderReversingQueueIdentifiersTransformer : IQueueIdentifiersTransformer
    {
        /// <summary>
        /// Transform the given queue identifiers by reversing their order.
        /// </summary>
        public Task<List<string>> TransformAsync(List<string> queueIdentifiers)
        {
            List<string> transformedQueueIdentifiers = [.. queueIdentifiers];
            transformedQueueIdentifiers.Reverse();
            return Task.FromResult(transformedQueueIdentifiers);
        }
    }
}
