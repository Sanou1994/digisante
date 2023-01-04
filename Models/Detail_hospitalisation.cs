namespace digi_sante.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    [Table("detail_hospitalisation")]
    public  class Detail_hospitalisation
    {
        [Key]
        public long detail_hospitalisationID { get; set; }
        public long hospitalisationID { get; set; }
        public bool? status { get; set; }
        public long? patientID { get; set; }
        public long? consultationID { get; set; }


    }
}
