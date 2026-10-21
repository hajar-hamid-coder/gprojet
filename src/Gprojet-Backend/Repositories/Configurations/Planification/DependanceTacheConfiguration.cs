// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Planification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Planification;

public sealed class DependanceTacheConfiguration : ConfigurationEntiteAuditable<DependanceTache>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<DependanceTache> builder)
    {
        builder.ToTable("dependance_tache", "planif", t =>
        {
            t.HasCheckConstraint("dependance_tache_type_lien_check", "type_lien IN ('FIN_DEBUT','DEBUT_DEBUT','FIN_FIN','DEBUT_FIN')");
            t.HasCheckConstraint("ck_dependance_differente", "predecesseur_id <> successeur_id");
        });

        builder.Property(e => e.TypeLien).HasMaxLength(15).HasDefaultValueSql("'FIN_DEBUT'");
        builder.Property(e => e.DelaiJ).HasPrecision(6, 2).HasDefaultValueSql("0");

        builder.HasOne(e => e.Predecesseur).WithMany().HasForeignKey(e => e.PredecesseurId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Successeur).WithMany().HasForeignKey(e => e.SuccesseurId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.PredecesseurId, e.SuccesseurId }, "ux_dependance").IsUnique().HasFilter("NOT est_supprime");
        builder.HasIndex(e => e.SuccesseurId, "ix_dependance_successeur");
    }
}
