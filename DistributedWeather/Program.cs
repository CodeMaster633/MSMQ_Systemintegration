using Experimental.System.Messaging;
using System.Text.Json;

namespace DistributedWeather
{
    class Program
    {
        private static readonly string sourceQueuePath = @".\private$\freshFromOW";
        private static readonly string acQueuePath = @".\private$\queueToAC";
        private static readonly string airportInfoQueuePath = @".\private$\queueToAirportInfo";
        private static readonly string airportCCQueuePath = @".\private$\queueToAirportCC";

        static void Main(string[] args)
        {
            CreateQueueIfNotExists(sourceQueuePath);
            CreateQueueIfNotExists(acQueuePath);
            CreateQueueIfNotExists(airportInfoQueuePath);
            CreateQueueIfNotExists(airportCCQueuePath);

            Console.WriteLine("Listening for new messages on the source queue...");

            using (MessageQueue sourceQueue = new MessageQueue(sourceQueuePath))
            {
                sourceQueue.Formatter = new XmlMessageFormatter(new String[] { "System.String,mscorlib" });

                while (true)
                {
                    try
                    {
                        Message message = sourceQueue.Receive();
                        string weatherDataJson = message.Body.ToString();

                        DistributeMessage(weatherDataJson);
                    }
                    catch (MessageQueueException mqEx)
                    {
                        Console.WriteLine($"MSMQ Error: {mqEx.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
            }
        }

        private static void CreateQueueIfNotExists(string queuePath)
        {
            if (!MessageQueue.Exists(queuePath))
            {
                MessageQueue.Create(queuePath);
                Console.WriteLine($"Created queue: {queuePath}");
            }
        }
        private static void DistributeMessage(string messageBody)
        {
            string weatherDataJson = messageBody.Replace("<string>", "").Replace("</string>", "").Trim();

            try
            {
                JsonDocument weatherData = JsonDocument.Parse(weatherDataJson);

                SendMessageToQueue(airportCCQueuePath, weatherDataJson);
                Console.WriteLine("Message sent to Command Center");

                var airportInfoData = new
                {
                    name = weatherData.RootElement.GetProperty("name").GetString(),
                    country = weatherData.RootElement.GetProperty("country").GetString(),
                    temperature = weatherData.RootElement.GetProperty("temperature").GetDouble()
                };
                SendMessageToQueue(airportInfoQueuePath, JsonSerializer.Serialize(airportInfoData));
                Console.WriteLine("Message sent to Airport Information Center queue.");

                var acData = new
                {
                    name = weatherData.RootElement.GetProperty("name").GetString(),
                    country = weatherData.RootElement.GetProperty("country").GetString(),
                    clouds = weatherData.RootElement.GetProperty("clouds").GetInt32(),
                    temperature = weatherData.RootElement.GetProperty("temperature").GetDouble()
                };
                SendMessageToQueue(acQueuePath, JsonSerializer.Serialize(acData));
                Console.WriteLine("Message sent to Air Traffic Control queue.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing JSON: {ex.Message}");
            }
        }


        private static void SendMessageToQueue(string queuePath, string messageContent)
        {
            using (MessageQueue queue = new MessageQueue(queuePath))
            {
                Message msg = new Message
                {
                    Body = messageContent,
                    Formatter = new XmlMessageFormatter(new String[] { "System.String,mscorlib" })
                };
                queue.Send(msg);
                Console.WriteLine($"Message sent to queue: {queuePath}");
            }
        }
    }
}
