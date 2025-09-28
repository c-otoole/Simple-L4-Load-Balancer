using System;
using NSubstitute;
using NUnit.Framework;
using SimpleLB;
using SimpleLB.Core;
using SimpleLB.Networking;
using SimpleLB.Services;
using SimpleLB.Strategies;

namespace SimpleLBTests
{
    [TestFixture]
    public class LoadBalancerServiceTest
    {
        [Test]
        public void Ctor_ListeningPortZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var listeningPort = 0;
            var backendPool = Substitute.For<IBackendPool>();
            var strategy = Substitute.For<IBackendStrategy>();
            var streamTimeOut = 100;
            var tcpProxy = Substitute.For<ITcpProxy>();

            // Arrange / Act 
            Assert.Throws<ArgumentOutOfRangeException>(() => new LoadBalancerService(listeningPort, backendPool, strategy, streamTimeOut, tcpProxy));
        }

        [Test]
        public void Ctor_BackendPoolNull_ThrowsArgumentNullException()
        {
            // Arrange
            var listeningPort = 8080;
            BackendPool backendPool = null;
            var strategy = Substitute.For<IBackendStrategy>();
            var streamTimeOut = 100;
            var tcpProxy = Substitute.For<ITcpProxy>();

            // Arrange / Act 
            Assert.Throws<ArgumentNullException>(() => new LoadBalancerService(listeningPort, backendPool, strategy, streamTimeOut, tcpProxy));
        }

        [Test]
        public void Ctor_StrategyNull_ThrowsArgumentNullException()
        {
            // Arrange
            var listeningPort = 8080;
            var backendPool = Substitute.For<IBackendPool>();
            RoundRobinStrategy strategy = null;
            var tcpProxy = Substitute.For<ITcpProxy>();
            var streamTimeOut = 100;

            // Arrange / Act 
            Assert.Throws<ArgumentNullException>(() => new LoadBalancerService(listeningPort, backendPool, strategy, streamTimeOut, tcpProxy));
        }

        [Test]
        public void Ctor_StreamTimeOutLessThanZero_ThrowsArgumentNullException()
        {
            // Arrange
            var listeningPort = 8080;
            var backendPool = Substitute.For<IBackendPool>();
            var strategy = Substitute.For<IBackendStrategy>();
            var streamTimeOut = -100;
            var tcpProxy = Substitute.For<ITcpProxy>();

            // Arrange / Act 
            Assert.Throws<ArgumentOutOfRangeException>(() => new LoadBalancerService(listeningPort, backendPool, strategy, streamTimeOut, tcpProxy));
        }

        [Test]
        public void Ctor_TcpProxyNull_ThrowsArgumentNullException()
        {
            // Arrange
            var listeningPort = 8080;
            var backendPool = Substitute.For<IBackendPool>();
            var strategy = Substitute.For<IBackendStrategy>();
            TcpProxy tcpProxy = null;
            var streamTimeOut = 100;

            // Arrange / Act 
            Assert.Throws<ArgumentNullException>(() => new LoadBalancerService(listeningPort, backendPool, strategy, streamTimeOut, tcpProxy));
        }
    }
}
