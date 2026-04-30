// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sustainsys.Saml2.Bindings;
using System.Text.Encodings.Web;

namespace Sustainsys.Saml2.AspNetCore;

/// <summary>
/// Advanced Saml2 Authentication Handler
/// </summary>
/// <param name="options">Saml2 Options</param>
/// <param name="logger"></param>
/// <param name="serviceProvider">Service provider used to resolve services.</param>
/// /// <remarks>
/// Service provider resolver is used instead of injected parameters to improve performance
/// and allow per-scheme service registration through keyed service.
/// </remarks>

public class Saml2HandlerPlus(
    IOptionsMonitor<Saml2Options> options,
    ILoggerFactory logger,
    IServiceProvider serviceProvider)
    : Saml2Handler(options, logger, serviceProvider)
{
    /// <summary>
    /// Gets a required service by first trying keyed service per scheme
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <returns></returns>
    protected override TService GetRequiredService<TService>() =>
        ServiceProvider.GetKeyedService<TService>(Scheme.Name) ??
        ServiceProvider.GetRequiredService<TService>();

    /// <summary>
    /// Get all available front channel bindings with any scheme specific bindings first
    /// </summary>
    /// <returns>Front channel bindings</returns>
    protected override IEnumerable<IFrontChannelBinding> GetAllFrontChannelBindings() =>
    ServiceProvider.GetKeyedServices<IFrontChannelBinding>(Scheme.Name)
    .Union(ServiceProvider.GetServices<IFrontChannelBinding>());

    /// <summary>
    /// Resolves events as keyed service from DI, falls back to creating an events instance.
    /// </summary>
    /// <returns><see cref="Saml2Events"/>Saml2 events instance</returns>
    protected override Task<object> CreateEventsAsync() => Task.FromResult<object>(
        ServiceProvider.GetKeyedService<Saml2Events>(Scheme.Name) ??
        new Saml2Events());

    // TODO: Use event to resolve IdentityProvider - presence of EntityId indicates if challenge or response processing
    // TODO: Event when Xml was created
}