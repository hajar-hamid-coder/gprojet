// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Projets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Projets;

public sealed class JalonConfiguration : ConfigurationEntiteAuditable<Jalon>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<Jalon> builder)
    {
        builder.ToTable("jalon", "projet", t =>
        {
            t.HasCheckConstraint("jalon_statut_check", "statut IN ('A_VENIR','ATTEINT','EN_RETARD','ANNULE')");
        });

        builder.Property(e => e.Libelle).HasMaxLength(200);
        builder.Property(e => e.Statut).HasMaxLength(20).HasDefaultValueSql("'A_VENIR'");

        builder.HasOne(e => e.Projet).WithMany(p => p.Jalons).HasForeignKey(e => e.ProjetId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Phase).WithMany().HasForeignKey(e => e.PhaseId).OnDelete(DeleteBehavior.SetNull).HasConstraintName("fk_jalon_phase");

        builder.HasIndex(e => new { e.ProjetId, e.DatePrevue }, "ix_jalon_projet_date");
        builder.HasIndex(e => e.PhaseId, "ix_jalon_phase");
    }
}
