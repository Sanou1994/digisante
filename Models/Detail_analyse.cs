namespace digi_sante.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    [Table("detail_analyse")]
    public  class Detail_analyse
    {
        [Key]
        public long detail_analyseID { get; set; }
        public long? listeAnalyseID { get; set; }
        public long? analyseID { get; set; }
        public bool? status { get; set; }
        public long? IdLab { get; set; }
        public long? patientID { get; set; }
        public long? consultationID { get; set; }
       

    }
}
