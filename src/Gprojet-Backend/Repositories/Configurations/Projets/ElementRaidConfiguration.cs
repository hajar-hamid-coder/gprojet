// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Projets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Projets;

public sealed class ElementRaidConfiguration : ConfigurationEntiteAuditable<ElementRaid>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<ElementRaid> builder)
    {
        builder.ToTable("element_raid", "projet", t =>
        {
            t.HasCheckConstraint("element_raid_type_check", "type IN ('RISQUE','ACTION','PROBLEME','DECISION')");
            t.HasCheckConstraint("element_raid_probabilite_check", "probabilite BETWEEN 1 AND 5");
            t.HasCheckConstraint("element_raid_impact_check", "impact BETWEEN 1 AND 5");
            t.HasCheckConstraint("element_raid_statut_check", "statut IN ('OUVERT','EN_COURS','CLOS','ANNULE')");
            t.HasCheckConstraint("ck_raid_risque", "type <> 'RISQUE' OR (probabilite IS NOT NULL AND impact IS NOT NULL)");
        });

        builder.Property(e => e.Type).HasMaxLength(20);
        builder.Property(e => e.Titre).HasMaxLength(250);
        builder.Property(e => e.Criticite).HasComputedColumnSql("probabilite * impact", stored: true);
        builder.Property(e => e.Statut).HasMaxLength(20).HasDefaultValueSql("'OUVERT'");

        builder.HasOne(e => e.Projet).WithMany(p => p.ElementRaids).HasForeignKey(e => e.ProjetId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Responsable).WithMany().HasForeignKey(e => e.ResponsableId).OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(e => new { e.ProjetId, e.Type, e.Statut }, "ix_raid_projet_type");
        builder.HasIndex(e => e.ResponsableId, "ix_raid_responsable");
    }
}
