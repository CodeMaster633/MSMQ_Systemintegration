using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslateTilNyhedsbrev
{
    public class NyhedsbrevModel
    {
        public string membershipNumber;
        public string name;
        public string address;
        public int zipCode;
        public string mail;
        public string dateRegistered;

        public NyhedsbrevModel() { }
        public NyhedsbrevModel(string membershipNumber, string name, string adresse, int zipCode, string mail, string dateRegistered)
        {
            this.membershipNumber = membershipNumber;
            this.name = name;
            this.address = adresse;
            this.zipCode = zipCode;
            this.mail = mail;
            this.dateRegistered = dateRegistered;
        }
    }
}
