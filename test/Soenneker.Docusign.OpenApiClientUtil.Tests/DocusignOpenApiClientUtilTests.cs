using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Soenneker.Docusign.OpenApiClientUtil.Tests;

public sealed class DocusignOpenApiClientUtilTests
{
    [Test]
    public async Task UsesAccountBaseUrlAndReadsRefreshedToken()
    {
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Docusign:AccessToken"] = "first"
        }).Build();
        using var handler = new RecordingHandler();
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://eu.docusign.net/restapi/") };
        await using var utility = new DocusignOpenApiClientUtil(new TestHttpClient(httpClient), configuration);
        var client = await utility.Get();
        if (!ReferenceEquals(client, await utility.Get()))
            throw new Exception("The client was not cached.");

        await client.V21.Accounts["account"].Envelopes.GetAsync();
        configuration["Docusign:AccessToken"] = "refreshed";
        await client.V21.Accounts["account"].Envelopes.GetAsync();

        if (handler.Requests.Count != 2 || handler.Requests[0].Url != "https://eu.docusign.net/restapi/v2.1/accounts/account/envelopes" ||
            handler.Requests[0].Authorization != "Bearer first" || handler.Requests[1].Authorization != "Bearer refreshed")
            throw new Exception("The request URL or bearer token was incorrect.");
    }

    [Test]
    public async Task RejectsAuthenticationToAnotherHost()
    {
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Docusign:AccessToken"] = "secret"
        }).Build();
        using var handler = new RecordingHandler();
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://demo.docusign.net/restapi/") };
        await using var utility = new DocusignOpenApiClientUtil(new TestHttpClient(httpClient), configuration);
        var client = await utility.Get();
        try
        {
            await client.V21.Accounts["account"].Envelopes.WithUrl("https://other.example/envelopes").GetAsync();
        }
        catch (InvalidOperationException)
        {
            if (handler.Requests.Count != 0)
                throw new Exception("An authenticated request escaped to another host.");
            return;
        }
        throw new Exception("An unexpected host was accepted.");
    }

    [Test]
    public async Task RejectsMissingAccessTokenBeforeSending()
    {
        IConfiguration configuration = new ConfigurationBuilder().Build();
        using var handler = new RecordingHandler();
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://demo.docusign.net/restapi/") };
        await using var utility = new DocusignOpenApiClientUtil(new TestHttpClient(httpClient), configuration);
        var client = await utility.Get();
        try
        {
            await client.V21.Accounts["account"].Envelopes.GetAsync();
        }
        catch (InvalidOperationException)
        {
            if (handler.Requests.Count != 0)
                throw new Exception("A request was sent without a token.");
            return;
        }
        throw new Exception("A missing access token was accepted.");
    }
}
