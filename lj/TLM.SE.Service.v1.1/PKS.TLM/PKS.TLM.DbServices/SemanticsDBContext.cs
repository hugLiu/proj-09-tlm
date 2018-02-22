namespace PKS.TLM.DbServices
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using Semantics;

    public partial class SemanticsDBContext : DbContext
    {
        public SemanticsDBContext()
            : base("name=SemanticsDBContext")
        {
        }

        public string Schema { get; set; }
        public virtual DbSet<SD_CCTerm> SD_CCTerm { get; set; }
        public virtual DbSet<SD_ConceptClass> SD_ConceptClass { get; set; }
        public virtual DbSet<SD_Semantics> SD_Semantics { get; set; }
        public virtual DbSet<SD_SemanticsType> SD_SemanticsType { get; set; }
        public virtual DbSet<SD_TermKeyword> SD_TermKeyword { get; set; }
        public virtual DbSet<SD_TermTranslation> SD_TermTranslation { get; set; }
        public virtual DbSet<Dict_CommonView> Dict_CommonView { get; set; }
        public virtual DbSet<Dict_ProfessionalRelationView> Dict_ProfessionalRelationView { get; set; }
        public virtual DbSet<Dict_ProfessionalView> Dict_ProfessionalView { get; set; }
        
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (!string.IsNullOrEmpty(Schema))
            {
                modelBuilder.HasDefaultSchema(Schema);
            }

            //modelBuilder.Entity<SD_CCTerm>()
            //    .Property(e => e.CCCode)
            //    .IsUnicode(false);

            //modelBuilder.Entity<SD_CCTerm>()
            //    .Property(e => e.LangCode)
            //    .IsUnicode(false);

            //modelBuilder.Entity<SD_CCTerm>()
            //    .HasMany(e => e.SD_TermTranslation)
            //    .WithRequired(e => e.SD_CCTerm)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<SD_ConceptClass>()
            //    .Property(e => e.CCCode)
            //    .IsUnicode(false);

            //modelBuilder.Entity<SD_ConceptClass>()
            //    .Property(e => e.Tag)
            //    .IsUnicode(false);


            //modelBuilder.Entity<SD_Semantics>()
            //    .Property(e => e.SR)
            //    .IsUnicode(false);

            //modelBuilder.Entity<SD_SemanticsType>()
            //    .Property(e => e.SR)
            //    .IsUnicode(false);

            //modelBuilder.Entity<SD_SemanticsType>()
            //    .Property(e => e.CCCode1)
            //    .IsUnicode(false);

            //modelBuilder.Entity<SD_SemanticsType>()
            //    .Property(e => e.CCCode2)
            //    .IsUnicode(false);

            //modelBuilder.Entity<SD_SemanticsType>()
            //    .HasMany(e => e.SD_Semantics)
            //    .WithRequired(e => e.SD_SemanticsType)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<SD_TermTranslation>()
            //    .Property(e => e.LangCode)
            //    .IsUnicode(false);
        }
    }
}
