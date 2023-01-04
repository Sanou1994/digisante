namespace digi_sante.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("resultat")]
    public  class Resultat
    {
        [Key]
        public long resultatID { get; set; }
        public string image { get; set; }
        public string description { get; set; }
        public string commentaire { get; set; }
        public long? redacteurID { get; set; }
        public long? analyseID { get; set; }
        public long? structureID { get; set; }
    }
}
