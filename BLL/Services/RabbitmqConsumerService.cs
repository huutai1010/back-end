using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Common.AppConfiguration;
using Microsoft.Extensions.Options;
using System.Runtime;
using DAL.Interfaces;
using Newtonsoft.Json;
using Common.Models;
using Microsoft.Extensions.DependencyInjection;
using DAL.DatabaseContext;

namespace BLL.Services
{
    public class RabbitmqConsumerService : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IServiceProvider _serviceProvider;
        private readonly string? _queueName;

        public RabbitmqConsumerService(IOptions<RabbitmqSettings> settings, IServiceProvider serviceProvider)
        {
            var _settings = settings.Value;
            _queueName = _settings.ConsumerQueueName;

            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                UserName = _settings.UserName,
                Password = _settings.Password,
                VirtualHost = _settings.VHost
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(_queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var messageResponse = JsonConvert.DeserializeObject<MessageResponseModel>(message);

                Console.WriteLine($"Received message: {messageResponse.FileName}");
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var placeDescription = context.PlaceDescriptions.Where(x => x.VoiceFile == messageResponse.FileName).ToList();
                
                if (placeDescription != null)
                {
                    foreach (var desc in placeDescription)
                    {
                        if (messageResponse.Status == "Success")
                        {
                            desc.VoiceFile = messageResponse.VoiceFileLink;
                            desc.Status = 2;
                            context.SaveChanges();
                        }
                        else
                        {
                            desc.VoiceFile = messageResponse.Message;
                            desc.Status = 3;
                            context.SaveChanges();
                        }
                    }
                    
                }
            };

            _channel.BasicConsume(_queueName, true, consumer);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }

            _channel.Dispose();
            _connection.Dispose();
        }
    }
}
