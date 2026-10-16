// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Commun;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Commun;

public sealed class CommentaireConfiguration : ConfigurationEntiteAuditable<Commentaire>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<Commentaire> builder)
    {
        builder.ToTable("commentaire", "commun");

        builder.Property(e => e.TypeCible).HasMaxLength(30);
        builder.Property(e => e.EstModifie).HasDefaultValueSql("false");

        builder.HasOne(e => e.Parent).WithMany(p => p.Enfants).HasForeignKey(e => e.ParentId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Auteur).WithMany().HasForeignKey(e => e.AuteurId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.TypeCible, e.CibleId, e.CreeLe }, "ix_commentaire_cible");
        builder.HasIndex(e => e.ParentId, "ix_commentaire_parent");
        builder.HasIndex(e => e.AuteurId, "ix_commentaire_auteur");
    }
}
