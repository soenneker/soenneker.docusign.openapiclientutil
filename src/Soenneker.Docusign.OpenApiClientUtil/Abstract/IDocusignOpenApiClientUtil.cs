using Soenneker.Docusign.OpenApiClient;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Docusign.OpenApiClientUtil.Abstract;

/// <summary>
/// Exposes a cached OpenAPI client instance.
/// </summary>
public interface IDocusignOpenApiClientUtil: IDisposable, IAsyncDisposable
{
    /// <summary>Gets the cached eSignature v2.1 client. Docusign:AccessToken is read for each request, allowing
    /// externally refreshed tokens to take effect. Configure Docusign:ClientBaseUrl with the account's base_uri
    /// plus /restapi before creating the client. OAuth token acquisition and refresh are owned by the caller.</summary>
    ValueTask<DocusignOpenApiClient> Get(CancellationToken cancellationToken = default);
}

