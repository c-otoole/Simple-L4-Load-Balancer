using System;
using NUnit.Framework;
using SimpleLB.Core;
using SimpleLB.Strategies;

namespace SimpleLBTests
{
    [TestFixture]
    public class StrategyFactoryTest
    {
        [Test]
        public void CreateBackendStrategy_ValidStrategyName_ReturnsIBackendStrategy()
        {
            // Arrange
            var validStrategyName = "RoundRobin";
            var factory = new StrategyFactory();

            // Act
            var result = factory.CreateBackendStrategy(validStrategyName);

            // Assert
            Assert.That(result.GetType(), Is.EqualTo(new RoundRobinStrategy().GetType()));
        }

        [Test]
        public void CreateBackendStrategy_InvalidStrategyName_ThrowsArgumentException()
        {
            // Arrange
            var invalidStrategyName = "InvalidStrategyName!";
            var factory = new StrategyFactory();

            // Act / Assert
            Assert.Throws<ArgumentException>(() => factory.CreateBackendStrategy(invalidStrategyName));
        }
    }
}
