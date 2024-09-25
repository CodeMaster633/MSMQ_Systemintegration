// See https://aka.ms/new-console-template for more information
using Experimental.System.Messaging;
using Newtonsoft.Json;

class Program
{

    static void Main()
    {
        string queuePath = @".\Private$\ETAKø";

        // Opret køen, hvis den ikke allerede findes
        if (!MessageQueue.Exists(queuePath))
        {
            MessageQueue.Create(queuePath);
        }

        // Opret en forbindelse til køen
        using (MessageQueue queue = new MessageQueue(queuePath))
        {
            // Opret beskeden
            Message message = new Message();
            var messageBody = new
            {
                Estimated_Arrival = "17:20",
                flightno = "SK345",
                destination = "Amsterdam",
            };
            string jsonBody = JsonConvert.SerializeObject(messageBody);

            message.Body = jsonBody;
            message.Label = "Min første besked";

            // Send beskeden til køen
            queue.Send(message);

            Console.WriteLine("Besked sendt!");

            queue.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });

            Message modtagetMessage = queue.Receive();
            Console.WriteLine("Besked modtaget: " + (string)modtagetMessage.Body);
        }

    }
}