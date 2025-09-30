using Sustainsys.Saml2.Samlp;
using Sustainsys.Saml2.Xml;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;
partial class SamlXmlReader
{
    /// <summary>
    /// Read IdpList.
    /// </summary>
    /// <param name="source">Xml Traverser to read from</param>
    /// <returns>IdpList</returns>
    protected virtual IdpList ReadIdpList(XmlTraverser source)
    {
        var result = Create<IdpList>();

        ReadElements(source.GetChildren(), result);

        return result;
    }
    /// <summary>
    /// Reads elements of a IdpList.
    /// </summary>
    /// <param name="source">Source Xml Reader</param>
    /// <param name="result">Subject to populate</param>
    protected virtual void ReadElements(XmlTraverser source, IdpList result)
    {
        // We require at least one element.
        source.MoveNext(false);

        // There should be at least one Idp Entry.
        if (source.EnsureName(Elements.IDPEntry, Namespaces.SamlpUri))
        {
            // Read IdpEntries as long as we find more.
            do
            {
                result.IdpEntries.Add(ReadIdpEntry(source));
            } while (source.MoveNext(true) && source.HasName(Elements.IDPEntry, Namespaces.SamlpUri));
        }

        // Check if source.HasName GetComplete => read it.
        if (source.HasName(Elements.GetComplete, Namespaces.SamlpUri))
        {
            result.GetComplete = source.GetTextContents();
            source.MoveNext(true);
        }
    }
}