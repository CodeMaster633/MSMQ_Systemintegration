using Experimental.System.Messaging;
using L13_Message_Transformation;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace L08_Request_Reply
{
    class AIC
    {
        public AIC() { }
        public void AICReply(MessageQueue queue, MessageQueue queue2)
        {

            try
            {
                queue.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });

                Message modtagetMessage = queue.Receive();
                Console.WriteLine("Request besked modtaget: " + (string)modtagetMessage.Body );
                Console.WriteLine(" Correlation ID : " +modtagetMessage.Id);

                ConvertToCanonical(modtagetMessage);

                //SendReply(queue2);
            }
            catch (Exception ex)
            {
                Console.WriteLine("fejl i AIC"+ex.Message);
            }
        }

        public void ConvertToCanonical(Message message)
        {
            Console.WriteLine("Convert to Canonical");

            string json = message.Body.ToString();
            Console.WriteLine("JSON: " + json);

            try
            {
                // Parse JSON-strengen som et JsonDocument
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    // Tjek om "flight" objektet findes
                    if (doc.RootElement.TryGetProperty("flight", out JsonElement flightElement))
                    {
                        // Tjek om "airline" findes i "flight" objektet
                        if (flightElement.TryGetProperty("airline", out JsonElement airlineElement))
                        {
                            string airline = airlineElement.GetString();
                            Console.WriteLine($"Airline: {airline}");
                        }
                        else
                        {
                            Console.WriteLine("Airline key not found in the flight object.");
                        }

                        // Tjek om andre nøgler findes og udtræk værdier
                        if (flightElement.TryGetProperty("flightNumber", out JsonElement flightNumberElement))
                        {
                            string flightNumber = flightNumberElement.GetString();
                            Console.WriteLine($"Flight Number: {flightNumber}");
                        }
                        else
                        {
                            Console.WriteLine("Flight Number key not found in the flight object.");
                        }

                        // Fortsæt med andre nøgler som "destination", "origin", etc.
                    }
                    else
                    {
                        Console.WriteLine("Flight object not found in JSON.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during JSON parsing: " + ex.Message);
            }
        }




        //public static void SendReply(MessageQueue queue)
        //{
        //    try
        //    {

        //        // Opret beskeden
        //        Message message = new Message();
        //        var messageBody = new
        //        {
        //            flightno = "SK345",
        //            ETA = "17:50"
        //        };
        //        string jsonBody = JsonConvert.SerializeObject(messageBody);

        //        message.Body = jsonBody;
        //        message.Label = "Reply af ETA";

        //        // Send beskeden til køen
        //        queue.Send(message);

        //        Console.WriteLine("Besked sendt til reply kø!");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Fejl Sendreply" + ex.Message);
        //    }
        //}
    }
}