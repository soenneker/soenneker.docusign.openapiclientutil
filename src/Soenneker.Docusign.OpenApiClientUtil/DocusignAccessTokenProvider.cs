using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Kiota.Abstractions.Authentication;

namespace Soenneker.Docusign.OpenApiClientUtil;

internal sealed class DocusignAccessTokenProvider(IConfiguration configuration, string host) : IAccessTokenProvider
{
    public AllowedHostsValidator AllowedHostsValidator { get; } = new([host]);

    public Task<string> GetAuthorizationTokenAsync(Uri uri, Dictionary<string, object>? additionalAuthenticationContext = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (uri.Scheme != Uri.UriSchemeHttps || !AllowedHostsValidator.IsUrlHostValid(uri))
            throw new InvalidOperationException("Docusign authentication requires HTTPS and the configured API host.");

        string? token = configuration["Docusign:AccessToken"];
        if (string.IsNullOrWhiteSpace(token))
            throw new InvalidOperationException("Docusign:AccessToken must contain a valid OAuth access token.");

        return Task.FromResult(token);
    }
}

