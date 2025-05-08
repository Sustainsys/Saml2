using Sustainsys.Saml2.Samlp;
using Sustainsys.Saml2.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;
partial class SamlXmlReader
{
    public virtual IdpList ReadIdpList(XmlTraverser source)
    {
        var result = Create<IdpList>();

        ReadElements(source.GetChildren(), result);

        return result;
    }

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
        if(source.HasName(Elements.GetComplete, Namespaces.SamlpUri))
        {
            result.GetComplete = source.GetTextContents();
            source.MoveNext(true);
        }

    }
}
