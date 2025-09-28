using NUnit.Framework;
using SimpleLB.Strategies;
using SimpleLB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleLB.Core;

namespace SimpleLBTests
{
    public class RoundRobinTest
    {
        [Test]
        public void Select_Rotates_Through_Healthy_List()
        {
            // Arrange
            var a = new Backend("h1", 1) { IsHealthy = true };
            var b = new Backend("h2", 2) { IsHealthy = true };
            var c = new Backend("h3", 3) { IsHealthy = true };
            var healthy = new List<IBackend> { a, b, c };
            var roundRobin = new RoundRobinStrategy();

            // Act / Assert
            Assert.That(roundRobin.Select(healthy), Is.SameAs(a));
            Assert.That(roundRobin.Select(healthy), Is.SameAs(b));
            Assert.That(roundRobin.Select(healthy), Is.SameAs(c));
            Assert.That(roundRobin.Select(healthy), Is.SameAs(a));
        }
        [Test]
        public void Select_ReturnsNull_WhenEmpty()
        {
            // Arrange
            var roundRobin = new RoundRobinStrategy();

            var result = roundRobin.Select(new List<IBackend>());
            Assert.That(result, Is.Null);
        }
    }
}

