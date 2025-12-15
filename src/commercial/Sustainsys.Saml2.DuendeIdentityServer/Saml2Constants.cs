// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

namespace Sustainsys.Saml2.DuendeIdentityServer;

/// <summary>
/// Constants for the Saml2 package for Duende IdentityServer
/// </summary>
public static class Saml2Constants
{
    /// <summary>
    /// Protocol type to set on clients.
    /// </summary>
    public const string Saml2Protocol = "Saml2";

    /// <summary>
    /// Profile service caller identifier.
    /// </summary>
    public const string SsoResponseProfileCaller = "Saml2SsoResponseGenerator";

    /// <summary>
    /// Default values
    /// </summary>
    public static class Defaults
    {
        /// <summary>
        /// Default path for Saml2
        /// </summary>
        public const string Saml2Path = "/Saml2";

        /// <summary>
        /// Default path for SSO endpoint
        /// </summary>
        public const string SingleSignOnServicePath = "/Saml2/SSO";
    }


    /// <summary>
    /// Name of endpoints.
    /// </summary>
    public static class EndPoints
    {
        /// <summary>
        /// Name of Saml2 Single Sign On Service endpoint.
        /// </summary>
        public const string SingleSignonService = "Saml2 SingleSignOnService";

        /// <summary>
        /// Name of Saml2 Metadata endpoint.
        /// </summary>
        public const string Metadata = "Saml2 Metadata";
    }
}