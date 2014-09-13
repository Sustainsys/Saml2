using System;
using System.IdentityModel.Metadata;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Xml.Linq;
using System.Linq;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class EntityDescriptorExtensionsTests
    {
        [TestMethod]
        public void EntityDescriptorExtensions_ToXElement_NullCheck_EntityDescriptor()
        {
            EntityDescriptor entityDescriptor = null;

            Action a = () => entityDescriptor.ToXElement();

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("entityDescriptor");
        }

        [TestMethod]
        public void EntityDescriptorExtensions_ToXElement_Nodes()
        {
            EntityId entityId = new EntityId("http://dummyentityid.com");
            var entityDescriptor = new EntityDescriptor(entityId);
            var spsso = new ServiceProviderSingleSignOnDescriptor();

            string sampleAcsUri = "https://some.uri.example.com/acs";

            var acs = new IndexedProtocolEndpoint()
            {
                IsDefault = false,
                Index = 17,
                Binding = Saml2Binding.HttpPostUri,
                Location = new Uri(sampleAcsUri)
            };

            spsso.AssertionConsumerServices.Add(1, acs);
            entityDescriptor.RoleDescriptors.Add(spsso);

            var rootName = Saml2Namespaces.Saml2Metadata + "EntityDescriptor";
            var elementName = Saml2Namespaces.Saml2Metadata + "SPSSODescriptor";

            var subject = entityDescriptor.ToXElement();

            subject.Name.Should().Be(rootName);
            subject.Elements().Single().Name.Should().Be(elementName);
            subject.Attribute("entityID").Value.Should().Be("http://dummyentityid.com");
        }
    }
}
