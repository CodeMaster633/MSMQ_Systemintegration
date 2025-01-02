

using Experimental.System.Messaging;
using Newtonsoft.Json;
using System.Xml;
using TranslateTilNyhedsbrev;
using Formatting = Newtonsoft.Json.Formatting;

class Program
{
    static void Main()
    {
        string medlemTilMetaPath = @".\Private$\MedlemTilMetaQueue";
        string metaTilNyhedPath = @".\Private$\MetaTillNyhedQueue";

        // Opret Message Queues, hvis de ikke findes
        if (!MessageQueue.Exists(medlemTilMetaPath))
        {
            MessageQueue.Create(medlemTilMetaPath);
        }
        if (!MessageQueue.Exists(metaTilNyhedPath))
        {
            MessageQueue.Create(metaTilNyhedPath);
        }

        using (MessageQueue medlemTilMetaQueue = new MessageQueue(medlemTilMetaPath))
        using (MessageQueue metaTilNyhedQueue = new MessageQueue(metaTilNyhedPath))
        {

            MedlemsregisterModel medlemsregisterMessage = new MedlemsregisterModel
            {
                medlemsNummer = "4321",
                fornavn = "Knud",
                efternavn = "Knudsen",
                adresselinie1 = "Gadevej 1",
                adresselinie2 = "Etage 2",
                postNr = "8000",
                byNavn = "Aarhus",
                mailAdresse = "knud@example.com",
                privat_firma = true,
                aktiv_passiv = true,
                kontigentBetalt = true,
                nyhedsBrev = true,
                datoBrev = "28.12.2024"
            };

            // Serialiser til JSON og sendes til MedlemTilMetaQueue
            string jsonBodyMedlem = JsonConvert.SerializeObject(medlemsregisterMessage);
            Message medlemMessage = new Message
            {
                Body = jsonBodyMedlem,
                Label = "Medlem til Meta",
            };

            medlemTilMetaQueue.Send(medlemMessage);
            Console.WriteLine("Besked sendt til MedlemTilMetaQueue");

            // Besked fra MedlemTilMetaQueue konverteres til KanoniskDataModel
            var medlemResponse = medlemTilMetaQueue.Receive(TimeSpan.FromSeconds(4));
            if (medlemResponse != null)
            {
                medlemResponse.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
                string medlemBody = (string)medlemResponse.Body;
                MedlemsregisterModel receivedMedlemsData = JsonConvert.DeserializeObject<MedlemsregisterModel>(medlemBody);

                KanoniskDataModel kanoniskMessage = MedlemTilKanonisk(receivedMedlemsData);

                // Serialiser den kanoniske besked og send til MetaTilNyhedQueue
                string jsonBodyKanonisk = JsonConvert.SerializeObject(kanoniskMessage);
                Message metaMessage = new Message
                {
                    Body = jsonBodyKanonisk,
                    Label = "Meta til Nyhed",
                };

                metaTilNyhedQueue.Send(metaMessage);
                Console.WriteLine("Besked sendt til MetaTilNyhedQueue");

                // Besked fra MetaTilNyhedQueue konverteres til NyhedsbrevModel
                var metaResponse = metaTilNyhedQueue.Receive(TimeSpan.FromSeconds(4));
                if (metaResponse != null)
                {
                    metaResponse.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
                    string metaBody = (string)metaResponse.Body;

                    KanoniskDataModel receivedKanoniskData = JsonConvert.DeserializeObject<KanoniskDataModel>(metaBody);
                    NyhedsbrevModel nyhedsbrevMessage = KanoniskTilNyhed(receivedKanoniskData);

                    Console.WriteLine("Modtaget besked fra MetaTilNyhedQueue: ");
                    Console.WriteLine(JsonConvert.SerializeObject(nyhedsbrevMessage, Formatting.Indented));
                }
                else
                {
                    Console.WriteLine("Ingen respons modtaget fra MetaTilNyhedQueue.");
                }
            }
            else
            {
                Console.WriteLine("Ingen respons modtaget fra MedlemTilMetaQueue.");
            }
        }
    }

    private static KanoniskDataModel MedlemTilKanonisk(MedlemsregisterModel medlemsregisterMessage)
    {
        return new KanoniskDataModel(
            medlemsregisterMessage.medlemsNummer,
            $"{medlemsregisterMessage.fornavn} {medlemsregisterMessage.efternavn}".Trim(),
            $"{medlemsregisterMessage.adresselinie1} {medlemsregisterMessage.adresselinie2} {medlemsregisterMessage.byNavn}".Trim(),
            medlemsregisterMessage.postNr,
            medlemsregisterMessage.mailAdresse,
            medlemsregisterMessage.datoBrev
        );
    }

    private static NyhedsbrevModel KanoniskTilNyhed(KanoniskDataModel kanoniskDataModel)
    {
        string dato = OmformDato(kanoniskDataModel.dato);

        return new NyhedsbrevModel(
            kanoniskDataModel.medlemsNummer,
            kanoniskDataModel.navn,
            kanoniskDataModel.adresse,
            Int32.Parse(kanoniskDataModel.postNr),
            kanoniskDataModel.mailAdresse,
            dato
        );
    }

    static string OmformDato(string dato)
    {
        string[] dele = dato.Split(".");
        return $"{dele[1]}/{dele[0]}/{dele[2]}";
    }
}
