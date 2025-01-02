using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslateTilNyhedsbrev
{
    public class MedlemsregisterModel
    {
        public string medlemsNummer;
        public string fornavn;
        public string efternavn;
        public string adresselinie1;
        public string adresselinie2;
        public string postNr;
        public string byNavn;
        public string mailAdresse;
        public bool privat_firma;
        public bool aktiv_passiv;
        public bool kontigentBetalt;
        public bool nyhedsBrev;
        public string datoBrev;

        public MedlemsregisterModel() { }
        public MedlemsregisterModel(string medlemsNummer, string fornavn, string efternavn, string adresselinie1, string adresselinie2, string postNr, string byNavn, string mailAdresse, bool privat_firma, bool aktiv_passiv, bool kontigentBetalt, bool nyhedsBrev, string datoBrev)
        {
            this.medlemsNummer = medlemsNummer;
            this.fornavn = fornavn;
            this.efternavn = efternavn;
            this.adresselinie1 = adresselinie1;
            this.adresselinie2 = adresselinie2;
            this.postNr = postNr;
            this.byNavn = byNavn;
            this.mailAdresse = mailAdresse;
            this.privat_firma = privat_firma;
            this.aktiv_passiv = aktiv_passiv;
            this.kontigentBetalt = kontigentBetalt;
            this.nyhedsBrev = nyhedsBrev;
            this.datoBrev = datoBrev;
        }
    }
}
