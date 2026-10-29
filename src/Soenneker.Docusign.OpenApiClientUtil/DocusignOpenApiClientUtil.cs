using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Soenneker.Extensions.Configuration;
using Soenneker.Extensions.ValueTask;
using Soenneker.Docusign.HttpClients.Abstract;
using Soenneker.Docusign.OpenApiClientUtil.Abstract;
using Soenneker.Docusign.OpenApiClient;
using Microsoft.Kiota.Abstractions.Authentication;
using Soenneker.Utils.AsyncSingleton;

namespace Soenneker.Docusign.OpenApiClientUtil;

public sealed class DocusignOpenApiClientUtil : IDocusignOpenApiClientUtil
{
    private readonly AsyncSingleton<DocusignOpenApiClient> _client;

    public DocusignOpenApiClientUtil(IDocusignOpenApiHttpClient httpClientUtil, IConfiguration configuration)
    {
        _client = new AsyncSingleton<DocusignOpenApiClient>(async token =>
        {
            HttpClient httpClient = await httpClientUtil.Get(token).NoSync();

            Uri baseAddress = httpClient.BaseAddress ?? throw new InvalidOperationException("The Docusign HTTP client requires a base address.");
            var authenticationProvider = new BaseBearerTokenAuthenticationProvider(new DocusignAccessTokenProvider(configuration, baseAddress.Host));
            var requestAdapter = new HttpClientRequestAdapter(authenticationProvider, httpClient: httpClient)
            {
                BaseUrl = baseAddress.AbsoluteUri.TrimEnd('/')
            };
            return new DocusignOpenApiClient(requestAdapter);
        });
    }

    public ValueTask<DocusignOpenApiClient> Get(CancellationToken cancellationToken = default)
    {
        return _client.Get(cancellationToken);
    }

    public void Dispose()
    {
        _client.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        return _client.DisposeAsync();
    }
}

