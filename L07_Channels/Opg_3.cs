using Experimental.System.Messaging;
using Newtonsoft.Json;

namespace L07_Channels
{
    class Opg_3
    {
        static void Main()
        {
            string queuePath = @".\Private$\AirportInformationCenter";
            string queuePathOut1 = @".\Private$\ETA_SAS";
            string queuePathOut2 = @".\Private$\ETA_KLM";
            string queuePathOut3 = @".\Private$\ETA_SW";

            // Opret køen, hvis den ikke allerede findes
            if (!MessageQueue.Exists(queuePath)) MessageQueue.Create(queuePath);
            if (!MessageQueue.Exists(queuePathOut1)) MessageQueue.Create(queuePathOut1);
            if (!MessageQueue.Exists(queuePathOut2)) MessageQueue.Create(queuePathOut2);
            if (!MessageQueue.Exists(queuePathOut3)) MessageQueue.Create(queuePathOut3);


            // Opret en forbindelse til køen
            using (MessageQueue queue = new MessageQueue(queuePath))
            {
                using (MessageQueue queueOut1 = new MessageQueue(queuePathOut1))
                using (MessageQueue queueOut2 = new MessageQueue(queuePathOut2))
                using (MessageQueue queueOut3 = new MessageQueue(queuePathOut3))

                {
                    // Opret beskeden
                    Message message1 = new Message();
                    Message message2 = new Message();
                    Message message3 = new Message();
                    var messageBody1 = new
                    {
                        airlineCompany = "SAS",
                        estimated_Arrival = "17:20",
                        flightNo = "SK345",
                        destination = "Amsterdam"
                    }; var messageBody2 = new
                    {
                        airlineCompany = "SW",
                        estimated_Arrival = "19:00",
                        flightNo = "KM345",
                        destination = "Paris"
                    }; var messageBody3 = new
                    {
                        airlineCompany = "KLM",
                        estimated_Arrival = "06:20",
                        flightNo = "SU345",
                        destination = "Kobenhavn"
                    };
                    string jsonBody1 = JsonConvert.SerializeObject(messageBody1);
                    string jsonBody2 = JsonConvert.SerializeObject(messageBody2);
                    string jsonBody3 = JsonConvert.SerializeObject(messageBody3);

                    message1.Body = jsonBody1;
                    message2.Body = jsonBody2;
                    message3.Body = jsonBody3;
                    message1.Label = "Wup wup - Besked";
                    message2.Label = "Wup wup - Besked";
                    message3.Label = "Wup wup - Besked";

                    // Send beskeden til køen
                    List<MessageQueue> queues = new List<MessageQueue>();
                    queues.Add(queueOut1);
                    queues.Add(queueOut2);
                    queues.Add(queueOut3);

                    List<Message> messages = new List<Message>();
                    messages.Add(message1);
                    messages.Add(message2);
                    messages.Add(message3);

                    foreach (var q in queues)
                    {
                        foreach (var m in messages)
                        {
                            q.Send(m);
                            Console.WriteLine("Besked---");
                        }
                    }

                    Console.WriteLine("Beskeder sendt!");

                    queueOut1.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
                    queueOut2.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
                    queueOut3.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });

                    Console.WriteLine("Indtast noget for at fortsætte");
                    Console.ReadLine();


                    foreach (var q in queues)
                    {
                        Console.WriteLine("____________________");
                        
                        for (int i = 0; i <= q.GetAllMessages().Length+1; i++) 
                        {
                            Message modtagetMessage = q.Receive();
                            //if (modtagetMessage!= null)
                            Console.WriteLine("Besked modtaget: " + (string)modtagetMessage.Body);
                        }
                    }
                }

            }

        }
    }
}
