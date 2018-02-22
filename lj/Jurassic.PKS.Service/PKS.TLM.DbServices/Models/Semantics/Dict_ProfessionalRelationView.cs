namespace PKS.TLM.DbServices.Semantics
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DICT_PROFESSIONALRELATIONVIEW")]
    public partial class Dict_ProfessionalRelationView
    {
        [Key]
        [Column("TERM")]
        public string term { get; set; }
    }
}
