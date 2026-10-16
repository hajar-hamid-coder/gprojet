// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Organisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Organisation;

public sealed class DelegationConfiguration : ConfigurationEntiteAuditable<Delegation>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<Delegation> builder)
    {
        builder.ToTable("delegation", "org", t =>
        {
            t.HasCheckConstraint("ck_delegation_agents", "delegant_id <> delegataire_id");
            t.HasCheckConstraint("ck_delegation_dates", "date_fin >= date_debut");
        });

        builder.Property(e => e.Motif).HasMaxLength(200);
        builder.Property(e => e.PeutCreerProjet).HasDefaultValueSql("false");
        builder.Property(e => e.PeutAffecter).HasDefaultValueSql("false");
        builder.Property(e => e.PeutValider).HasDefaultValueSql("false");

        builder.HasOne(e => e.Delegant).WithMany().HasForeignKey(e => e.DelegantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Delegataire).WithMany().HasForeignKey(e => e.DelegataireId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Entite).WithMany().HasForeignKey(e => e.EntiteId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.DelegataireId, e.DateDebut, e.DateFin }, "ix_delegation_delegataire");
        builder.HasIndex(e => e.DelegantId, "ix_delegation_delegant");
        builder.HasIndex(e => e.EntiteId, "ix_delegation_entite");
    }
}
