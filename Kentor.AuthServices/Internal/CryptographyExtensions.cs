using System.Collections.Generic;
using System.Security.Cryptography;
using System.Xml;

namespace Kentor.AuthServices.Internal
{
    internal static class CryptographyExtensions
    {
        internal static IEnumerable<XmlElement> Decrypt(this IEnumerable<XmlElement> elements, AsymmetricAlgorithm key)
        {
            foreach (var element in elements)
            {
                // Create an XmlDocument object.
                var xmlDoc = new XmlDocument { PreserveWhitespace = true };
                xmlDoc.LoadXml(element.OuterXml);

                // Create a new EncryptedXml object.
                var exml = new RSAEncryptedXml(xmlDoc, (RSA)key);

                exml.DecryptDocument();

                yield return xmlDoc.DocumentElement;
            }
        } 
    }
}
