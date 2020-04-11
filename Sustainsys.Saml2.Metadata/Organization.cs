using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;

namespace Sustainsys.Saml2.Metadata
{
    public class Organization
    {
        public ICollection<XmlElement> Extensions { get; private set; } =
            new Collection<XmlElement>();

        public LocalizedEntryCollection<LocalizedName> DisplayNames { get; private set; } =
            new LocalizedEntryCollection<LocalizedName>();

        public LocalizedEntryCollection<LocalizedName> Names { get; private set; } =
            new LocalizedEntryCollection<LocalizedName>();

        public LocalizedEntryCollection<LocalizedUri> Urls { get; private set; } =
            new LocalizedEntryCollection<LocalizedUri>();
    }
}