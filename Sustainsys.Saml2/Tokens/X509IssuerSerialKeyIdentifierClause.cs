using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Numerics;

namespace Sustainsys.Saml2.Tokens
{
    public class X509IssuerSerialKeyIdentifierClause : SecurityKeyIdentifierClause
    {
		public string IssuerName { get; private set; }
		public string IssuerSerialNumber { get; private set; }

		static string AsDecimal(byte[] number)
		{
			return new BigInteger(number).ToString();
		}

		public X509IssuerSerialKeyIdentifierClause(X509Certificate2 certificate) :
			base(null)
		{
			if (certificate == null)
			{
				throw new ArgumentNullException(nameof(certificate));
			}
			IssuerName = certificate.IssuerName.Name;
			IssuerSerialNumber = AsDecimal(certificate.GetSerialNumber());
		}

		public X509IssuerSerialKeyIdentifierClause(string issuerName, string issuerSerialNumber) :
			base(null)
		{
			IssuerName = issuerName;
			IssuerSerialNumber = issuerSerialNumber;
		}

		public override bool Matches(SecurityKeyIdentifierClause keyIdentifierClause)
		{
			if (keyIdentifierClause == null)
			{
				throw new ArgumentNullException(nameof(keyIdentifierClause));
			}
			return keyIdentifierClause is X509IssuerSerialKeyIdentifierClause otherClause &&
				Matches(otherClause.IssuerName, otherClause.IssuerSerialNumber);
		}

		public bool Matches(string issuerName, string issuerSerialNumber)
		{
			return IssuerName == issuerName && IssuerSerialNumber == issuerSerialNumber;
		}

		public bool Matches(X509Certificate2 certificate)
		{
			return Matches(certificate.IssuerName.Name, AsDecimal(certificate.GetSerialNumber()));
		}
	}
}
