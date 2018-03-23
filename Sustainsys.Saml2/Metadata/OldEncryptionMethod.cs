using System;

namespace Sustainsys.Saml2.Metadata
{
#if FALSE
    public class OldEncryptionMethod
    {
		private Uri algorithm;

		public Uri Algorithm
		{
			get
			{
				return algorithm;
			}

			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}
				algorithm = value;
			}
		}

		public EncryptionMethod(Uri algorithm)
		{
			Algorithm = algorithm;
		}
    }
#endif
}
