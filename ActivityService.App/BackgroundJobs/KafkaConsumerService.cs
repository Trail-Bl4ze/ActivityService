using System.Text.Json;
using ActivityService.App.Interfaces;
using ActivityService.App.Models;
using ActivityService.Domain;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ActivityService.App.BackgroundJobs;

// KafkaConsumerService.cs
public class KafkaConsumerService : BackgroundService
{
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly IServiceProvider _scopeFactory;

    public KafkaConsumerService(IConfiguration configuration, ILogger<KafkaConsumerService> logger, IServiceProvider serviceScopeFactory)
    {
        _logger = logger;
        var config = new ConsumerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            GroupId = "activity-service-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        _scopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {   
            _consumer.Subscribe("user-activities");
            
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(stoppingToken);
                    
                    using (var scope = _scopeFactory.CreateScope()) // Создаем scope для каждого сообщения
                    {
                        var activityService = scope.ServiceProvider.GetRequiredService<IActivityService>();
    
                        var activityRequest = JsonSerializer.Deserialize<ActivityRequest>(consumeResult.Message.Value);
                        await activityService.AddUserActivityAsync(activityRequest, stoppingToken);
                        
                        _consumer.Commit(consumeResult);
                    }
                }
                catch (ConsumeException e)
                {
                    _logger.LogError(e, "Kafka consume error");
                    await Task.Delay(1000, stoppingToken); // Пауза при ошибках потребления
                }
                catch (JsonException e)
                {
                    _logger.LogError(e, "Message deserialization error");
                    // Можно добавить логику обработки битых сообщений
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Unexpected error processing message");
                    await Task.Delay(1000, stoppingToken);
                }
            }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        await base.StopAsync(stoppingToken);
    }

    public override void Dispose()
    {
        _consumer.Dispose();
        base.Dispose();
    }
}