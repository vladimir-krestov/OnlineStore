using System.Numerics;
using System.Threading;

namespace OnlineStore.WebAPI.Utilities
{
    public class FactorialBackgroundService : BackgroundService
    {
        private readonly ILogger<FactorialBackgroundService> _logger;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _appCancellationToken;
        private bool _processIsRunning;

        public FactorialBackgroundService(ILogger<FactorialBackgroundService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_processIsRunning)
            {
                throw new InvalidOperationException("Process is in progress already.");
            }

            _processIsRunning = true;
            _appCancellationToken = stoppingToken;
            _cancellationTokenSource = new();

            _logger.LogInformation("Factorial Background Service is starting.");

            try
            {
                using CancellationTokenSource linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken, _cancellationTokenSource.Token);

                while (!linkedTokenSource.IsCancellationRequested)
                {
                    _logger.LogInformation("Calculating factorial...");

                    BigInteger result = CalculateFactorial(30); // Например, факториал 30
                    _logger.LogInformation($"Factorial of 30 is: {result}");

                    await Task.Delay(5000, stoppingToken); // Задержка в 5 секунд
                }
            }
            catch
            { }
            finally
            {
                _logger.LogInformation("Factorial Background Service is stopping.");
                _processIsRunning = false;
                _cancellationTokenSource.Dispose();
            }
        }

        public void StopService()
        {
            _cancellationTokenSource.Cancel();
        }

        public void RestartService()
        {
            ExecuteAsync(_appCancellationToken);
        }

        private BigInteger CalculateFactorial(int number)
        {
            BigInteger result = 1;
            for (int i = 2; i <= number; i++)
            {
                result *= i;
            }
            return result;
        }
    }
}
