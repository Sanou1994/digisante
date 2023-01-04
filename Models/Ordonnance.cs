namespace digi_sante.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ordonnance")]
    public  class Ordonnance
    {
        [Key]
        public long ordonnanceID { get; set; }
        public long? structureID { get; set; }
        public long? utilisateurID { get; set; }
        public long? consultationID { get; set; }
        public DateTime? date_ordonnance { get; set; }
    }
}
