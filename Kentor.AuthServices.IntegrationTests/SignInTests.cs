using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAutomation;

namespace Kentor.AuthServices.IntegrationTests
{
    [TestClass]
    public class SignInTests : FluentTest
    {
        [TestInitialize]
        public void SignInTestsInitialize()
        {
            SeleniumWebDriver.Bootstrap(SeleniumWebDriver.Browser.Chrome);
            FluentAutomation.FluentSettings.Current.WaitOnAllActions = false;
        }

        [TestMethod]
        public void SignIn_Unsolicited_MVC()
        {
            I.Open("http://localhost:52071/")
                .Enter("http://localhost:2181/AuthServices/Acs").In("#AssertionConsumerServiceUrl")
                .Click("#main form button")
                .Click("a[href=\"/Home/Secure\"]")
                .Assert.Text("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier - JohnDoe").In(".body-content ul li:first-child");

            I.Click("a[href=\"/AuthServices/SignOut\"");
        }

        [TestMethod]
        public void SignIn_Unsolicited_HttpModule()
        {
            I.Open("http://localhost:52071/")
                .Enter("http://localhost:17009/SamplePath/Saml2AuthenticationModule/Acs").In("#AssertionConsumerServiceUrl")
                .Click("#main form button")
                .Assert.Text("JohnDoe").In("tbody tr td:nth-child(2)");

            I.Click("a[href=\"/SamplePath/Home/SignOut\"");
        }

        [TestMethod]
        public void SignIn_AuthnRequest_MVC_via_DiscoveryService()
        {
            I.Open("http://localhost:2181")
                .Click("a[href=\"/Home/Secure\"]")
                .Assert.Text("http://localhost:2181/AuthServices/SignIn?ReturnUrl=%2FHome%2FSecure").In("#return");

            I.Click("#main form button")
                .Assert.Text("http://localhost:2181/AuthServices/Acs").In("#AssertionConsumerServiceUrl");

            I.Assert.False(() => string.IsNullOrEmpty(I.Find("#InResponseTo").Element.Value));

            I.Click("#main form button")
                .Assert.Url("http://localhost:2181/Home/Secure");

            I.Assert.Text("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier - JohnDoe").In(".body-content ul li:first-child");

            I.Click("a[href=\"/AuthServices/SignOut\"");
        }

        [TestMethod]
        public void SignIn_AuthnRequest_HttpModule_via_DiscoveryService()
        {
            I.Open("http://localhost:17009/SamplePath")
                .Click("a[href=\"/SamplePath/Saml2AuthenticationModule/SignIn\"]")
                .Assert.Text("http://localhost:17009/SamplePath/Saml2AuthenticationModule/SignIn").In("#return");

            I.Click("#main form button")
                .Assert.Text("http://localhost:17009/SamplePath/Saml2AuthenticationModule/Acs").In("#AssertionConsumerServiceUrl");

            I.Assert.False(() => string.IsNullOrEmpty(I.Find("#InResponseTo").Element.Value));

            I.Click("#main form button")
                .Assert.Text("JohnDoe").In("tbody tr td:nth-child(2)");

            I.Click("a[href=\"/SamplePath/Home/SignOut\"");
        }

        [TestMethod]
        public void SignIn_AuthnRequest_MVC_SpecificIdp()
        {
            I.Open("http://localhost:2181/AuthServices/SignIn?idp=http%3a%2f%2fstubidp.kentor.se%2fMetadata")
                .Assert.Url(u => u.Host == "stubidp.kentor.se");
        }

        [TestMethod]
        public void SignIn_Unsolicited_Owin()
        {
            I.Open("http://localhost:52071/")
                .Enter("http://localhost:57294/AuthServices/Acs").In("#AssertionConsumerServiceUrl")
                .Enter("SomeUnusedNameId").In("#NameId")
                .Click("#main form button")
                .Assert.Text("You've successfully authenticated with http://localhost:52071/Metadata. Please enter a user name for this site below and click the Register button to finish logging in.")
                .In("p.text-info");
        }

        [TestMethod]
        public void SignIn_AuthnRequest_Owin_via_DiscoveryService()
        {
            I.Open("http://localhost:57294/Account/Login")
                .Click("#KentorAuthServices")
                .Assert.Text("http://localhost:52071/AuthServices/SignIn?ReturnUrl=%2FAccount%2FExternalLoginCallback");

            I.Click("#main form button")
                .Assert.Text("http://localhost:57294/AuthServices/Acs").In("#AssertionConsumerServiceUrl");

            I.Assert.False(() => string.IsNullOrEmpty(I.Find("#InResponseTo").Element.Value));

            I.Enter("SomeUnusedNameId").In("#NameId");

            I.Click("#main form button")
                .Assert.Text("You've successfully authenticated with http://localhost:52071/Metadata. Please enter a user name for this site below and click the Register button to finish logging in.")
                .In("p.text-info");
        }
    }
}
