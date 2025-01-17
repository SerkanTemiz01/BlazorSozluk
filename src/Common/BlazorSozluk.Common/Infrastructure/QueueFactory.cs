﻿
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorSozluk.Common.Infrastructure
{
    public static class QueueFactory
    {
        public static void SendMessageToExchange(string exchangeName, 
                                                 string exchangeType,
                                                 string queueName, 
                                                 object obj)
        {
            // send message to queue
            var channel = CreateBasicConsumer()
                          .EnsureExchange(exchangeName,exchangeType)
                          .EnsureQueue(queueName,exchangeName)
                          .Model;

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(obj));

            channel.BasicPublish(exchange: exchangeName,
                                 routingKey: queueName,
                                 basicProperties: null,
                                 body: body);
        }

        public static EventingBasicConsumer CreateBasicConsumer()
        {
            try
            {
                var factory = new ConnectionFactory() { HostName = SozlukConstants.RabbitMQHost };
                var connection = factory.CreateConnection();
                var channel = connection.CreateModel();
                var consumer = new EventingBasicConsumer(channel);
                return consumer;
            }
            catch (Exception)
            {

                throw new Exception("RabbitMQ bağlantısı kurulamadı.");
            }
        
            
        }
        
        public static EventingBasicConsumer EnsureExchange(this EventingBasicConsumer consumer,
                                                           string exchangeName,
                                                           string exchangeType=SozlukConstants.DefaultExchangeType)
        {
            consumer.Model.ExchangeDeclare(exchange: exchangeName,
                                           type: exchangeType,
                                           durable: false,
                                           autoDelete: false);
            // ensure exchange
            return consumer;
        }

        public static EventingBasicConsumer EnsureQueue(this EventingBasicConsumer consumer,
                                                        string queueName,
                                                        string exchangeName)
        {
            consumer.Model.QueueDeclare(queue: queueName,
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

            consumer.Model.QueueBind(queue: queueName,
                                     exchange: exchangeName,
                                     routingKey: queueName);
            // ensure queue
            return consumer;
        }
    }
}
