// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Commun;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Commun;

/// <summary>Journal d'audit en ajout seul : pas d'audit, pas de suppression logique, PK bigint identity.
/// Le trigger tg_historique_lecture_seule est créé par la migration (SQL brut).</summary>
public sealed class HistoriqueConfiguration : IEntityTypeConfiguration<Historique>
{
    public void Configure(EntityTypeBuilder<Historique> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("historique", "commun", t =>
        {
            t.HasCheckConstraint("historique_action_check", "action IN ('CREATION','MODIFICATION','SUPPRESSION','RESTAURATION','CONSULTATION')");
        });

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseIdentityAlwaysColumn();

        builder.Property(e => e.TypeCible).HasMaxLength(30);
        builder.Property(e => e.Action).HasMaxLength(15);
        builder.Property(e => e.Champ).HasMaxLength(100);
        builder.Property(e => e.DateAction).HasDefaultValueSql("now()");
        builder.Property(e => e.EmpreintePrecedente).HasMaxLength(64).IsFixedLength();
        builder.Property(e => e.Empreinte).HasMaxLength(64).IsFixedLength();

        builder.HasIndex(e => new { e.TypeCible, e.CibleId, e.DateAction }, "ix_historique_cible");
        builder.HasIndex(e => new { e.AgentId, e.DateAction }, "ix_historique_agent");
    }
}
