namespace digi_sante.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("liste_analyse")]
    public  class Liste_analyse
    {
        [Key]
        public long liste_analyseID { get; set; }
        public string nom { get; set; }
        public long? id_user { get; set; }
        public bool? status { get; set; }
        public DateTime? date_creation { get; set; }
        public long? structureID { get; set; }
    }
}
