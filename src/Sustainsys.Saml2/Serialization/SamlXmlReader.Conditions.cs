using Sustainsys.Saml2.Saml;
using Sustainsys.Saml2.Xml;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;
partial class SamlXmlReader
{
    /// <summary>
    /// Reads Conditions.
    /// </summary>
    /// <param name="source">Source data</param>
    /// <returns>Conditions read</returns>
    protected Conditions ReadConditions(XmlTraverser source)
    {
        var result = Create<Conditions>();

        ReadAttributes(source, result);
        ReadElements(source.GetChildren(), result);

        return result;
    }

    /// <summary>
    /// Extension point to add reading of attributes for Conditions
    /// </summary>
    /// <param name="source">Source</param>
    /// <param name="conditions">Conditions</param>
    protected virtual void ReadAttributes(XmlTraverser source, Conditions conditions)
    {
        conditions.NotBefore = source.GetDateTimeAttribute(Attributes.NotBefore);
    }

    /// <summary>
    /// Reads elements of a Conditions.
    /// </summary>
    /// <param name="source">Source Xml Reader</param>
    /// <param name="conditions">Conditions to populate</param>
    protected virtual void ReadElements(XmlTraverser source, Conditions conditions)
    {
        source.MoveNext(true);

        while (source.HasName(Namespaces.SamlUri, Elements.AudienceRestriction))
        {
            conditions.AudienceRestrictions.Add(ReadAudienceRestriction(source));
            source.MoveNext(true);
        }

        if (source.HasName(Namespaces.SamlUri, Elements.OneTimeUse))
        {
            conditions.OneTimeUse = true;
            source.MoveNext(true);
        }

        if (source.HasName(Namespaces.SamlUri, Elements.ProxyRestriction))
        {
            // TODO: Support proxy restrictions
            source.IgnoreChildren();
            source.MoveNext(true);
        }
    }
}
