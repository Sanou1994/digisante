using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace digi_sante.Models
{
    [Table("structure_utilisateur")]
    public  class Structure_Utilisateur
    {
        [Key]
        public long structure_utilisateurID { get; set; }
        public long? utilisateurID { get; set; }
        public long? structureID { get; set; }

    }
}