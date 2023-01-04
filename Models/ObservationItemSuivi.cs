namespace digi_sante.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("observationItemSuivi")]
    public  class ObservationItemSuivi
    {
        [Key]
        public long observationItemSuiviID { get; set; }        
        public DateTime? date_observationItemSuivi { get; set; }
        public string observation { get; set; }
        public long? suiviID { get; set; }

    }
}
