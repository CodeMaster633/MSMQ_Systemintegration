using Experimental.System.Messaging;
using System.Text.Json;
using System.Xml.Serialization;

namespace WeatherToAC
{
    class Program
    {
        private static readonly string acQueuePath = @".\private$\queueToAC";
        private static readonly string sasQueuePath = @".\private$\queueToSAS";
        private static readonly string klmQueuePath = @".\private$\queueToKLM";
        private static readonly string baQueuePath = @".\private$\queueToBA";
        private static readonly string swaQueuePath = @".\private$\queueToSWA";

        static void Main(string[] args)
        {
            CreateQueueIfNotExists(acQueuePath);

            Console.WriteLine("Listening...");

            using (MessageQueue acQueue = new MessageQueue(acQueuePath))
            {
                acQueue.Formatter = new XmlMessageFormatter(new String[] { "System.String,mscorlib" });

                while (true)
                {
                    try
                    {
                        Message message = acQueue.Receive();
                        string weatherDataJson = message.Body.ToString();

                        DistributeWeatherData(weatherDataJson);
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

        private static void DistributeWeatherData(string weatherDataJson)
        {
            var recipients = new Dictionary<string, Action<string>>
            {
                { "SAS", m => SendAsClass(m, sasQueuePath) },
                { "KLM", m => SendAsJsonString(m, klmQueuePath) },
                { "BA", m => SendAsXml(m, baQueuePath) },
                { "SWA", m => SendAsXml(m, swaQueuePath) }
            };
            foreach (var recipient in recipients)
            {
                recipient.Value(weatherDataJson);
            }
        }

        private static void SendAsClass(string weatherDataJson, string queuePath)
        {
            JsonDocument weatherData = JsonDocument.Parse(weatherDataJson);
            var sasData = new WeatherClass
            {
                Name = weatherData.RootElement.GetProperty("name").GetString(),
                Country = weatherData.RootElement.GetProperty("country").GetString(),
                Temperature = weatherData.RootElement.GetProperty("temperature").GetDouble(),
                Clouds = weatherData.RootElement.GetProperty("clouds").GetInt32()
            };

            using (MessageQueue queue = new MessageQueue(queuePath))
            {
                queue.Formatter = new XmlMessageFormatter(new Type[] { typeof(WeatherClass) });
                queue.Send(sasData);  
                Console.WriteLine("Sent WeatherClass as xml format. - can't sent as binary in lattest .net");
            }
        }

        private static void SendAsJsonString(string weatherDataJson, string queuePath)
        {
            Console.WriteLine(weatherDataJson);

            using (MessageQueue queue = new MessageQueue(queuePath))
            {
                queue.Send(weatherDataJson); 
                Console.WriteLine($"Sent JSON string to queue: {queuePath}");
            }
        }

        private static void SendAsXml(string weatherDataJson, string queuePath)
        {
            JsonDocument weatherData = JsonDocument.Parse(weatherDataJson);
            var xmlWeather = new WeatherXml
            {
                Name = weatherData.RootElement.GetProperty("name").GetString(),
                Country = weatherData.RootElement.GetProperty("country").GetString(),
                Temperature = weatherData.RootElement.GetProperty("temperature").GetDouble(),
                Clouds = weatherData.RootElement.GetProperty("clouds").GetInt32()
            };

            XmlSerializer serializer = new XmlSerializer(typeof(WeatherXml));
            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, xmlWeather);
                string xmlData = writer.ToString();

                using (MessageQueue queue = new MessageQueue(queuePath))
                {
                    queue.Formatter = new XmlMessageFormatter(new String[] { "System.String,mscorlib" });
                    queue.Send(xmlData);
                    Console.WriteLine($"Sent XML format to queue: {queuePath}");
                }
            }
        }
    }


    public class WeatherClass
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public double Temperature { get; set; }
        public int Clouds { get; set; }
    }

    // Serializable class for XML
    [Serializable]
    public class WeatherXml
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public double Temperature { get; set; }
        public int Clouds { get; set; }
    }
}