// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Planification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Planification;

public sealed class BaselineConfiguration : ConfigurationEntiteAuditable<Baseline>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<Baseline> builder)
    {
        builder.ToTable("baseline", "planif");

        builder.Property(e => e.Libelle).HasMaxLength(150);
        builder.Property(e => e.DateCreation).HasDefaultValueSql("now()");

        builder.HasOne(e => e.Projet).WithMany(p => p.Baselines).HasForeignKey(e => e.ProjetId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.ProjetId, "ix_baseline_projet");
    }
}
