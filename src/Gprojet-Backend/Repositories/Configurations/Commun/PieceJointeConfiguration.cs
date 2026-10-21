// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Commun;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Commun;

public sealed class PieceJointeConfiguration : ConfigurationEntiteAuditable<PieceJointe>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<PieceJointe> builder)
    {
        builder.ToTable("piece_jointe", "commun", t =>
        {
            t.HasCheckConstraint("piece_jointe_taille_octets_check", "taille_octets >= 0");
        });

        builder.Property(e => e.TypeCible).HasMaxLength(30);
        builder.Property(e => e.NomFichier).HasMaxLength(255);
        builder.Property(e => e.TypeMime).HasMaxLength(100);
        builder.Property(e => e.CheminStockage).HasMaxLength(500);
        builder.Property(e => e.EmpreinteSha256).HasMaxLength(64).IsFixedLength();

        builder.HasOne(e => e.TeleversePar).WithMany().HasForeignKey(e => e.TeleverseParId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.TypeCible, e.CibleId }, "ix_piece_jointe_cible");
        builder.HasIndex(e => e.TeleverseParId, "ix_piece_jointe_auteur");
    }
}
