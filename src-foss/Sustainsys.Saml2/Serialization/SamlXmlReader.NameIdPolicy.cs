using Sustainsys.Saml2.Samlp;
using Sustainsys.Saml2.Xml;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;
partial class SamlXmlReader
{
    /// <summary>
    /// Reads a NameIdPolicy.
    /// </summary>
    /// <param name="source">Source data</param>
    /// <returns>read</returns>
    protected NameIdPolicy ReadNameIdPolicy(XmlTraverser source)
    {
        var result = Create<NameIdPolicy>();
        ReadAttributes(source, result);

        return result;
    }

    /// <summary>
    /// Read NameIdPolicy attributes.
    /// </summary>
    /// <param name="source">Source</param>
    /// <param name="nameIdPolicy">NameIdPolicy</param>
    protected virtual void ReadAttributes(XmlTraverser source, NameIdPolicy nameIdPolicy)
    {
        nameIdPolicy.Format = source.GetAbsoluteUriAttribute(Attributes.Format);
        nameIdPolicy.SPNameQualifier = source.GetAttribute(Attributes.SPNameQualifier);
        nameIdPolicy.AllowCreate = source.GetBoolAttribute(Attributes.AllowCreate);
    }
}