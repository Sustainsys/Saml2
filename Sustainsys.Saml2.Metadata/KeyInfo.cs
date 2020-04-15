using Sustainsys.Saml2.Metadata.Tokens;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace Sustainsys.Saml2.Metadata
{
    public abstract class KeyData
    {
    }

    public class X509Digest
    {
        public Uri Algorithm { get; set; }
        public byte[] Value { get; set; }
    }

    public class X509IssuerSerial
    {
        public string Name { get; private set; }
        public string Serial { get; private set; }

        public X509IssuerSerial(string name, string serial)
        {
            Name = name;
            Serial = serial;
        }
    }

    public class X509Data : KeyData
    {
        public X509IssuerSerial IssuerSerial { get; set; }
        public byte[] SKI { get; set; }
        public string SubjectName { get; set; }

        public ICollection<X509Certificate2> Certificates { get; set; } =
            new Collection<X509Certificate2>();

        public byte[] CRL { get; set; }
        public X509Digest Digest { get; set; }
    }

    public class RetrievalMethod
    {
        public Uri Uri { get; set; }
        public Uri Type { get; set; }

        public ICollection<XmlElement> Transforms { get; private set; } =
            new Collection<XmlElement>();
    }

    public abstract class KeyValue
    {
    }

    public class DsaKeyValue : KeyValue
    {
        public DSAParameters Parameters { get; set; }

        public DsaKeyValue(DSAParameters parameters)
        {
            Parameters = parameters;
        }
    }

    public class RsaKeyValue : KeyValue
    {
        public RSAParameters Parameters { get; set; }

        public RsaKeyValue(RSAParameters parameters)
        {
            Parameters = parameters;
        }
    }

#if !NET461

    public class EcKeyValue : KeyValue
    {
        public ECParameters Parameters { get; set; }

        public EcKeyValue(ECParameters parameters)
        {
            Parameters = parameters;
        }
    }

#endif

    public class DSigKeyInfo
    {
        public string Id { get; set; }

        public ICollection<string> KeyNames { get; private set; } =
            new Collection<string>();

        public ICollection<KeyValue> KeyValues { get; private set; } =
            new Collection<KeyValue>();

        public ICollection<RetrievalMethod> RetrievalMethods { get; private set; } =
            new Collection<RetrievalMethod>();

        public ICollection<KeyData> Data { get; private set; } =
            new Collection<KeyData>();

        public SecurityKeyIdentifier MakeSecurityKeyIdentifier()
        {
            var ski = new SecurityKeyIdentifier();
            foreach (var keyValue in KeyValues)
            {
                if (keyValue is RsaKeyValue rsaKeyValue)
                {
                    ski.Add(new RsaKeyIdentifierClause(rsaKeyValue.Parameters));
                }
                else if (keyValue is DsaKeyValue dsaKeyValue)
                {
                    ski.Add(new DsaKeyIdentifierClause(dsaKeyValue.Parameters));
                }
#if !NET461
                else if (keyValue is EcKeyValue ecKeyValue)
                {
                    ski.Add(new EcKeyIdentifierClause(ecKeyValue.Parameters));
                }
#endif
            }
            foreach (string keyName in KeyNames)
            {
                ski.Add(new KeyNameIdentifierClause(keyName));
            }
            foreach (var keyData in Data)
            {
                if (keyData is X509Data x509Data)
                {
                    foreach (var cert in x509Data.Certificates)
                    {
                        ski.Add(new X509RawDataKeyIdentifierClause(cert));
                    }
                    if (x509Data.IssuerSerial != null)
                    {
                        ski.Add(new X509IssuerSerialKeyIdentifierClause(
                            x509Data.IssuerSerial.Name, x509Data.IssuerSerial.Serial));
                    }
                }
            }
            return ski;
        }
    }
}