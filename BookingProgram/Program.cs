using Experimental.System.Messaging;
using TilmeldAktivitet;

class Program
{

    public static async Task Main(string[] args)
    {
        string requestPath = @".\Private$\RequestQueue";
        string replyPath = @".\Private$\ReplyQueue";

        if (!MessageQueue.Exists(requestPath))
        {
            MessageQueue.Create(requestPath);
        }
        if (!MessageQueue.Exists(replyPath))
        {
            MessageQueue.Create(replyPath);
        }

        using (MessageQueue requestQueue = new MessageQueue(requestPath))
        using (MessageQueue replyQueue = new MessageQueue(replyPath))
        {
            var booking = new Booking(requestQueue, replyQueue);   
            var medlemsregister = new Medlemsregister(requestQueue, replyQueue);

            Task.Run(() =>  medlemsregister.Start());
            await Task.Delay(1000);
            booking.Start();
        }
    }
}