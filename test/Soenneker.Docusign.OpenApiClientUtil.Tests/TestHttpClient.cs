using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.Docusign.HttpClients.Abstract;

namespace Soenneker.Docusign.OpenApiClientUtil.Tests;

internal sealed class TestHttpClient(HttpClient client) : IDocusignOpenApiHttpClient
{
    public ValueTask<HttpClient> Get(CancellationToken cancellationToken = default) => ValueTask.FromResult(client);
    public void Dispose() { }
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
