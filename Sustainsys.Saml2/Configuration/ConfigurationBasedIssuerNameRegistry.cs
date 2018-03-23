using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.IdentityModel.Tokens;

namespace Sustainsys.Saml2.Configuration
{
	public class ThumbprintKeyComparer : IEqualityComparer<string>
	{
		public bool Equals(string x, string y)
		{
			return String.Equals(x, y, StringComparison.OrdinalIgnoreCase);
		}

		public int GetHashCode(string obj)
		{
			return obj.ToUpper(CultureInfo.InvariantCulture).GetHashCode();
		}
	}

	public class ConfigurationBasedIssuerNameRegistry : IssuerNameRegistry
    {
		public IDictionary<string, string> ConfiguredTrustedIssuers { get; private set; } =
			new Dictionary<string, string>(new ThumbprintKeyComparer());

		public override void LoadCustomConfiguration(XmlNodeList nodeList)
		{
			// <trustedIssuers>
			// <add thumbprint='ASN.1Thumbprint' name='Name'/>
			// <remove thumbprint='..'/>
			// <clear/>
			// </trustedIssuers>
			if (nodeList == null)
			{
				throw new ArgumentNullException(nameof(nodeList));
			}

			var rootNodes = nodeList.OfType<XmlElement>().ToArray();
			if (rootNodes.Length != 1 ||
				!String.Equals(rootNodes[0].Name, "trustedIssuers", StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException("Expected a <trustedIssuers> node");
			}
			foreach (var actionEl in rootNodes[0].ChildNodes.OfType<XmlElement>())
			{
				if (String.Equals(actionEl.Name, "add", StringComparison.OrdinalIgnoreCase))
				{
					var thumb = actionEl.Attributes["thumbprint"];
					var name = actionEl.Attributes["name"];
					if (thumb == null || String.IsNullOrEmpty(thumb.Value) || actionEl.Attributes.Count > 2)
					{
						throw new InvalidOperationException($"Found an <add> element with unexpected attributes -- should only have thumbprint and name");
					}
					ConfiguredTrustedIssuers.Add(thumb.Value.Replace(" ", ""), name?.Value ?? "");
				}
				else if (String.Equals(actionEl.Name, "remove", StringComparison.OrdinalIgnoreCase))
				{
					var thumb = actionEl.Attributes["thumbprint"];
					if (thumb == null || String.IsNullOrEmpty(thumb.Value) || actionEl.Attributes.Count > 1)
					{
						throw new InvalidOperationException($"Found an <remove> element with unexpected attributes -- should only have thumbprint");
					}
					ConfiguredTrustedIssuers.Remove(thumb.Value.Replace(" ", ""));
				}
				else if (String.Equals(actionEl.Name, "clear", StringComparison.OrdinalIgnoreCase))
				{
					if (actionEl.Attributes.Count > 0)
					{
						throw new InvalidOperationException("Found a <clear> element with unexpected attributes -- should have none");
					}
					ConfiguredTrustedIssuers.Clear();
				}
				else
				{
					throw new InvalidOperationException($"Found an unexpected <{actionEl.Name}> element");
				}
			}
		}

		public override string GetIssuerName(SecurityToken securityToken)
		{
			if (securityToken == null)
			{
				throw new ArgumentNullException(nameof(securityToken));
			}
			// TODO
			//if (securityToken is X509SecurityKey

			throw new NotImplementedException();
		}

		public void AddTrustedIssuer(string thumbprint, string name)
		{
			if (String.IsNullOrEmpty(thumbprint))
			{
				throw new ArgumentNullException(nameof(thumbprint));
			}
			if (String.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException(nameof(name));
			}
			thumbprint = thumbprint.Replace(" ", "");
			if (ConfiguredTrustedIssuers.ContainsKey(thumbprint))
			{
				throw new InvalidOperationException($"The certificate with thumbprint {thumbprint} is already registered");
			}
			ConfiguredTrustedIssuers.Add(thumbprint, name);
		}
	}
}
