namespace digi_sante.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("consultation")]
    public  class Consultation
    {
        [Key]
        public long consultationID { get; set; }
        public DateTime? date_consultation { get; set; }
        public DateTime? date_fin { get; set; }

        public string observation { get; set; }
        public string constant { get; set; }
        public long? patientID { get; set; }
        public long? infirmierID { get; set; }
        public long? departementID { get; set; }
        public long? docteurID { get; set; }
        public bool? status { get; set; }
        public bool? isClose { get; set; }
        public long? structureID { get; set; }
    }
}
