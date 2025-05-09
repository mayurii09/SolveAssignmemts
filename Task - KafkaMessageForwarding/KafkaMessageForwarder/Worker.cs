using KafkaMessageForwarder.Services;

namespace KafkaMessageForwarder
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly KafkaForwarderService _forwarderService;

        public Worker(IConfiguration config, ILogger<Worker> logger, KafkaForwarderService forwarderService)
        {
            _logger = logger;
            _forwarderService = forwarderService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker service started.");
            _logger.LogInformation("Waiting for new Messages!!!");
            await _forwarderService.ForwardMessagesAsync(stoppingToken);
        }
    }
}
