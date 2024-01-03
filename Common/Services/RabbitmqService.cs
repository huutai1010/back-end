using Common.AppConfiguration;
using Common.Interfaces;
using Common.Models.RabbitMq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Services
{
    public class RabbitmqService : IRabbitmqService
    {
        private readonly RabbitmqSettings _settings;

        public RabbitmqService(IOptions<RabbitmqSettings> settings)
        {
            _settings = settings.Value;
        }

        public bool PublidMessage(List<FileMessageModel> fileMessageModels)
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _settings.HostName,
                    UserName = _settings.UserName,
                    Password = _settings.Password,
                    VirtualHost = _settings.VHost
                };
                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel(); 
                foreach (var messageModel in fileMessageModels)
                {
                    string? queueName = _settings.ProducerQueueName;
                    channel.QueueDeclare(queueName, true, false, false, null);

                    var message = new
                    {
                        FileName = messageModel.FileName,
                        BlobName = messageModel.BlobName
                    };
                    var messageJson = JsonConvert.SerializeObject(message);
                    var body = Encoding.UTF8.GetBytes(messageJson);

                    // Send the JSON message to RabbitMQ
                    channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
                }
            }catch(Exception ex)
            {
                throw new Exception("Error: "+ ex.Message);
            }
            return true;

        }
    }
}
