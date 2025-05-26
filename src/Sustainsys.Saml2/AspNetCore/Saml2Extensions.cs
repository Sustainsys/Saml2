using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Sustainsys.Saml2;
using Sustainsys.Saml2.AspNetCore;
using Sustainsys.Saml2.Bindings;
using Sustainsys.Saml2.Serialization;
using Sustainsys.Saml2.Validation;


// By convention, the extension methods are placed in the Microsoft shared 
// namespace to allow convenient access from intellisense.
#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Extension methods to configure Saml2 authentication.
/// </summary>
public static class Saml2Extensions
{
    /// <summary>
    /// Adds Saml2 support using the default scheme name <see cref="Saml2Defaults.AuthenticationScheme"/>
    /// Using this overload assumes that Saml2 Options have been configured using the configuration
    /// system in separate calls. The normal case is to use <see cref="AddSaml2(AuthenticationBuilder, Action{Saml2Options})"/>
    /// </summary>
    /// <param name="builder">Authentication builder</param>
    /// <returns>Authentication builder</returns>
    public static AuthenticationBuilder AddSaml2(this AuthenticationBuilder builder)
        => builder.AddSaml2(Saml2Defaults.AuthenticationScheme, _ => { });

    /// <summary>
    /// Add Saml2 support using the default scheme name <see cref="Saml2Defaults.AuthenticationScheme"/>
    /// </summary>
    /// <param name="builder">Authentication builder</param>
    /// <param name="configureOptions">Options configurator</param>
    /// <returns>Authentication builder</returns>
    public static AuthenticationBuilder AddSaml2(this AuthenticationBuilder builder, Action<Saml2Options> configureOptions)
        => builder.AddSaml2(Saml2Defaults.AuthenticationScheme, configureOptions);

    /// <summary>
    /// Add Saml2 support using the specified scheme.
    /// </summary>
    /// <param name="builder">Authentication builder</param>
    /// <param name="authenticationScheme">Authentication scheme name</param>
    /// <param name="configureOptions">Options configurator</param>
    /// <returns>Authentication builder</returns>
    public static AuthenticationBuilder AddSaml2(
        this AuthenticationBuilder builder,
        string authenticationScheme,
        Action<Saml2Options> configureOptions)
        => builder.AddSaml2(authenticationScheme, Saml2Defaults.DisplayName, configureOptions);

    /// <summary>
    /// Add Saml2 support using the specified scheme.
    /// </summary>
    /// <param name="builder">Authentication builder</param>
    /// <param name="authenticationScheme">Authentication scheme name</param>
    /// <param name="configureOptions">Options configurator</param>
    /// <param name="displayName">Display name of authentication scheme</param>
    /// <returns>Authentication builder</returns>
    public static AuthenticationBuilder AddSaml2(
        this AuthenticationBuilder builder,
        string authenticationScheme,
        string? displayName,
        Action<Saml2Options> configureOptions)
    {
        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<IPostConfigureOptions<Saml2Options>,
            Saml2PostConfigureOptions>());

        builder.Services.TryAddSingleton<ISamlXmlReader, SamlXmlReader>();
        builder.Services.TryAddSingleton<ISamlXmlWriter, SamlXmlWriter>();

        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<IFrontChannelBinding, HttpRedirectBinding>());
        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<IFrontChannelBinding, HttpPostBinding>());

        builder.Services.TryAddSingleton<IResponseValidator, ResponseValidator>();
        builder.Services.TryAddSingleton<IClaimsFactory, ClaimsFactory>();

        return builder.AddRemoteScheme<Saml2Options, Saml2Handler>(authenticationScheme, displayName, configureOptions);
    }
}