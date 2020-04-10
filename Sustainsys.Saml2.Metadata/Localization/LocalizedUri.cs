using System;

namespace Sustainsys.Saml2.Metadata.Localization
{
    public class LocalizedUri : LocalizedEntry
    {
        public Uri Uri { get; set; }

        public LocalizedUri(Uri uri, string language) :
            base(language)
        {
            Uri = uri;
        }

        public LocalizedUri() :
            this(null, null)
        {
        }
    }
}