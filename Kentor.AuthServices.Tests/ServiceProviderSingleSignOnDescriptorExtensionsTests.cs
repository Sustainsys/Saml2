using System;
using System.IdentityModel.Metadata;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Linq;
using System.Xml.Linq;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class ServiceProviderSingleSignOnDescriptorExtensionsTests
    {
        [TestMethod]
        public void ServiceProviderSingleSignOnDescriptorExtensions_ToXElement_NullCheck_SPSSO()
        {
            ServiceProviderSingleSignOnDescriptor spsso = null;

            Action a = () => spsso.ToXElement(null);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("spsso");
        }

        [TestMethod]
        public void ServiceProviderSingleSignOnDescriptorExtensions_ToXElement_NullCheck_ElementName()
        {
            XName xName = null;
            var spsso = new ServiceProviderSingleSignOnDescriptor();

            Action a = () => spsso.ToXElement(xName);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("elementName");
        }

        [TestMethod]
        public void ServiceProviderSingleSignOnDescriptorExtensions_ToXElement_BasicAttributes()
        {
            string sampleAcsUri = "https://some.uri.example.com/acs";

            var acs = new IndexedProtocolEndpoint()
            {
                IsDefault = false,
                Index = 17,
                Binding = Saml2Binding.HttpPostUri,
                Location = new Uri(sampleAcsUri)
            };

            var spsso = new ServiceProviderSingleSignOnDescriptor();
            spsso.AssertionConsumerServices.Add(1, acs);

            var elementName = Saml2Namespaces.Saml2Metadata + "SPSSODescriptor";
            var innerElementName = Saml2Namespaces.Saml2Metadata + "AssertionConsumerService";

            var subject = spsso.ToXElement(elementName);

            subject.Name.Should().Be(elementName);
            subject.Elements().Single().Name.Should().Be(innerElementName);
        }
    }
}
