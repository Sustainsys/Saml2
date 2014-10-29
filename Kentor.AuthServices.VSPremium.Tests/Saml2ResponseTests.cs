using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography;
using Kentor.AuthServices.TestHelpers;
using Microsoft.QualityTools.Testing.Fakes;
using System.Reflection;
using System.Linq;
using FluentAssertions;
using Kentor.AuthServices.Configuration;
using Kentor.AuthServices.Saml2P;

namespace Kentor.AuthServices.VSPremium.Tests
{
    [TestClass]
    public class Saml2ResponseTests
    {
        [TestMethod]
        public void Saml2Response_Validate_FalseOnMissingReferenceInSignature()
        {
            var response =
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = ""Saml2Response_Validate_FalseOnMissingReference"" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Requester"" />
                </saml2p:Status>
            </saml2p:Response>";

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(response);

            var signedXml = new SignedXml(xmlDoc);

            signedXml.SigningKey = (RSACryptoServiceProvider)SignedXmlHelper.TestCert.PrivateKey;

            // The .NET implementation prevents creation of signatures without references (which is good)
            // but for this naughty test we want to create such a signature. Let's replace the real implementation
            // with a shim that bypasses the control. Code copied from reference source at
            // http://referencesource.microsoft.com/#System.Security/cryptography/xml/signedinfo.cs
            using (ShimsContext.Create())
            {
                Func<SignedInfo, XmlDocument, XmlElement> signedInfoGetXml =
                    (SignedInfo signedInfo, XmlDocument document) =>
                    {
                        // Create the root element
                        XmlElement signedInfoElement = document.CreateElement("SignedInfo", SignedXml.XmlDsigNamespaceUrl);
                        if (!String.IsNullOrEmpty(signedInfo.Id))
                            signedInfoElement.SetAttribute("Id", signedInfo.Id);

                        // Add the canonicalization method, defaults to SignedXml.XmlDsigNamespaceUrl

                        // *** GetXml(XmlDocument, string) is internal, call it by reflection. ***
                        // XmlElement canonicalizationMethodElement = signedInfo.CanonicalizationMethodObject.GetXml(document, "CanonicalizationMethod");
                        var transformGetXml = typeof(Transform).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                            .Single(m => m.Name == "GetXml" && m.GetParameters().Length == 2);
                        XmlElement canonicalizationMethodElement = (XmlElement)transformGetXml.Invoke(signedInfo.CanonicalizationMethodObject,
                            new object[] { document, "CanonicalizationMethod" });

                        signedInfoElement.AppendChild(canonicalizationMethodElement);

                        // Add the signature method
                        if (String.IsNullOrEmpty(signedInfo.SignatureMethod))
                            throw new CryptographicException("Cryptography_Xml_SignatureMethodRequired");

                        XmlElement signatureMethodElement = document.CreateElement("SignatureMethod", SignedXml.XmlDsigNamespaceUrl);
                        signatureMethodElement.SetAttribute("Algorithm", signedInfo.SignatureMethod);
                        // Add HMACOutputLength tag if we have one
                        if (signedInfo.SignatureLength != null)
                        {
                            XmlElement hmacLengthElement = document.CreateElement(null, "HMACOutputLength", SignedXml.XmlDsigNamespaceUrl);
                            XmlText outputLength = document.CreateTextNode(signedInfo.SignatureLength);
                            hmacLengthElement.AppendChild(outputLength);
                            signatureMethodElement.AppendChild(hmacLengthElement);
                        }

                        signedInfoElement.AppendChild(signatureMethodElement);

                        //*** This is the part of the original source that we want to bypass. ***
                        //// Add the references
                        //if (m_references.Count == 0)
                        //    throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_ReferenceElementRequired"));

                        //for (int i = 0; i < m_references.Count; ++i) {
                        //    Reference reference = (Reference)m_references[i];
                        //    signedInfoElement.AppendChild(reference.GetXml(document));
                        //}

                        return signedInfoElement;
                    };

                System.Security.Cryptography.Xml.Fakes.ShimSignedInfo.AllInstances.GetXml =
                    (SignedInfo signedInfo) =>
                    {
                        // Copy from SignedInfo.GetXml(XmlDocument)
                        XmlDocument document = new XmlDocument();
                        document.PreserveWhitespace = true;

                        return signedInfoGetXml(signedInfo, document);
                    };

                signedXml.ComputeSignature();

                // Can't call SignedXml.GetXml(); as it calls original SignedInfo.GetXml(XmlDocument). This is
                // pasted / expanded code from SignedXml.GetXml that calls the above shim instead.

                // Create the Signature
                XmlElement signatureElement = (XmlElement)xmlDoc.CreateElement("Signature", SignedXml.XmlDsigNamespaceUrl);
                if (!String.IsNullOrEmpty(signedXml.Signature.Id))
                    signatureElement.SetAttribute("Id", signedXml.Signature.Id);

                // Add the SignedInfo
                if (signedXml.Signature.SignedInfo == null)
                    throw new CryptographicException("Cryptography_Xml_SignedInfoRequired");

                signatureElement.AppendChild(signedInfoGetXml(signedXml.Signature.SignedInfo, xmlDoc));

                // Add the SignatureValue
                if (signedXml.Signature.SignatureValue == null)
                    throw new CryptographicException("Cryptography_Xml_SignatureValueRequired");

                XmlElement signatureValueElement = xmlDoc.CreateElement("SignatureValue", SignedXml.XmlDsigNamespaceUrl);
                signatureValueElement.AppendChild(xmlDoc.CreateTextNode(Convert.ToBase64String(signedXml.Signature.SignatureValue)));

                var m_signatureValueId = (string)typeof(Signature).GetField("m_signatureValueId", BindingFlags.Instance | BindingFlags.NonPublic)
                        .GetValue(signedXml.Signature);

                if (!String.IsNullOrEmpty(m_signatureValueId))
                    signatureValueElement.SetAttribute("Id", m_signatureValueId);

                signatureElement.AppendChild(signatureValueElement);

                // Add the KeyInfo
                if (signedXml.Signature.KeyInfo.Count > 0)
                    signatureElement.AppendChild((XmlElement)typeof(KeyInfo).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                        .Single(m => m.Name == "GetXml" && m.GetParameters().Length == 1)
                        .Invoke(signedXml.Signature.KeyInfo, new object[] { xmlDoc }));

                // Add the Objects
                foreach (Object obj in signedXml.Signature.ObjectList)
                {
                    DataObject dataObj = obj as DataObject;
                    if (dataObj != null)
                    {
                        signatureElement.AppendChild((XmlElement)
                            typeof(DataObject).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                            .Single(m => m.Name == "GetXml" && m.GetParameters().Length == 1)
                            .Invoke(dataObj, new object[] { xmlDoc }));
                    }
                }

                xmlDoc.DocumentElement.AppendChild(xmlDoc.ImportNode(signatureElement, true));

                var samlResponse = Saml2Response.Read(xmlDoc.OuterXml);

                samlResponse.Validate(Options.FromConfiguration).Should().BeFalse();
            }
        }
    }
}
