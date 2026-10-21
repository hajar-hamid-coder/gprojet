// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Organisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Organisation;

public sealed class JourFerieConfiguration : ConfigurationEntiteAuditable<JourFerie>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<JourFerie> builder)
    {
        builder.ToTable("jour_ferie", "org");

        builder.Property(e => e.Libelle).HasMaxLength(150);

        builder.HasIndex(e => e.DateFerie, "ux_jour_ferie_date").IsUnique().HasFilter("NOT est_supprime");
    }
}
