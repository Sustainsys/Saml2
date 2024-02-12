using Sustainsys.Saml2.Saml;
using Sustainsys.Saml2.Xml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;

public partial class SamlXmlReader
{
    /// <inheritdoc/>
    public Assertion ReadAssertion(
        XmlTraverser source,
        Action<ReadErrorInspectorContext<Assertion>>? errorInspector = null)
    {
        Assertion assertion = default!;

        if (source.EnsureName(Namespaces.SamlUri, Elements.Assertion))
        {
            assertion = ReadAssertion(source);
            source.MoveNext(true);
        }

        CallErrorInspector(errorInspector, assertion, source);

        source.ThrowOnErrors();

        return assertion;
    }

    /// <summary>
    /// Read an <see cref="Assertion"/>
    /// </summary>
    /// <param name="source">Xml Traverser to read from</param>
    protected Assertion ReadAssertion(XmlTraverser source)
    {
        var assertion = Create<Assertion>();

        ReadAttributes(source, assertion);
        ReadElements(source.GetChildren(), assertion);

        return assertion;
    }

    /// <summary>
    /// Read attributes of an assertion
    /// </summary>
    /// <param name="source">Xml Traverser to read from</param>
    /// <param name="assertion"></param>
    protected virtual void ReadAttributes(XmlTraverser source, Assertion assertion)
    {
        assertion.Id = source.GetRequiredAttribute(Attributes.ID);
        assertion.IssueInstant = source.GetRequiredDateTimeAttribute(Attributes.IssueInstant);
        assertion.Version = source.GetRequiredAttribute(Attributes.Version);
    }

    /// <summary>
    /// Read elements of an assertion
    /// </summary>
    /// <param name="source"></param>
    /// <param name="assertion"></param>
    protected virtual void ReadElements(XmlTraverser source, Assertion assertion)
    {
        source.MoveNext();

        if (source.EnsureName(Namespaces.SamlUri, Elements.Issuer))
        {
            assertion.Issuer = ReadNameId(source);
            source.MoveNext();
        }

        (var trustedSigningKeys, var allowedHashAlgorithms) =
            GetSignatureValidationParametersFromIssuer(source, assertion.Issuer);

        if (source.ReadAndValidateOptionalSignature(trustedSigningKeys, allowedHashAlgorithms, out var trustLevel))
        {
            assertion.TrustLevel = trustLevel;
            source.MoveNext();
        }

        // Status is optional on XML schema level, but Core 2.3.3. says that
        // "an assertion without a subject has no defined meaning in this specification."
        // so we are treating it as mandatory.
        if (source.EnsureName(Namespaces.SamlUri, Elements.Subject))
        {
            assertion.Subject = ReadSubject(source);
            source.MoveNext(true);
        }

        if (source.HasName(Namespaces.SamlUri, Elements.Conditions))
        {
            assertion.Conditions = ReadConditions(source);
            source.MoveNext(true);
        }

        if (source.HasName(Namespaces.SamlUri, Elements.Advice))
        {
            // We're not supporting Advice
            source.IgnoreChildren();
            source.MoveNext(true);
        }

        if (source.HasName(Namespaces.SamlUri, Elements.AuthnStatement))
        {
            assertion.AuthnStatement = ReadAuthnStatement(source);
            source.MoveNext(true);
        }

        if (source.HasName(Namespaces.SamlUri, Elements.AuthzDecisionStatement))
        {
            // Not supporting AuthzDecisionStatement, skip it
            source.IgnoreChildren();
            source.MoveNext(true);
        }

        if (source.HasName(Namespaces.SamlUri, Elements.AttributeStatement))
        {
            var attributes = source.GetChildren();

            while(attributes.MoveNext(true))
            {
                if(attributes.EnsureName(Namespaces.SamlUri, Elements.Attribute))
                {
                    assertion.Attributes.Add(ReadAttribute(attributes));
                }
            }
            
            source.MoveNext(true);
        }
    }
}
