// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Modelisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Modelisation;

public sealed class TemplateGenerationConfiguration : ConfigurationEntiteAuditable<TemplateGeneration>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<TemplateGeneration> builder)
    {
        builder.ToTable("template_generation", "modelisation", t =>
        {
            t.HasCheckConstraint("template_generation_couche_check", "couche IN ('ENTITE','DTO','MAPPING','REPOSITORY','SERVICE','ORCHESTRATION','CONTROLLER','API_CLIENT','FRONT_SERVICE','FRONT_MODEL','PAGE','SQL','DOCKER','CI')");
            t.HasCheckConstraint("template_generation_portee_check", "portee IN ('ENTITE','MODULE','MODELE')");
        });

        builder.Property(e => e.Couche).HasMaxLength(15);
        builder.Property(e => e.Portee).HasMaxLength(10).HasDefaultValueSql("'ENTITE'");
        builder.Property(e => e.Nom).HasMaxLength(100);
        builder.Property(e => e.CheminSortie).HasMaxLength(300);
        builder.Property(e => e.Version).HasDefaultValueSql("1");
        builder.Property(e => e.EstDefaut).HasDefaultValueSql("false");

        builder.HasOne(e => e.Config).WithMany(p => p.TemplateGenerations).HasForeignKey(e => e.ConfigId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.ConfigId, e.Couche }, "ix_template_config_couche");
    }
}
