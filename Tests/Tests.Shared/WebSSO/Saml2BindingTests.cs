using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Web;
using System.Collections.Specialized;
using FluentAssertions;
using System.Collections.Generic;
using Sustainsys.Saml2.WebSso;
using Sustainsys.Saml2.Tests.WebSSO;

namespace Sustainsys.Saml2.Tests.WebSso
{
    [TestClass]
    public class Saml2BindingTests
    {
        [TestMethod]
        public void Saml2Binding_Get_ReturnsSaml2Postbinding()
        {
            var r = new HttpRequestData(
                "POST",
                new Uri("http://example.com"),
                "/ModulePath",
                new KeyValuePair<string, IEnumerable<string>>[]
                {
                    new KeyValuePair<string, IEnumerable<string>>("SAMLResponse", new string[] { "Some Data" })
                },
                null,
                null);

            Saml2Binding.Get(r).Should().BeOfType<Saml2PostBinding>();
        }

        [TestMethod]
        public void Saml2Binding_Get_ReturnsSaml2Artifact_ForArtifactInUrl()
        {
            var r = new HttpRequestData(
                "GET",
                new Uri("http://example.com/ModulePath/Acs?SAMLart=ABCD"));

            Saml2Binding.Get(r).Should().BeOfType<Saml2ArtifactBinding>();
        }

        [TestMethod]
        public void Saml2Binding_Get_ReturnsSamlArtifact_ForArtifactInPost()
        {
            var r = new HttpRequestData(
                "POST",
                new Uri("http://example.com/ModulePath"),
                "/ModulePath",
                new KeyValuePair<string, IEnumerable<string>>[]
                {
                    new KeyValuePair<string, IEnumerable<string>>("SAMLart", new string[] { "Some Data" })
                },
                null,
                null);

            Saml2Binding.Get(r).Should().BeOfType<Saml2ArtifactBinding>();
        }

        [TestMethod]
        public void Saml2Binding_Get_NullOnPlainGet()
        {
            var r = new HttpRequestData("GET", new Uri("http://example.com"));

            Saml2Binding.Get(r).Should().BeNull();
        }

        [TestMethod]
        public void Saml2Binding_Get_NullOnGetWithSamlResponseBody()
        {
            var r = new HttpRequestData(
                "GET",
                new Uri("http://example.com"),
                "/ModulePath",
                new KeyValuePair<string, IEnumerable<string>>[]
                {
                    new KeyValuePair<string, IEnumerable<string>>("SAMLResponse", new string[] { "Some Data" })
                },
                null,
                null);

            Saml2Binding.Get(r).Should().BeNull();
        }

        [TestMethod]
        public void Saml2Binding_Get_NullOnGetWithSamlartBody()
        {
            var r = new HttpRequestData(
                "GET",
                new Uri("http://example.com"),
                "/ModulePath",
                new KeyValuePair<string, IEnumerable<string>>[]
                {
                    new KeyValuePair<string, IEnumerable<string>>("SAMLart", new string[] { "Some Data" })
                },
                null,
                null);

            Saml2Binding.Get(r).Should().BeNull();
        }

        [TestMethod]
        public void Saml2Binding_Get_NullOnPostWithSamlartQuery()
        {
            var r = new HttpRequestData("POST", new Uri("http://example.com?Samlart=foo"));

            Saml2Binding.Get(r).Should().BeNull();
        }

        [TestMethod]
        public void Saml2Binding_Get_NullOnPlainPost()
        {
            var r = new HttpRequestData("POST", new Uri("http://example.com"));

            Saml2Binding.Get(r).Should().BeNull();
        }

        [TestMethod]
        public void Saml2Binding_Get_ExplanatoryExceptionOnUnknownBinding()
        {
            Action a = () => Saml2Binding.Get((Saml2BindingType)1473);

            a.Should().Throw<ArgumentException>()
                .WithMessage("1473 is not a valid value for the Saml2BindingType enum. Have you forgotten to configure a binding for your identity provider?")
                .WithInnerException<KeyNotFoundException>();
        }

        [TestMethod]
        public void Saml2Binding_Bind_IsNotImplemented()
        {
            var message = new Saml2MessageImplementation();

            Action a = () => new StubSaml2Binding().Bind(message);

            a.Should().Throw<NotImplementedException>();
        }

        [TestMethod]
        public void Saml2Binding_Bind_ThrowsNotImplementedException()
        {
            new StubSaml2Binding().Invoking(b => b.Bind(null))
                .Should().Throw<NotImplementedException>();
        }

		class ConcreteSaml2Binding : Saml2Binding
        {
			protected internal override bool CanUnbind(HttpRequestData request)
            {
                throw new NotImplementedException();
            }
		}

        [TestMethod]
        public void Saml2Binding_Unbind_IsNotImplemented()
        {
            Action a = () => new ConcreteSaml2Binding().Unbind(null, null);

            a.Should().Throw<NotImplementedException>();
        }

        [TestMethod]
        public void Saml2Binding_UriToSaml2BindingType_Redirect()
        {
            Saml2Binding.UriToSaml2BindingType(Saml2Binding.HttpRedirectUri)
                .Should().Be(Saml2BindingType.HttpRedirect);
        }

        [TestMethod]
        public void Saml2Binding_UriToSaml2BindingType_Post()
        {
            Saml2Binding.UriToSaml2BindingType(Saml2Binding.HttpPostUri)
                .Should().Be(Saml2BindingType.HttpPost);
        }

        [TestMethod]
        public void Saml2Binding_UriToSaml2BindingType_Unknown()
        {
            Action a = () => Saml2Binding.UriToSaml2BindingType(new Uri("urn:SomeUnknownUri"));

            a.Should().Throw<ArgumentException>().And.Message.Should().Be("Unknown Saml2 Binding Uri \"urn:SomeUnknownUri\".");
        }

        [TestMethod]
        public void Saml2Binding_UriToSaml2BindingType_Nullcheck()
        {
            Action a = () => Saml2Binding.UriToSaml2BindingType(null);

            a.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("uri");
        }

        [TestMethod]
        public void Saml2Binding_Saml2BindingTypeToUri_Artifact()
        {
            Saml2Binding.Saml2BindingTypeToUri(Saml2BindingType.Artifact)
                .Should().Be(Saml2Binding.HttpArtifactUri);
        }

        [TestMethod]
        public void Saml2Binding_Saml2BindingTypeToUri_Post()
        {
            Saml2Binding.Saml2BindingTypeToUri(Saml2BindingType.HttpPost)
                .Should().Be(Saml2Binding.HttpPostUri);
        }

        [TestMethod]
        public void Saml2Binding_Saml2BindingTypeToUri_Unknown()
        {
            Action a = () => Saml2Binding.Saml2BindingTypeToUri(Saml2BindingType.HttpRedirect);

            a.Should().Throw<ArgumentException>().And.Message.Should().Be("Unknown Saml2 Binding Type \"HttpRedirect\".");
        }
    }
}
