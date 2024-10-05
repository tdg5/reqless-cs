using Microsoft.Extensions.DependencyInjection;
using Reqless.Worker.DependencyInjection;

namespace Reqless.Worker.Tests.DependencyInjection;

/// <summary>
/// Tests for the <see cref="ServiceBuilder"/> class.
/// </summary>
public class ServiceBuilderTest
{
    /// <summary>
    /// <see cref="ServiceBuilder.FromFactory{T, F}"/> uses the given factory to
    /// create the service.
    /// </summary>
    [Fact]
    public void FromFactory_UsesTheFactoryToCreateTheService()
    {
        var expectedValue = "Hello, World!";
        IServiceCollection services = new ServiceCollection();
        services.AddSingleton(new SecondOrderDependency(expectedValue));
        services.AddSingleton(
            ServiceBuilder.FromFactory<FirstOrderDependency, FirstOrderDependencyFactory>()
        );
        var serviceProvider = services.BuildServiceProvider();
        var firstOrderDependency = serviceProvider.GetRequiredService<FirstOrderDependency>();
        Assert.Equal(expectedValue, firstOrderDependency.SecondOrderDependencyValue);
    }

    /// <summary>
    /// Example first order dependency that has a second order dependency and is
    /// created by a factory.
    /// </summary>
    public class FirstOrderDependency
    {
        /// <summary>
        /// The value of the second order dependency provided to the
        /// constructor.
        /// </summary>
        public string SecondOrderDependencyValue { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="FirstOrderDependency"/>.
        /// </summary>
        /// <param name="secondOrderDependencyValue">The value of the second
        /// order dependency that should be used to create the first order
        /// dependency.</param>
        public FirstOrderDependency(string secondOrderDependencyValue)
        {
            SecondOrderDependencyValue = secondOrderDependencyValue;
        }
    }

    /// <summary>
    /// Example second order dependency that is needed to create first order
    /// dependencies.
    /// </summary>
    public class SecondOrderDependency
    {
        /// <summary>
        /// Arbitrary string value.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="SecondOrderDependency"/>.
        /// </summary>
        /// <param name="value">Arbitray string value.</param>
        public SecondOrderDependency(string value)
        {
            Value = value;
        }
    }

    /// <summary>
    /// Example <see cref="IServiceFactory{T}"/> that builds <see
    /// cref="FirstOrderDependency"/> instances.
    /// </summary>
    public class FirstOrderDependencyFactory : IServiceFactory<FirstOrderDependency>
    {
        private readonly SecondOrderDependency _secondOrderDependency;

        /// <summary>
        /// Initializes a new instance of <see cref="FirstOrderDependency"/>.
        /// </summary>
        /// <param name="secondOrderDependency">The second order dependency
        /// that should be used to create first order dependencies.</param>
        public FirstOrderDependencyFactory(SecondOrderDependency secondOrderDependency)
        {
            _secondOrderDependency = secondOrderDependency;
        }

        /// <inheritdoc/>
        public FirstOrderDependency Build() => new(_secondOrderDependency.Value);
    }
}
