using Microsoft.Extensions.DependencyInjection.ServiceCollection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EventBus.UnitTest
{
    [TestClass]
    public class EventBusTests
    {
        private ServiceCollection services;

        public EventBusTests(ServiceCollection services)
        {
            this.services = services;
        }

        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
