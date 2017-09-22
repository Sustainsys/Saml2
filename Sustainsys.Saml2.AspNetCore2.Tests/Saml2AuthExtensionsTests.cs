
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Sustainsys.Saml2.AspNetCore2.Tests
{
    [TestClass]
    public class Saml2AuthExtensionsTests
    {
        [TestMethod]
        public void Saml2AuthExtensions_AddSaml2_RegistersSaml2Scheme_DefaultScheme()
        {
            var serviceCollection = new ServiceCollection();
            var builder = new AuthenticationBuilder(serviceCollection);

            builder.AddSaml2(options => { });

            serviceCollection.Should().Contain(
                sd => sd.ImplementationType == typeof(Saml2Handler));

            serviceCollection.Should().Contain(
                sd => sd.ImplementationType == typeof(PostConfigureSaml2Options));
        }

        [TestMethod]
        public void Saml2AuthExtensions_AddSaml2_NullCheckBuilder()
        {
            AuthenticationBuilder builder = null;

            builder.Invoking(b => b.AddSaml2(opt => { }))
                .ShouldThrow<ArgumentNullException>();
        }
    }
}
