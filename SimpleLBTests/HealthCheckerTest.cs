using System;
using NSubstitute;
using NUnit.Framework;
using SimpleLB.Core;
using SimpleLB.Services;

namespace SimpleLBTests
{
    [TestFixture]
    public class HealthCheckerTest
    {
        [Test]
        public void Ctor_BackendPoolNull_ThrowsArgumentNullException()
        {
           
            // Act / Assert
            Assert.Throws<ArgumentNullException>(() => new HealthChecker(null, 2000, 500));
        }

        [Test] public void Ctor_IntervalsMsLessThanZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var backendPool = Substitute.For<IBackendPool>();

            // Act / Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new HealthChecker(backendPool, -10, 500));
        }

        [Test]
        public void Ctor_ConnectionTimeoutLessThanZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var backendPool = Substitute.For<IBackendPool>();

            // Act / Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new HealthChecker(backendPool, 2000, -10));
        }

    }
}
