// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Modelisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Modelisation;

public sealed class ConfigGenerationConfiguration : ConfigurationEntiteAuditable<ConfigGeneration>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<ConfigGeneration> builder)
    {
        builder.ToTable("config_generation", "modelisation", t =>
        {
            t.HasCheckConstraint("config_generation_stack_back_check", "stack_back IN ('DOTNET_10')");
            t.HasCheckConstraint("config_generation_stack_front_check", "stack_front IN ('BLAZOR_WASM')");
            t.HasCheckConstraint("config_generation_architecture_check", "architecture IN ('COUCHES_GP')");
        });

        builder.Property(e => e.StackBack).HasMaxLength(20).HasDefaultValueSql("'DOTNET_10'");
        builder.Property(e => e.StackFront).HasMaxLength(20).HasDefaultValueSql("'BLAZOR_WASM'");
        builder.Property(e => e.Architecture).HasMaxLength(20).HasDefaultValueSql("'COUCHES_GP'");
        builder.Property(e => e.DepotGitlabId).HasMaxLength(100);
        builder.Property(e => e.BrancheCible).HasMaxLength(100).HasDefaultValueSql("'generation/socle'");

        builder.HasOne(e => e.Projet).WithMany(p => p.ConfigGenerations).HasForeignKey(e => e.ProjetId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.ProjetId, "ux_config_generation_projet").IsUnique().HasFilter("NOT est_supprime");
    }
}
