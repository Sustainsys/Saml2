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
    /// <summary>
    /// Create an empty Assertion instances
    /// </summary>
    /// <returns>Assertion</returns>
    protected virtual Assertion CreateAssertion() => new();

    /// <inheritdoc/>
    public Assertion ReadAssertion(
        XmlTraverser source,
        Action<ReadErrorInspectorContext<Assertion>>? errorInspector = null)
    {
        var assertion = ReadAssertion(source);

        CallErrorInspector(errorInspector, assertion, source);

        source.ThrowOnErrors();

        return assertion;
    }

    /// <summary>
    /// Read an <see cref="Assertion"/>
    /// </summary>
    /// <param name="source">Xml Traverser to read from</param>
    protected virtual Assertion ReadAssertion(XmlTraverser source)
    {
        var assertion = CreateAssertion();

        if (source.EnsureName(Namespaces.SamlUri, Elements.Assertion))
        {
            ReadAttributes(source, assertion);
            ReadElements(source.GetChildren(), assertion);

            source.MoveNext(true);
        }

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
        assertion.IssueInstance = source.GetRequiredDateTimeAttribute(Attributes.IssueInstant);
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

        // Status is optional on XML schema level, but Core 2.3.3. says that
        // "an assertion without a subject has no defined meaning in this specification."
        if (source.EnsureName(Namespaces.SamlUri, Elements.Subject))
        {
            assertion.Subject = ReadSubject(source);
            source.MoveNext(true);
        }
    }
}
