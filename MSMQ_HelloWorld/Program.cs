using System;
using Experimental.System.Messaging;

class Program
{
    //static void Main()
    //{
    //    string queuePath = @".\Private$\MinKø";

    //    // Opret køen, hvis den ikke allerede findes
    //    if (!MessageQueue.Exists(queuePath))
    //    {
    //        MessageQueue.Create(queuePath);
    //    }

    //    // Opret en forbindelse til køen
    //    using (MessageQueue queue = new MessageQueue(queuePath))
    //    {
    //        // Opret beskeden
    //        Message message = new Message();
    //        message.Body = "Hej, verden!";
    //        message.Label = "Min første besked";

    //        // Send beskeden til køen
    //        queue.Send(message);

    //        Console.WriteLine("Besked sendt!");

    //        queue.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });

    //        Message modtagetMessage = queue.Receive();
    //        Console.WriteLine("Besked modtaget: " + (string)modtagetMessage.Body);
    //    }

    //}
}
