namespace digi_sante.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("assurance")]
    public  class Assurance
    {
        [Key]
        public long assuranceID { get; set; }
        public string nom { get; set; }
        public string adresse { get; set; }
        public string telephone { get; set; }
        public string prenom_a_contacter { get; set; }
        public long? structureID { get; set; }
        public bool? status { get; set; }
    }
}
