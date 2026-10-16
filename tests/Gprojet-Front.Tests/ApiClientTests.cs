using System.Net;
using System.Text;
using FluentAssertions;
using Gprojet.Front.Api;

namespace Gprojet.Front.Tests;

[Trait("Category", "Api")]
public class ApiClientTests
{
    private sealed class Simule(Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> f) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage r, CancellationToken ct) =>
            Task.FromResult(f(r, ct));
    }

    private static SanteApiClient Client(HttpStatusCode code, string corps, string type) =>
        Client((_, _) => new HttpResponseMessage(code) { Content = new StringContent(corps, Encoding.UTF8, type) });

    private static SanteApiClient Client(Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> f) =>
        new(new HttpClient(new Simule(f)) { BaseAddress = new Uri("http://localhost/") });

    [Fact]
    public async Task Ok_retourne_le_dto()
    {
        var dto = await Client(HttpStatusCode.OK, """{"statut":"ok"}""", "application/json").LireAsync();
        dto!.Statut.Should().Be("ok");
    }

    [Fact]
    public async Task Requete_relative_vers_api_health()
    {
        Uri? vue = null;
        await Client((r, _) => { vue = r.RequestUri; return new(HttpStatusCode.OK) { Content = new StringContent("""{"statut":"ok"}""", Encoding.UTF8, "application/json") }; }).LireAsync();
        vue!.AbsolutePath.Should().Be("/api/health");
    }

    [Fact]
    public async Task Validation_400_expose_les_erreurs_par_champ()
    {
        var corps = """{"type":"t","title":"Validation","status":400,"detail":"d","errors":{"Nom":["Requis","Trop court"]}}""";
        var ex = (await Assert.ThrowsAsync<ApiException>(() =>
            Client(HttpStatusCode.BadRequest, corps, "application/problem+json").LireAsync()));
        ex.Statut.Should().Be(400);
        ex.Titre.Should().Be("Validation");
        ex.Erreurs["Nom"].Should().Equal("Requis", "Trop court");
    }

    [Fact]
    public async Task Introuvable_404_porte_le_statut()
    {
        var ex = await Assert.ThrowsAsync<ApiException>(() =>
            Client(HttpStatusCode.NotFound, """{"title":"Introuvable","status":404}""", "application/problem+json").LireAsync());
        ex.Statut.Should().Be(404);
    }

    [Fact]
    public async Task Corps_non_json_en_500_donne_le_statut_seul()
    {
        var ex = await Assert.ThrowsAsync<ApiException>(() =>
            Client(HttpStatusCode.InternalServerError, "<html>boom</html>", "text/html").LireAsync());
        ex.Statut.Should().Be(500);
        ex.Erreurs.Should().BeEmpty();
    }

    [Fact]
    public async Task Annulation_propage_OperationCanceledException()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var client = Client((_, ct) => { ct.ThrowIfCancellationRequested(); return new(HttpStatusCode.OK); });
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => client.LireAsync(cts.Token));
    }
}
