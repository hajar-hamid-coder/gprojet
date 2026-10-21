// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Modelisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Modelisation;

public sealed class ExecutionGenerationConfiguration : ConfigurationEntiteAuditable<ExecutionGeneration>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<ExecutionGeneration> builder)
    {
        builder.ToTable("execution_generation", "modelisation", t =>
        {
            t.HasCheckConstraint("execution_generation_statut_check", "statut IN ('EN_COURS','SUCCES','ECHEC')");
        });

        builder.Property(e => e.DateExecution).HasDefaultValueSql("now()");
        builder.Property(e => e.Statut).HasMaxLength(10).HasDefaultValueSql("'EN_COURS'");
        builder.Property(e => e.Branche).HasMaxLength(100);
        builder.Property(e => e.CommitSha).HasMaxLength(40).IsFixedLength();
        builder.Property(e => e.Rapport).HasColumnType("jsonb");

        builder.HasOne(e => e.Modele).WithMany(p => p.ExecutionGenerations).HasForeignKey(e => e.ModeleId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Lanceur).WithMany().HasForeignKey(e => e.LanceurId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.ModeleId, e.DateExecution }, "ix_execution_modele").IsDescending(false, true);
        builder.HasIndex(e => e.LanceurId, "ix_execution_lanceur");
    }
}
