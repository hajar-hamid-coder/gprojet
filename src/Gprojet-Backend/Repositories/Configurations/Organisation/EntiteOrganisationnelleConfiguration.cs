// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Organisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Organisation;

public sealed class EntiteOrganisationnelleConfiguration : ConfigurationEntiteAuditable<EntiteOrganisationnelle>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<EntiteOrganisationnelle> builder)
    {
        builder.ToTable("entite_organisationnelle", "org", t =>
        {
            t.HasCheckConstraint("entite_organisationnelle_niveau_check", "niveau IN ('DIRECTION','DIVISION','SERVICE')");
            t.HasCheckConstraint("ck_entite_parent_different", "parent_id IS NULL OR parent_id <> id");
        });

        builder.Property(e => e.Code).HasMaxLength(30);
        builder.Property(e => e.Libelle).HasMaxLength(200);
        builder.Property(e => e.Sigle).HasMaxLength(30);
        builder.Property(e => e.Niveau).HasMaxLength(20);
        builder.Property(e => e.Chemin).HasColumnType("ltree");
        builder.Property(e => e.Actif).HasDefaultValueSql("true");

        builder.HasOne(e => e.Parent).WithMany().HasForeignKey(e => e.ParentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Responsable).WithMany().HasForeignKey(e => e.ResponsableId).OnDelete(DeleteBehavior.SetNull).HasConstraintName("fk_entite_responsable");

        builder.HasIndex(e => e.Code, "ux_entite_code").IsUnique().HasFilter("NOT est_supprime");
        builder.HasIndex(e => e.ResponsableId, "ux_entite_responsable").IsUnique().HasFilter("responsable_id IS NOT NULL AND NOT est_supprime");
        builder.HasIndex(e => e.ParentId, "ix_entite_parent");
        builder.HasIndex(e => e.Chemin, "ix_entite_chemin").HasMethod("gist");
    }
}
