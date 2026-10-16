// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Modelisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Modelisation;

public sealed class PatteConfiguration : ConfigurationEntiteAuditable<Patte>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<Patte> builder)
    {
        builder.ToTable("patte", "modelisation", t =>
        {
            t.HasCheckConstraint("patte_card_min_check", "card_min IN (0,1)");
            t.HasCheckConstraint("patte_card_max_check", "card_max IN ('1','N')");
        });

        builder.Property(e => e.CardMax).HasMaxLength(1);
        builder.Property(e => e.Role).HasMaxLength(63);
        builder.Property(e => e.NomNavigation).HasMaxLength(100);
        builder.Property(e => e.EstNavigable).HasDefaultValueSql("true");
        builder.Property(e => e.Ordre).HasDefaultValueSql("1");

        builder.HasOne(e => e.Association).WithMany(p => p.Pattes).HasForeignKey(e => e.AssociationId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Entite).WithMany(p => p.Pattes).HasForeignKey(e => e.EntiteId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.AssociationId, "ix_patte_association");
        builder.HasIndex(e => e.EntiteId, "ix_patte_entite");
    }
}
