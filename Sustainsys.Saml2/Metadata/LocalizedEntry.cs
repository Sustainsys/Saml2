using System;
using System.Globalization;

namespace Sustainsys.Saml2.Metadata
{
    public abstract class LocalizedEntry
    {
#if TODO
		private CultureInfo language;

		public CultureInfo Language
		{
			get { return language; }
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(language));
				}
				language = value;
			}
		}

		protected LocalizedEntry(CultureInfo language)
		{
			Language = language;
		}
#endif
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
