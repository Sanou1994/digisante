
namespace digi_sante.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;


    [Table("suivi")]
    public class Suivi
    {
        [Key]
        public long suiviID { get; set; }
        public DateTime? date_suivi { get; set; }
        public long? hospitalisationID { get; set; }
        public long? consultationID { get; set; }

    }
}