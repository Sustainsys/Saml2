using Sustainsys.Saml2.Saml;
using Sustainsys.Saml2.Xml;
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

        if (source.EnsureName(Elements.Assertion, Namespaces.SamlUri))
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

        if (source.EnsureName(Elements.Issuer, Namespaces.SamlUri))
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
        if (source.EnsureName(Elements.Subject, Namespaces.SamlUri))
        {
            assertion.Subject = ReadSubject(source);
            source.MoveNext(true);
        }

        if (source.HasName(Elements.Conditions, Namespaces.SamlUri))
        {
            assertion.Conditions = ReadConditions(source);
            source.MoveNext(true);
        }

        if (source.HasName(Elements.Advice, Namespaces.SamlUri))
        {
            // We're not supporting Advice
            source.IgnoreChildren();
            source.MoveNext(true);
        }

        if (source.HasName(Elements.AuthnStatement, Namespaces.SamlUri))
        {
            assertion.AuthnStatement = ReadAuthnStatement(source);
            source.MoveNext(true);
        }

        if (source.HasName(Elements.AuthzDecisionStatement, Namespaces.SamlUri))
        {
            // Not supporting AuthzDecisionStatement, skip it
            source.IgnoreChildren();
            source.MoveNext(true);
        }

        if (source.HasName(Elements.AttributeStatement, Namespaces.SamlUri))
        {
            var attributes = source.GetChildren();

            while (attributes.MoveNext(true))
            {
                if (attributes.EnsureName(Elements.Attribute, Namespaces.SamlUri))
                {
                    assertion.Attributes.Add(ReadAttribute(attributes));
                }
            }

            source.MoveNext(true);
        }
    }
}