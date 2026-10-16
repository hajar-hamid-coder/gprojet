// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Planification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Planification;

public sealed class TicketConfiguration : ConfigurationEntiteAuditable<Ticket>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("ticket", "planif", t =>
        {
            t.HasCheckConstraint("ticket_priorite_check", "priorite IN ('BASSE','NORMALE','HAUTE','CRITIQUE')");
            t.HasCheckConstraint("ticket_severite_check", "severite IN ('MINEURE','MAJEURE','CRITIQUE','BLOQUANTE')");
        });

        builder.Property(e => e.Numero).HasMaxLength(40);
        builder.Property(e => e.Titre).HasMaxLength(250);
        builder.Property(e => e.Priorite).HasMaxLength(20).HasDefaultValueSql("'NORMALE'");
        builder.Property(e => e.Severite).HasMaxLength(20);
        builder.Property(e => e.Rang).HasDefaultValueSql("0");
        builder.Property(e => e.EstBloquant).HasDefaultValueSql("false");
        builder.Property(e => e.Environnement).HasMaxLength(50);
        builder.Property(e => e.EstimationH).HasPrecision(8, 2);
        builder.Property(e => e.ResteAFaireH).HasPrecision(8, 2);

        builder.HasOne(e => e.Tache).WithMany(p => p.Tickets).HasForeignKey(e => e.TacheId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Projet).WithMany(p => p.Tickets).HasForeignKey(e => e.ProjetId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.TypeTicket).WithMany().HasForeignKey(e => e.TypeTicketId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Statut).WithMany().HasForeignKey(e => e.StatutId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Assigne).WithMany().HasForeignKey(e => e.AssigneId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(e => e.Rapporteur).WithMany().HasForeignKey(e => e.RapporteurId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.Numero, "ux_ticket_numero").IsUnique();
        builder.HasIndex(e => new { e.TacheId, e.Rang }, "ix_ticket_tache_rang").HasFilter("NOT est_supprime");
        builder.HasIndex(e => e.ProjetId, "ix_ticket_projet");
        builder.HasIndex(e => e.AssigneId, "ix_ticket_assigne").HasFilter("NOT est_supprime");
        builder.HasIndex(e => e.RapporteurId, "ix_ticket_rapporteur");
        builder.HasIndex(e => e.StatutId, "ix_ticket_statut");
        builder.HasIndex(e => e.TypeTicketId, "ix_ticket_type");
    }
}
