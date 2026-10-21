// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Commun;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Commun;

public sealed class NotificationConfiguration : ConfigurationEntiteAuditable<Notification>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("notification", "commun");

        builder.Property(e => e.Type).HasMaxLength(50);
        builder.Property(e => e.Titre).HasMaxLength(200);
        builder.Property(e => e.Lien).HasMaxLength(500);
        builder.Property(e => e.EstLue).HasDefaultValueSql("false");

        builder.HasOne(e => e.Destinataire).WithMany(p => p.Notifications).HasForeignKey(e => e.DestinataireId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.DestinataireId, e.CreeLe }, "ix_notification_non_lues").IsDescending(false, true).HasFilter("NOT est_lue AND NOT est_supprime");
    }
}
