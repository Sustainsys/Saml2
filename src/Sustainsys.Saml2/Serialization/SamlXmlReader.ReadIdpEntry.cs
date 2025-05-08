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
    public virtual IdpEntry ReadIdpEntry(XmlTraverser source)
    {
        var result = Create<IdpEntry>();

        ReadAttributes(source, result);

        return result;
    }

    public virtual void ReadAttributes(XmlTraverser source, IdpEntry result)
    {
        result.ProviderId = source.GetRequiredAbsoluteUriAttribute(Attributes.ProviderID);
        result.Name = source.GetAttribute(Attributes.Name);
        result.Loc = source.GetAbsoluteUriAttribute(Attributes.Loc);
    }
}