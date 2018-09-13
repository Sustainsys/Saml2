
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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

            var configureOptionsCalled = false;

            builder.AddSaml2(options =>
            {
                configureOptionsCalled = true;
            }).Should().BeSameAs(builder);

            serviceCollection.Should().Contain(
                sc => sc.ImplementationType == typeof(Saml2Handler));

            serviceCollection.Should().Contain(
                sc => sc.ImplementationType == typeof(PostConfigureSaml2Options));

            var authOptions = new AuthenticationOptions();

            serviceCollection.Single(sd => sd.ServiceType == typeof(IConfigureOptions<AuthenticationOptions>))
                .ImplementationInstance.As<IConfigureOptions<AuthenticationOptions>>()
                .Configure(authOptions);

            var saml2Scheme = authOptions.Schemes.Single();

            saml2Scheme.Name.Should().Be(Saml2Defaults.Scheme);
            saml2Scheme.DisplayName.Should().Be(Saml2Defaults.DisplayName);
            saml2Scheme.HandlerType.Should().Be(typeof(Saml2Handler));

            var saml2Options = new Saml2Options();

            serviceCollection.Single(sd => sd.ServiceType == typeof(IConfigureOptions<Saml2Options>))
                .ImplementationInstance.As<ConfigureNamedOptions<Saml2Options>>()
                .Configure(Saml2Defaults.Scheme, saml2Options);

            configureOptionsCalled.Should().BeTrue();
        }

        [TestMethod]
        public void Saml2AuthExtensions_AddSaml2_RegisterSaml2Scheme_CustomScheme()
        {
            var serviceCollection = new ServiceCollection();
            var builder = new AuthenticationBuilder(serviceCollection);

            builder.AddSaml2("CustomScheme", opt => { })
                .Should().BeSameAs(builder);

            var authOptions = new AuthenticationOptions();
            serviceCollection.Single(sd => sd.ServiceType == typeof(IConfigureOptions<AuthenticationOptions>))
                .ImplementationInstance.As<IConfigureOptions<AuthenticationOptions>>()
                .Configure(authOptions);

            authOptions.Schemes.Single().Name.Should().Be("CustomScheme", "scheme should be added with right name");

            serviceCollection.Single(sd => sd.ServiceType == typeof(IConfigureOptions<Saml2Options>))
                .ImplementationInstance.As<ConfigureNamedOptions<Saml2Options>>()
                .Name.Should().Be("CustomScheme", "configuration should be registered for right name");
        }

        [TestMethod]
        public void Saml2AuthExtensions_AddSaml2_RegisterSaml2Scheme_CustomDisplayName()
        {
            var serviceCollection = new ServiceCollection();
            var builder = new AuthenticationBuilder(serviceCollection);

            builder.AddSaml2("CustomScheme", "DisplayName", opt => { })
                .Should().BeSameAs(builder);

            var authOptions = new AuthenticationOptions();
            serviceCollection.Single(sd => sd.ServiceType == typeof(IConfigureOptions<AuthenticationOptions>))
                .ImplementationInstance.As<IConfigureOptions<AuthenticationOptions>>()
                .Configure(authOptions);

            authOptions.Schemes.Single().DisplayName.Should().Be("DisplayName", "scheme should be added with right dispaly name");
        }

        [TestMethod]
        public void Saml2AuthExtensions_AddSaml2_NullCheckBuilder()
        {
            AuthenticationBuilder builder = null;

            builder.Invoking(b => b.AddSaml2(opt => { }))
                .Should().Throw<ArgumentNullException>();
        }
    }
}
