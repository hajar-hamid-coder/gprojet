// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Commun;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Commun;

public sealed class ParametreConfiguration : ConfigurationEntiteAuditable<Parametre>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<Parametre> builder)
    {
        builder.ToTable("parametre", "commun", t =>
        {
            t.HasCheckConstraint("parametre_type_valeur_check", "type_valeur IN ('TEXTE','ENTIER','DECIMAL','BOOLEEN','JSON')");
        });

        builder.Property(e => e.Cle).HasMaxLength(100);
        builder.Property(e => e.TypeValeur).HasMaxLength(10);
        builder.Property(e => e.Description).HasMaxLength(300);

        builder.HasIndex(e => e.Cle, "ux_parametre_cle").IsUnique().HasFilter("NOT est_supprime");
    }
}
