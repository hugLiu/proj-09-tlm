namespace PKS.TLM.DbServices.Semantics
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DICT_PROFESSIONALVIEW")]
    public partial class Dict_ProfessionalView
    {
        [Key]
        [StringLength(255)]
        [Column("TERM")]
        public string term { get; set; }
    }
}
