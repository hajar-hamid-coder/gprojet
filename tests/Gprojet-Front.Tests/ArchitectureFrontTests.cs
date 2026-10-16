using FluentAssertions;
using Gprojet.Backend.Contracts.Commun;
using Gprojet.Front.Api;

namespace Gprojet.Front.Tests;

public sealed class ArchitectureFrontTests
{
    private static string CsprojFront()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Gprojet.slnx"))) dir = dir.Parent;
        return File.ReadAllText(Path.Combine(dir!.FullName, "src", "Gprojet-Front", "Gprojet-Front.csproj"));
    }

    [Fact]
    public void Le_front_ne_contient_que_des_types_de_Contracts_venant_du_backend()
    {
        var types = ReglesDuFront.TypesDe(typeof(SanteApiClient).Assembly);

        types.Should().Contain(typeof(SanteDto)); // les DTO liés sont bien compilés dans le front
        ReglesDuFront.TypesInterdits(types).Should().BeEmpty();
    }

    [Fact]
    public void Le_csproj_du_front_ne_reference_pas_le_backend() =>
        ReglesDuFront.ReferenceLeBackend(CsprojFront()).Should().BeFalse();

    [Fact]
    public void Garde_fou_la_regle_detecte_une_violation()
    {
        var violations = ReglesDuFront.TypesInterdits([typeof(Gprojet.Backend.Services.ServiceQuiFuiteDansLeFront)]);
        violations.Should().ContainSingle();

        ReglesDuFront.ReferenceLeBackend("""<ProjectReference Include="..\Gprojet-Backend\Gprojet-Backend.csproj" />""")
            .Should().BeTrue();
    }
}
