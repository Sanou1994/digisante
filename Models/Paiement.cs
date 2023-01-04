 namespace digi_sante.Models

{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("paiement")]
    public  class Paiement
    {
        [Key]
        public long paiementID { get; set; }
        public DateTime? date_paiement { get; set; }
        public float? montant_consultation { get; set; }
        public float? montant_espece { get; set; }
        public float? montant_assurance { get; set; }
        public string url_lettre_prise_en_charge { get; set; }
        public string taux_prise_en_charge { get; set; }
        public string observation { get; set; }
        public long? consultationID { get; set; }
        public long? assuranceID { get; set; }
        public long? departementID { get; set; }
        public long? caissierID { get; set; }
        public long? structureID { get; set; }
        public bool? status { get; set; }
        public long? patientID { get; set; }

    }
}
