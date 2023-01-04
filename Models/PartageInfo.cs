namespace digi_sante.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("partageInfo")]
    public  class PartageInfo
    {
        [Key]
        public long? partageInfoID { get; set; }
        public DateTime? date_partageInfo { get; set; }
        public bool? status { get; set; }
        public long? patientID { get; set; }
        public long? duree_partage { get; set; }
        public long? docteur_primaireID { get; set; }     
        public long? docteur_secondaireID { get; set; }
         public string type_dossier { get; set; }
        public long? id_choisi { get; set; }
        public long? structure_primaireID { get; set; }
        public long? structre_secondaireID { get; set; }            

    }
}
