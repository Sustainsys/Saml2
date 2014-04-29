using System;
using System.IdentityModel.Metadata;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
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
            throw new NotImplementedException();
        }
    }
}
