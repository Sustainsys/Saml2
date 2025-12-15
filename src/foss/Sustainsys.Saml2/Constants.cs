// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

namespace Sustainsys.Saml2;

/// <summary>
/// Constants
/// </summary>
public static class Constants
{
#pragma warning disable CS1591

    public static class Namespaces
    {
        public const string SamlpUri = "urn:oasis:names:tc:SAML:2.0:protocol";
        public const string Samlp = "samlp";
        public const string SamlUri = "urn:oasis:names:tc:SAML:2.0:assertion";
        public const string Saml = "saml";
        public const string MetadataUri = "urn:oasis:names:tc:SAML:2.0:metadata";
        public const string Metadata = "md";
        public const string Xsi = "xsi";
        public const string XsiUri = "http://www.w3.org/2001/XMLSchema-instance";
    }

    public const string SamlRequest = "SAMLRequest";
    public const string SamlResponse = "SAMLResponse";
    public const string RelayState = "RelayState";
    public const string Saml2Protocol = "urn:oasis:names:tc:SAML:2.0:protocol";

    public const string MetadataContentType = "application/samlmetadata+xml";

    public static class BindingUris
    {
        public const string HttpRedirect = "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect";
        public const string HttpPOST = "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST";
        public const string SOAP = "urn:oasis:names:tc:SAML:2.0:bindings:SOAP";
    }

    public static class StatusCodes
    {
        public const string Requester = "urn:oasis:names:tc:SAML:2.0:status:Requester";
        public const string Responder = "urn:oasis:names:tc:SAML:2.0:status:Responder";
        public const string Success = "urn:oasis:names:tc:SAML:2.0:status:Success";
    }

    /// <summary>
    /// Names of elements
    /// </summary>
    /// <remarks>The naming of the constants are deriberately not following
    /// casing convention in order to be exactly the same as the contents.
    /// </remarks>
    public static class Elements
    {
        public const string Advice = nameof(Advice);
        public const string AttributeAuthorityDescriptor = nameof(AttributeAuthorityDescriptor);
        public const string ArtifactResolutionService = nameof(ArtifactResolutionService);
        public const string Assertion = nameof(Assertion);
        public const string Attribute = nameof(Attribute);
        public const string AttributeStatement = nameof(AttributeStatement);
        public const string AttributeValue = nameof(AttributeValue);
        public const string Audience = nameof(Audience);
        public const string AudienceRestriction = nameof(AudienceRestriction);
        public const string AuthnAuthorityDescriptor = nameof(AuthnAuthorityDescriptor);
        public const string AuthnContext = nameof(AuthnContext);
        public const string AuthnContextClassRef = nameof(AuthnContextClassRef);
        public const string AuthnContextDeclRef = nameof(AuthnContextDeclRef);
        public const string AuthnRequest = nameof(AuthnRequest);
        public const string AuthnStatement = nameof(AuthnStatement);
        public const string AuthzDecisionStatement = nameof(AuthzDecisionStatement);
        public const string Conditions = nameof(Conditions);
        public const string ContactPerson = nameof(ContactPerson);
        public const string EntityDescriptor = nameof(EntityDescriptor);
        public const string Extensions = nameof(Extensions);
        public const string GetComplete = nameof(GetComplete);
        public const string IDPEntry = nameof(IDPEntry);
        public const string IDPList = nameof(IDPList);
        public const string IDPSSODescriptor = nameof(IDPSSODescriptor);
        public const string Issuer = nameof(Issuer);
        public const string KeyDescriptor = nameof(KeyDescriptor);
        public const string KeyInfo = nameof(KeyInfo);
        public const string ManageNameIDService = nameof(ManageNameIDService);
        public const string NameID = nameof(NameID);
        public const string NameIDFormat = nameof(NameIDFormat);
        public const string NameIDPolicy = nameof(NameIDPolicy);
        public const string OneTimeUse = nameof(OneTimeUse);
        public const string Organization = nameof(Organization);
        public const string PDPDescriptor = nameof(PDPDescriptor);
        public const string ProxyRestriction = nameof(ProxyRestriction);
        public const string Response = nameof(Response);
        public const string RequestedAuthnContext = nameof(RequestedAuthnContext);
        public const string RequesterID = nameof(RequesterID);
        public const string RoleDescriptor = nameof(RoleDescriptor);
        public const string Scoping = nameof(Scoping);
        public const string Signature = nameof(Signature);
        public const string SingleLogoutService = nameof(SingleLogoutService);
        public const string SingleSignOnService = nameof(SingleSignOnService);
        public const string SPSSODescriptor = nameof(SPSSODescriptor);
        public const string Status = nameof(Status);
        public const string StatusCode = nameof(StatusCode);
        public const string Subject = nameof(Subject);
        public const string SubjectConfirmation = nameof(SubjectConfirmation);
        public const string SubjectConfirmationData = nameof(SubjectConfirmationData);
        public const string SubjectLocality = nameof(SubjectLocality);
    }

