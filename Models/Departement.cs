namespace digi_sante.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("departement")]
    public  class Departement
    {
        [Key]
        public long departementID { get; set; }
        public string nom { get; set; }
        public long? structureID { get; set; }
        public bool? status { get; set; }

    }
}
