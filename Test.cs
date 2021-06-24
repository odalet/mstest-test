using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace MSTestTest
{
    [TestClass, ExcludeFromCodeCoverage]
    public class Test
    {
        // This test works when using MSTest 2.1, but not in MSTest 2.2 in a Docker
        [TestMethod, ExpectedException(typeof(BrokerUnreachableException))]
        public void RabbitClient_cannot_connect_when_passed_invalid_host_and_credentials()
        {
            var factory = new ConnectionFactory
            {
                HostName = "IDONTEXIST",
                UserName = "jdoe",
                Password = "p@ssw0rd",
                Port = 5672,
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true
            };

            try
            {
                _ = factory.CreateConnection("ncore");
                Assert.Fail("Connection should be impossible");
            }
            catch (BrokerUnreachableException)
            {                
                throw; // That's what we expect
            }
            catch (Exception ex)
            {
                Assert.Fail($"Connection creation error: {ex}");
            }
        }
    }
}
