using Experimental.System.Messaging;
using Newtonsoft.Json;


class Opgave4
{
    static void Main()
    {
        string queuePath = @".\Private$\AirportInformationCenter";

        // Opret køen, hvis den ikke allerede findes
        if (!MessageQueue.Exists(queuePath))
        {
            MessageQueue.Create(queuePath);
        }

        // Opret en forbindelse til køen
        using (MessageQueue queue = new MessageQueue(queuePath))
        {
            // Opret beskeden
            var messageBody = new
            {
                planlagt_tid = "17:20/20:10",
                flightno = "12345",
                destination = "Amsterdam",
                checkIn = "Open"
            };

            string jsonBody = JsonConvert.SerializeObject(messageBody);

            Message message = new Message();
            message.Body = jsonBody;
            message.Label = "InformationsCenter";

            // Send beskeden til køen
            queue.Send(message, "SAS");

            Console.WriteLine("Besked sendt!");

            queue.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });

            Message modtagetMessage = queue.Receive();
            Console.WriteLine("Besked modtaget: " + (string)modtagetMessage.Body + " TIL:  " + modtagetMessage.Label);
        }

    }
}