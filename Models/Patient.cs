namespace  digi_sante.Models

{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("patient")]
    public  class Patient
    {
        [Key]
        public long patientID { get; set; }
        public string nom { get; set; }
        public string prenom { get; set; }
        public string adresse { get; set; }
        public long? date_naissance { get; set; }
        public string sexe { get; set; }
        public string telephone_patient { get; set; }
        public DateTime? date_enregistrement { get; set; }
        public string nationnalite { get; set; }
        public string profession { get; set; }
        public string photo { get; set; }
        public string prenom_accompagnant { get; set; }
        public bool? status { get; set; }
        public long? structureID { get; set; }
    }
}
