using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace digi_sante.Models
{
    [Table("code")]
    public class Code
    {
        [Key]       
        public long CodeID { get; set; }
        public int Codes { get; set; }
        public long? userConnectedID { get; set; }
        public bool? status { get; set; }
    }
}