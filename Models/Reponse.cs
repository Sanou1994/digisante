using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace digi_sante.Models
{
    public class Reponse
    {
        public string  message {get;set;}
        public int statusCode { get; set; }
        public dynamic resultat { get; set; }
    }
}