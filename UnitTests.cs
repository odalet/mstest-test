using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace MSTestTest
{
    [TestClass, ExcludeFromCodeCoverage]
    public class UnitTests
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

        [TestMethod]
        public void TryConnectAsync_is_cancelable()
        {
            try
            {
                var bus = new Bus("IDONTEXIST", "jdoe", "p@ssw0rd");
                using var cts = new CancellationTokenSource();
                using var done = new AutoResetEvent(false);

                var connected = false;

                var thread = new Thread(() =>
                {
                    connected = bus.TryConnectAsync(0, cts.Token).GetAwaiter().GetResult();
                    _ = done.Set();
                });

                thread.Start();

                cts.CancelAfter(1000); // Wait for 1s, then cancel
                _ = done.WaitOne(10000); // Wait for TryConnectAsync to finish (and timeout after 10s)

                // Make sure we are not connected
                Assert.IsFalse(connected);
            }
            catch (Exception ex)
            {
                // This happens when running in GitLab CI...
                Assert.Fail($"Test failed: {ex}");
            }
        }
    }
}
