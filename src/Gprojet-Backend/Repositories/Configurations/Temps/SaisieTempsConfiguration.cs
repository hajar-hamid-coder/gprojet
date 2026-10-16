// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Temps;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Temps;

public sealed class SaisieTempsConfiguration : ConfigurationEntiteAuditable<SaisieTemps>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<SaisieTemps> builder)
    {
        builder.ToTable("saisie_temps", "temps", t =>
        {
            t.HasCheckConstraint("saisie_temps_duree_h_check", "duree_h > 0 AND duree_h <= 24");
            t.HasCheckConstraint("saisie_temps_source_check", "source IN ('MANUEL','CHRONO','GITLAB')");
            t.HasCheckConstraint("ck_saisie_ticket_tache", "ticket_id IS NULL OR tache_id IS NOT NULL");
            t.HasCheckConstraint("ck_saisie_tache_projet", "tache_id IS NULL OR projet_id IS NOT NULL");
            t.HasCheckConstraint("ck_saisie_chrono", "chrono_fin IS NULL OR chrono_debut IS NULL OR chrono_fin >= chrono_debut");
        });

        builder.Property(e => e.DureeH).HasPrecision(5, 2);
        builder.Property(e => e.Commentaire).HasMaxLength(500);
        builder.Property(e => e.Source).HasMaxLength(10).HasDefaultValueSql("'MANUEL'");
        builder.Property(e => e.ReferenceGitlab).HasMaxLength(100);

        builder.HasOne(e => e.Feuille).WithMany(p => p.SaisieTemps).HasForeignKey(e => e.FeuilleId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Agent).WithMany().HasForeignKey(e => e.AgentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Categorie).WithMany().HasForeignKey(e => e.CategorieId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Projet).WithMany().HasForeignKey(e => e.ProjetId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Tache).WithMany().HasForeignKey(e => e.TacheId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Ticket).WithMany().HasForeignKey(e => e.TicketId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.FeuilleId, "ix_saisie_feuille");
        builder.HasIndex(e => new { e.AgentId, e.DateSaisie }, "ix_saisie_agent_date");
        builder.HasIndex(e => e.ProjetId, "ix_saisie_projet").HasFilter("projet_id IS NOT NULL");
        builder.HasIndex(e => e.TacheId, "ix_saisie_tache").HasFilter("tache_id  IS NOT NULL");
        builder.HasIndex(e => e.TicketId, "ix_saisie_ticket").HasFilter("ticket_id IS NOT NULL");
        builder.HasIndex(e => e.CategorieId, "ix_saisie_categorie");
    }
}
