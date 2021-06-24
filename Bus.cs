using System;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace MSTestTest
{
    internal sealed class Bus
    {
        private readonly ConnectionFactory factory;
        public Bus(string hostName, string userName, string password)
        {
            factory = new ConnectionFactory
            {
                HostName = hostName,
                UserName = userName,
                Password = password,
                Port = 5672,
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true
            };
        }

        public bool IsConnected { get; private set; }
        private IConnection Connection { get; set; }

        public Bus Connect()
        {
            try
            {
                Connection = factory.CreateConnection("foo");
                IsConnected = true;
            }
            catch
            {
                IsConnected = false;
            }

            return this;
        }

        public async Task<bool> TryConnectAsync(int attempts, CancellationToken cancellationToken, int delayBetweenAttemptsInSeconds = 20)
        {
            var forEver = attempts <= 0;
            var currentAttempt = 0;
            var delay = delayBetweenAttemptsInSeconds * 1000; // convert to ms
            var wasCanceled = false;

            while (forEver || currentAttempt < attempts)
            {
                try
                {
                    // NB: we pass cancellationToken to the task below; however it is useless for now as 
                    // with the current version of RabbitMQ Client, connection attempt is not cancelable
                    // Hopefully, this will serve once we use the v6 RabbitMQ client...
                    _ = await Task.Run(Connect, cancellationToken).ConfigureAwait(false);
                    if (IsConnected)
                        return true;

                    await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                    currentAttempt++;

                    if (cancellationToken.IsCancellationRequested)
                    {
                        wasCanceled = true;
                        break;
                    }
                }
                catch (OperationCanceledException)
                {
                    wasCanceled = true;
                    break;
                }
            }

            //if (wasCanceled)
            //    log.Info($"Connecting to the bus was canceled externally after {currentAttempt} attempt(s)");
            //else
            //    log.Error($"Number of connection attempts exceeded {attempts}");

            return false;
        }
    }
}
