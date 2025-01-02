using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Experimental.System.Messaging;


namespace weatherForcarst
{

    class Program
    {
        private static readonly string apiKey = "f153098e38af822347828029a7164658"; 
        private static readonly string city = "Herning";
        private static readonly string apiUrl = $"http://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric";

        static async Task Main(string[] args)
        {
            Console.WriteLine("Fetching weather data for Air Traffic Control Center...");

            var weatherData = await FetchWeatherData();

            if (weatherData != null)
            {
                var filteredData = FilterForAirTrafficControl(weatherData);
                string jsonMessage = JsonSerializer.Serialize(filteredData, new JsonSerializerOptions { WriteIndented = true });

                Console.WriteLine("Filtered JSON Message for Air Traffic Control Center:");
                Console.WriteLine(jsonMessage);

                SendMessageToQueue(jsonMessage);
            }
            else
            {
                Console.WriteLine("Failed to retrieve weather data.");
            }
        }

  
        private static async Task<JsonDocument> FetchWeatherData()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return JsonDocument.Parse(responseBody);
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request error: {e.Message}");
                    return null;
                }
            }
        }

        private static object FilterForAirTrafficControl(JsonDocument weatherData)
        {
            var root = weatherData.RootElement;

            var filteredData = new
            {
                name = root.GetProperty("name").GetString(),
                coordinates = new
                {
                    lat = root.GetProperty("coord").GetProperty("lat").GetDouble(),
                    lon = root.GetProperty("coord").GetProperty("lon").GetDouble()
                },
                country = root.GetProperty("sys").GetProperty("country").GetString(),
                temperature = root.GetProperty("main").GetProperty("temp").GetDouble(),
                humidity = root.GetProperty("main").GetProperty("humidity").GetInt32(),
                pressure = root.GetProperty("main").GetProperty("pressure").GetInt32(),
                wind = new
                {
                    speed = root.GetProperty("wind").GetProperty("speed").GetDouble(),
                    deg = root.GetProperty("wind").GetProperty("deg").GetInt32()
                },
                clouds = root.GetProperty("clouds").GetProperty("all").GetInt32(),
                visibility = root.GetProperty("visibility").GetInt32()
            };

            return filteredData;
        }

        private static void SendMessageToQueue(string message)
        {
            string queuePath = @".\private$\freshFromOW";

            if (!MessageQueue.Exists(queuePath))
            {
                MessageQueue.Create(queuePath);
            }

            using (MessageQueue queue = new MessageQueue(queuePath))
            {

                Message msg = new Message
                {
                    Body = message,
                    Formatter = new XmlMessageFormatter(new String[] { "System.String,mscorlib" })
                };

                queue.Send(msg);
                Console.WriteLine("Message sent to queue: freshFromOW");
            }
        }
    }
}
