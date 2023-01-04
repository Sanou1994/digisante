namespace digi_sante.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("hospitalisation")]
    public  class Hospitalisation
    {
        [Key]
        public long hospitalisationID { get; set; }

        public DateTime? date_entree { get; set; }

        public DateTime? date_sortie { get; set; }

        public string numero_lit { get; set; }

        public long? consultationID { get; set; }

        public string numero_salle { get; set; }
    }
}
