using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Kentor.AuthServices.Metadata
{
    // The default KeyInfoSerializer can't handle X509Data elements with
    // multiple child elements. It will only read the first child element and if
    // that doesn't contain the info required to create the key, we're stuck.
    class KeyInfoSerializer : SecurityTokenSerializer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Only called by framework code")]
        protected override SecurityKeyIdentifier ReadKeyIdentifierCore(XmlReader reader)
        {
            var result = new SecurityKeyIdentifier();

            reader.ReadStartElement("KeyInfo", SignedXml.XmlDsigNamespaceUrl);

            while (reader.IsStartElement())
            {
                if (reader.IsStartElement("X509Data", SignedXml.XmlDsigNamespaceUrl))
                {
                    foreach (var clause in ReadX509Data(reader))
                    {
                        result.Add(clause);
                    }
                }
                else
                {
                    if (reader.IsStartElement("KeyValue", SignedXml.XmlDsigNamespaceUrl))
                    {
                        result.Add(ReadRSAKeyValue(reader));
                    }
                    else
                    {
                        if (reader.IsStartElement("KeyName", SignedXml.XmlDsigNamespaceUrl))
                        {
                            result.Add(ReadKeyNameClause(reader));
                        }
                        else
                        {
                            reader.Skip();
                        }
                    }
                }
            }

            reader.ReadEndElement();

            return result;
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private static SecurityKeyIdentifierClause ReadRSAKeyValue(XmlReader reader)
        {
            string rsaXmlElement = reader.ReadInnerXml();

            var rsa = new RSACryptoServiceProvider(); // Do not dispose! Used later when creating key
            rsa.FromXmlString(rsaXmlElement);
            RsaKeyIdentifierClause clause = new RsaKeyIdentifierClause(rsa);

            return clause;
        }

        private static KeyNameIdentifierClause ReadKeyNameClause(XmlReader reader)
        {
            reader.ReadStartElement("KeyName", SignedXml.XmlDsigNamespaceUrl);
            var keyIdentifier = new KeyNameIdentifierClause(reader.ReadContentAsString());
            reader.ReadEndElement();

            return keyIdentifier;
        }

        private static IEnumerable<SecurityKeyIdentifierClause> ReadX509Data(XmlReader reader)
        {
            reader.ReadStartElement("X509Data", SignedXml.XmlDsigNamespaceUrl);

            while (reader.IsStartElement())
            {
                if (reader.IsStartElement("X509Certificate", SignedXml.XmlDsigNamespaceUrl))
                {
                    yield return ReadX509Certificate(reader);
                }
                else
                {
                    if (reader.IsStartElement("X509IssuerSerial", SignedXml.XmlDsigNamespaceUrl))
                    {
                        yield return ReadX509IssuerSerialKeyIdentifierClause(reader);
                    }
                    else
                    {
                        reader.Skip();
                    }
                }
            }
            reader.ReadEndElement();
        }

        private static SecurityKeyIdentifierClause ReadX509IssuerSerialKeyIdentifierClause(XmlReader reader)
        {
            reader.ReadStartElement("X509IssuerSerial", SignedXml.XmlDsigNamespaceUrl);
            reader.ReadStartElement("X509IssuerName", SignedXml.XmlDsigNamespaceUrl);
            string issuerName = reader.ReadContentAsString();
            reader.ReadEndElement();
            reader.ReadStartElement("X509SerialNumber", SignedXml.XmlDsigNamespaceUrl);
            string serialNumber = reader.ReadContentAsString();
            reader.ReadEndElement();
            reader.ReadEndElement();

            return new X509IssuerSerialKeyIdentifierClause(issuerName, serialNumber);
        }

        private static SecurityKeyIdentifierClause ReadX509Certificate(XmlReader reader)
        {
            reader.ReadStartElement("X509Certificate", SignedXml.XmlDsigNamespaceUrl);
            var clause = new X509RawDataKeyIdentifierClause(
                ((XmlDictionaryReader)reader).ReadContentAsBase64());
            reader.ReadEndElement();

            return clause;
        }

        #region overrides throwing NotImplementedException

        [ExcludeFromCodeCoverage]
        protected override bool CanReadKeyIdentifierClauseCore(System.Xml.XmlReader reader)
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        protected override bool CanReadKeyIdentifierCore(System.Xml.XmlReader reader)
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        protected override bool CanReadTokenCore(System.Xml.XmlReader reader)
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        protected override bool CanWriteKeyIdentifierClauseCore(System.IdentityModel.Tokens.SecurityKeyIdentifierClause keyIdentifierClause)
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        protected override bool CanWriteKeyIdentifierCore(System.IdentityModel.Tokens.SecurityKeyIdentifier keyIdentifier)
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        protected override bool CanWriteTokenCore(System.IdentityModel.Tokens.SecurityToken token)
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        protected override System.IdentityModel.Tokens.SecurityKeyIdentifierClause ReadKeyIdentifierClauseCore(System.Xml.XmlReader reader)
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        protected override System.IdentityModel.Tokens.SecurityToken ReadTokenCore(System.Xml.XmlReader reader, SecurityTokenResolver tokenResolver)
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        protected override void WriteKeyIdentifierClauseCore(System.Xml.XmlWriter writer, System.IdentityModel.Tokens.SecurityKeyIdentifierClause keyIdentifierClause)
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        protected override void WriteKeyIdentifierCore(System.Xml.XmlWriter writer, System.IdentityModel.Tokens.SecurityKeyIdentifier keyIdentifier)
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        protected override void WriteTokenCore(System.Xml.XmlWriter writer, System.IdentityModel.Tokens.SecurityToken token)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
