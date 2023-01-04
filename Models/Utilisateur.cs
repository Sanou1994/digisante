namespace digi_sante.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("utilisateur")]
    public  class Utilisateur
    {
        [Key]
        public long  utilisateurID { get; set; }
        public string prenom { get; set; }
        public string nom { get; set; }
        public string titre { get; set; }
        public string adresse { get; set; }
        public string role { get; set; }
        public string pwd { get; set; }
        public bool? status { get; set; }
        public string telephone { get; set; }
        public string email { get; set; }
        public DateTime? date_creation { get; set; }
        public bool? validite_compte_temporaire { get; set; }
        public long? structureID { get; set; }
        public long? departementID { get; set; }
    }
}
