namespace digi_sante.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("structure")]
    public  class Structure
    {
        [Key]
        public long structureID { get; set; }

        public string nom { get; set; }

        public string adresse { get; set; }

        public string email { get; set; }

        public string telephone { get; set; }
        public bool? status { get; set; }
        public bool? status_premiere_connexion_manager { get; set; }
        public string type { get; set; }
        public DateTime? date_creation { get; set; }
      
    }
}
