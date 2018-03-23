using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Sustainsys.Saml2.Tokens;
using Microsoft.IdentityModel.Xml;

namespace Sustainsys.Saml2.Metadata
{
	public class KeyDescriptor
	{
		public DSigKeyInfo KeyInfo { get; set; }
		public KeyType Use { get; set; } = KeyType.Unspecified;
		public ICollection<EncryptionMethod> EncryptionMethods { get; private set; } =
			new Collection<EncryptionMethod>();

		public KeyDescriptor()
		{
		}
	}
}
