using Sustainsys.Saml2.Samlp;
using Sustainsys.Saml2.Xml;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;

partial class SamlXmlReader
{
    /// <summary>
    /// Read a IdpEntry.
    /// </summary>
    /// <param name="source">Xml Traverser to read from</param>
    /// <returns>IdpEntry</returns>
    protected virtual IdpEntry ReadIdpEntry(XmlTraverser source)
    {
        var result = Create<IdpEntry>();

        ReadAttributes(source, result);

        return result;
    }
    /// <summary>
    /// Read IdpEntry attributes.
    /// </summary>
    /// <param name="source">Source</param>
    /// <param name="result">result</param>
    protected virtual void ReadAttributes(XmlTraverser source, IdpEntry result)
    {
        result.ProviderId = source.GetRequiredAbsoluteUriAttribute(Attributes.ProviderID);
        result.Name = source.GetAttribute(Attributes.Name);
        result.Loc = source.GetAbsoluteUriAttribute(Attributes.Loc);
    }
}