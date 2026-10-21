using Gprojet.Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations;

/// <summary>
/// Colonnes communes à toute <see cref="EntiteAuditable"/> : PK <c>uuidv7()</c>, audit, suppression logique
/// (filtre global <c>!EstSupprime</c>) et concurrence optimiste sur la colonne système <c>xmin</c>.
/// </summary>
public abstract class ConfigurationEntiteAuditable<T> : IEntityTypeConfiguration<T> where T : EntiteAuditable
{
    public void Configure(EntityTypeBuilder<T> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasDefaultValueSql("uuidv7()");
        builder.Property(e => e.CreeLe).HasDefaultValueSql("now()");
        builder.Property(e => e.EstSupprime).HasDefaultValueSql("false");
        builder.Property(e => e.VersionLigne).IsRowVersion(); // Npgsql : uint + row version => xmin
        builder.HasQueryFilter(e => !e.EstSupprime);

        ConfigurerEntite(builder);
    }

    protected abstract void ConfigurerEntite(EntityTypeBuilder<T> builder);
}
