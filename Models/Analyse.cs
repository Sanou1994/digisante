using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace digi_sante.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("analyse")]
    public  class Analyse
    {
        [Key]
        public long analyseID { get; set; }
        public long? consultationID { get; set; }
        public long? structureID { get; set; }
        public long? utilisateurID { get; set; }
        public DateTime? date_analyse { get; set; }
        public bool? ResultatEstDisponible { get; set; }
    }
}