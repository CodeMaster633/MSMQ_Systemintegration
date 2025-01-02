using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslateTilNyhedsbrev
{
    public class KanoniskDataModel
    {
        public string medlemsNummer;
        public string navn;
        public string adresse;
        public string postNr;
        public string mailAdresse;
        public string dato;

        public KanoniskDataModel() { }
        public KanoniskDataModel(string medlemsNummer, string navn, string adresse, string postNr, string mailAdresse, string dato)
        {
            this.medlemsNummer = medlemsNummer;
            this.navn = navn;
            this.adresse = adresse;
            this.postNr = postNr;
            this.mailAdresse = mailAdresse;
            this.dato = dato;
        }
    }
}
