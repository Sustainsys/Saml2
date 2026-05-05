// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB.

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sustainsys.Saml2.AspNetCore;
using Sustainsys.Saml2.Serialization;

// Convention to put the extensions in the Microsoft.Extensions.DependencyInjection namespace
// for visibility.
#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Extension methods to configure Saml2 plus authentication.
/// </summary>
public static class Saml2ExtensionsPlus
{
    /// <summary>
    /// Adds Saml2 support using the default scheme name <see cref="Saml2Defaults.AuthenticationScheme"/>
    /// Using this overload assumes that Saml2 Options have been configured using the configuration
    /// system in separate calls. The normal case is to use <see cref="AddSaml2Plus(AuthenticationBuilder, Action{Saml2Options})"/>
    /// </summary>
    /// <param name="builder">Authentication builder</param>
    /// <returns>Authentication builder</returns>
    public static AuthenticationBuilder AddSaml2Plus(this AuthenticationBuilder builder)
        => builder.AddSaml2Plus(Saml2Defaults.AuthenticationScheme, _ => { });

    /// <summary>
    /// Add Saml2 support using the default scheme name <see cref="Saml2Defaults.AuthenticationScheme"/>
    /// </summary>
    /// <param name="builder">Authentication builder</param>
    /// <param name="configureOptions">Options configurator</param>
    /// <returns>Authentication builder</returns>
    public static AuthenticationBuilder AddSaml2Plus(this AuthenticationBuilder builder, Action<Saml2Options> configureOptions)
        => builder.AddSaml2Plus(Saml2Defaults.AuthenticationScheme, configureOptions);

    /// <summary>
    /// Add Saml2 support using the specified scheme.
    /// </summary>
    /// <param name="builder">Authentication builder</param>
    /// <param name="authenticationScheme">Authentication scheme name</param>
    /// <param name="configureOptions">Options configurator</param>
    /// <returns>Authentication builder</returns>
    public static AuthenticationBuilder AddSaml2Plus(
        this AuthenticationBuilder builder,
        string authenticationScheme,
        Action<Saml2Options> configureOptions)
        => builder.AddSaml2Plus(authenticationScheme, Saml2Defaults.DisplayName, configureOptions);

    /// <summary>
    /// Add Saml2 support using the specified scheme.
    /// </summary>
    /// <param name="builder">Authentication builder</param>
    /// <param name="authenticationScheme">Authentication scheme name</param>
    /// <param name="configureOptions">Options configurator</param>
    /// <param name="displayName">Display name of authentication scheme</param>
    /// <returns>Authentication builder</returns>
    public static AuthenticationBuilder AddSaml2Plus(
        this AuthenticationBuilder builder,
        string authenticationScheme,
        string? displayName,
        Action<Saml2Options> configureOptions)
    {
        builder.Services.AddSaml2CoreServices();

        builder.Services.TryAddSingleton<ISamlXmlWriterPlus, SamlXmlWriterPlus>();
        builder.Services.TryAddSingleton<ISamlXmlWriter>(services => services.GetRequiredService<ISamlXmlWriterPlus>());

        // Reader has state for trusted keys, so it has to be transient.
        builder.Services.TryAddTransient<ISamlXmlReader, SamlXmlReader>();
        builder.Services.TryAddTransient<ISamlXmlReaderPlus, SamlXmlReaderPlus>();

        return builder.AddRemoteScheme<Saml2Options, Saml2HandlerPlus>(authenticationScheme, displayName, configureOptions);
    }
}