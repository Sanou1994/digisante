namespace digi_sante.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("detail_ordonnance")]
    public  class Detail_ordonnance
    {
        [Key]
        public long detail_ordonnanceID { get; set; }
        public string description { get; set; }
        public long? ordonnanceID { get; set; }
        public long ? patientID { get; set; }
        public bool? status { get; set; }
        public long? consultationID { get; set; }

    }
}
