// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Organisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Organisation;

public sealed class AgentConfiguration : ConfigurationEntiteAuditable<Agent>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<Agent> builder)
    {
        builder.ToTable("agent", "org", t =>
        {
            t.HasCheckConstraint("agent_capacite_hebdo_h_check", "capacite_hebdo_h BETWEEN 0 AND 80");
        });

        builder.Property(e => e.Matricule).HasMaxLength(30);
        builder.Property(e => e.Nom).HasMaxLength(100);
        builder.Property(e => e.Prenom).HasMaxLength(100);
        builder.Property(e => e.Email).HasMaxLength(254);
        builder.Property(e => e.Telephone).HasMaxLength(30);
        builder.Property(e => e.CapaciteHebdoH).HasPrecision(5, 2).HasDefaultValueSql("40");
        builder.Property(e => e.KeycloakSub).HasMaxLength(100);
        builder.Property(e => e.Actif).HasDefaultValueSql("true");

        builder.HasOne(e => e.Entite).WithMany().HasForeignKey(e => e.EntiteId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Fonction).WithMany().HasForeignKey(e => e.FonctionId).OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(e => e.Matricule, "ux_agent_matricule").IsUnique().HasFilter("NOT est_supprime");
        // shortcut: EF ne modélise pas l'index sur expression lower(email) ; InitialCreate le crée en SQL brut. Toute migration
        // future qui toucherait cet index doit être retouchée à la main (scripts/comparer-schema.py le détecte).
        builder.HasIndex(e => e.Email, "ux_agent_email").IsUnique().HasFilter("NOT est_supprime");
        builder.HasIndex(e => e.KeycloakSub, "ux_agent_keycloak").IsUnique().HasFilter("keycloak_sub IS NOT NULL");
        builder.HasIndex(e => e.EntiteId, "ix_agent_entite");
        builder.HasIndex(e => e.FonctionId, "ix_agent_fonction");
    }
}