    /// <summary>
    /// Names of attributes.
    /// </summary>
    /// <remarks>The naming of the constants are deriberately not following
    /// casing convention in order to be exactly the same as the contents.
    /// </remarks>
    public static class Attributes
    {
        public const string Address = nameof(Address);
        public const string AllowCreate = nameof(AllowCreate);
        public const string AssertionConsumerServiceIndex = nameof(AssertionConsumerServiceIndex);
        public const string AssertionConsumerServiceURL = nameof(AssertionConsumerServiceURL);
        public const string AuthnInstant = nameof(AuthnInstant);
        public const string Binding = nameof(Binding);
        public const string cacheDuration = nameof(cacheDuration);
        public const string Comparison = nameof(Comparison);
        public const string Consent = nameof(Consent);
        public const string Destination = nameof(Destination);
        public const string entityID = nameof(entityID);
        public const string ForceAuthn = nameof(ForceAuthn);
        public const string Format = nameof(Format);
        public const string ID = nameof(ID);
        public const string index = nameof(index);
        public const string isDefault = nameof(isDefault);
        public const string IsPassive = nameof(IsPassive);
        public const string InResponseTo = nameof(InResponseTo);
        public const string IssueInstant = nameof(IssueInstant);
        public const string Loc = nameof(Loc);
        public const string Location = nameof(Location);
        public const string Method = nameof(Method);
        public const string Name = nameof(Name);
        public const string NotBefore = nameof(NotBefore);
        public const string NotOnOrAfter = nameof(NotOnOrAfter);
        public const string ProtocolBinding = nameof(ProtocolBinding);
        public const string protocolSupportEnumeration = nameof(protocolSupportEnumeration);
        public const string ProviderID = nameof(ProviderID);
        public const string ProviderName = nameof(ProviderName);
        public const string ProxyCount = nameof(ProxyCount);
        public const string Recipient = nameof(Recipient);
        public const string SessionIndex = nameof(SessionIndex);
        public const string SessionNotOnOrAfter = nameof(SessionNotOnOrAfter);
        public const string SPNameQualifier = nameof(SPNameQualifier);
        public const string use = nameof(use);
        public const string validUntil = nameof(validUntil);
        public const string Value = nameof(Value);
        public const string Version = nameof(Version);
        public const string WantAuthnRequestsSigned = nameof(WantAuthnRequestsSigned);
    }

    public static class SubjectConfirmationMethods
    {
        public const string Bearer = "urn:oasis:names:tc:SAML:2.0:cm:bearer";
    }

    public static class NameIdFormats
    {
        public const string Entity = "urn:oasis:names:tc:SAML:2.0:nameid-format:entity";
        public const string Unspecified = "urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified";
    }

    public static class AuthnContextClasses
    {
        public const string PasswordProtectedTransport = "urn:oasis:names:tc:SAML:2.0:ac:classes:PasswordProtectedTransport";
        public const string Unspecified = "urn:oasis:names:tc:SAML:2.0:ac:classes:unspecified";
    }
}