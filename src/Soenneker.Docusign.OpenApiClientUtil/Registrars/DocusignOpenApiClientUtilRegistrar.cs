using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Docusign.HttpClients.Registrars;
using Soenneker.Docusign.OpenApiClientUtil.Abstract;

namespace Soenneker.Docusign.OpenApiClientUtil.Registrars;

/// <summary>
/// Registers the OpenAPI client utility for dependency injection.
/// </summary>
public static class DocusignOpenApiClientUtilRegistrar
{
    /// <summary>
    /// Adds <see cref="DocusignOpenApiClientUtil"/> as a singleton service. <para/>
    /// </summary>
    public static IServiceCollection AddDocusignOpenApiClientUtilAsSingleton(this IServiceCollection services)
    {
        services.AddDocusignOpenApiHttpClientAsSingleton()
                .TryAddSingleton<IDocusignOpenApiClientUtil, DocusignOpenApiClientUtil>();

        return services;
    }

    /// <summary>
    /// Adds <see cref="DocusignOpenApiClientUtil"/> as a scoped service. <para/>
    /// </summary>
    public static IServiceCollection AddDocusignOpenApiClientUtilAsScoped(this IServiceCollection services)
    {
        services.AddDocusignOpenApiHttpClientAsSingleton()
                .TryAddScoped<IDocusignOpenApiClientUtil, DocusignOpenApiClientUtil>();

        return services;
    }
}
