using Confluent.Kafka;

namespace KafkaMessageForwarder.Services
{
    public class KafkaForwarderService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<KafkaForwarderService> _logger;
        private readonly TimeSpan _pauseStart;
        private readonly TimeSpan _pauseEnd;

        public KafkaForwarderService(IConfiguration configuration, ILogger<KafkaForwarderService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            _pauseStart = TimeSpan.Parse(_configuration["Kafka:PauseStart"]);
            _pauseEnd = TimeSpan.Parse(_configuration["Kafka:PauseEnd"]);
        }
        private bool IsInPauseWindow()
        {
            var now = DateTime.Now.TimeOfDay;
            if (_pauseEnd > _pauseStart)
            {
                return now >= _pauseStart && now < _pauseEnd;
            }
            else
            {
                return now >= _pauseStart || now < _pauseEnd;
            }
        }

        public async Task ForwardMessagesAsync(CancellationToken stoppingToken)
        {
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = _configuration["Kafka:BootstrapServers"],
                GroupId = _configuration["Kafka:ConsumerGroupId"],
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
                //MaxPollIntervalMs = 600000 // 10 minutes in milliseconds --> When background services takes more time then use it otherwise not needed)
            };

            var producerConfig = new ProducerConfig
            {
                BootstrapServers = _configuration["Kafka:BootstrapServers"]
            };

            using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
            using var producer = new ProducerBuilder<string, string>(producerConfig).Build();

            consumer.Subscribe(_configuration["Kafka:SourceTopic"]);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var inPause = IsInPauseWindow();

                    if (inPause)
                    {
                        _logger.LogWarning("In pause window. Waiting to resume...");
                        await Task.Delay(10000, stoppingToken); //Delay of 10 seconds
                        continue;
                    }

                    try
                    {
                        var result = consumer.Consume(stoppingToken);

                        var message = new Message<string, string>
                        {
                            Key = null,
                            Value = result.Message.Value
                        };

                        await producer.ProduceAsync(_configuration["Kafka:DestinationTopic"], message);

                        consumer.Commit(result);

                        _logger.LogInformation("Message forwarded and committed: '{Value}' at {Offset}", message.Value, result.Offset);
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError(ex, "Consume error");
                    }
                    catch (ProduceException<string, string> ex)
                    {
                        _logger.LogError(ex, "Produce error");
                    }
                }
            }
            finally
            {
                consumer.Close();
            }

        }
    }
}
