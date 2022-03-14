namespace Sustainsys.Saml2.Metadata
{
    public abstract class LocalizedEntry
    {
        public string Language { get; set; }

        protected LocalizedEntry()
        {
        }

        protected LocalizedEntry(string language)
        {
            Language = language;
        }
    }
}