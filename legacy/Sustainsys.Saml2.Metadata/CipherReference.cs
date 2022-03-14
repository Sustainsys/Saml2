using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;

namespace Sustainsys.Saml2.Metadata
{
    public class CipherReference
    {
        public Uri Uri { get; set; }

        public ICollection<XmlElement> Transforms { get; private set; } =
            new Collection<XmlElement>();
    }
}