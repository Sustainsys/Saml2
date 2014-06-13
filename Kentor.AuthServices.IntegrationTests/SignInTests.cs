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
        public void SignIn_AuthnRequest_MVC()
        {
            I.Open("http://localhost:2181")
                .Click("a[href=\"/Home/Secure\"]")
                .Assert.Text("http://localhost:2181/AuthServices/Acs").In("#AssertionConsumerServiceUrl");

            I.Assert.False(() => string.IsNullOrEmpty(I.Find("#InResponseTo").Element.Value));

            I.Click("#main form button")
                .Assert.Text("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier - JohnDoe").In(".body-content ul li:first-child");

            I.Click("a[href=\"/AuthServices/SignOut\"");
        }

        [TestMethod]
        public void SignIn_AuthnRequest_HttpModule()
        {
            I.Open("http://localhost:17009/SamplePath")
                .Click("a[href=\"/SamplePath/Saml2AuthenticationModule/SignIn\"]")
                .Assert.Text("http://localhost:17009/SamplePath/Saml2AuthenticationModule/Acs").In("#AssertionConsumerServiceUrl");

            I.Assert.False(() => string.IsNullOrEmpty(I.Find("#InResponseTo").Element.Value));

            I.Click("#main form button")
                .Assert.Text("JohnDoe").In("tbody tr td:nth-child(2)");

            I.Click("a[href=\"/SamplePath/Home/SignOut\"");
        }

        [TestMethod]
        public void SignIn_AuthnRequest_MVC_SpecificIdp()
        {
            I.Open("http://localhost:2181/AuthServices/SignIn?idp=Kentor.AuthServices.StubIdp")
                .Assert.Url(u => u.Host == "stubidp.kentor.se");            
        }
    }
}
