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
        public void SignInAndOut_IdpInitiated_MVC()
        {
            I.Open("http://localhost:52071/")
                .Enter("http://localhost:2181/AuthServices/Acs").In("#AssertionModel_AssertionConsumerServiceUrl")
                .Enter("http://localhost:2181/AuthServices").In("#AssertionModel_Audience")
                .Click("#binding_artifact")
                .Click("#submit");

            I.Click("a[href=\"/Home/Secure\"]");

            I.Assert.Text("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier - JohnDoe").In(".body-content ul li:first-child");

            I.Open("http://localhost:52071/Logout")
                .Enter("JohnDoe").In("#NameId")
                .Enter("http://localhost:2181/AuthServices/Logout").In("#DestinationUrl")
                .Click("#submit")
                .Assert.Text("urn:oasis:names:tc:SAML:2.0:status:Success").In("#status");

            I.Open("http://localhost:2181/")
                .Assert.Text("not signed in").In("#status");
        }

        [TestMethod]
        public void SignInAndOut_IdpInitiated_HttpModule()
        {
            I.Open("http://localhost:52071/")
                .Enter("http://localhost:17009/SamplePath/AuthServices/Acs").In("#AssertionModel_AssertionConsumerServiceUrl")
                .Enter("http://localhost:17009/SamplePath/AuthServices").In("#AssertionModel_Audience")
                .Click("#submit")
                .Assert.Text("JohnDoe").In("tbody tr td:nth-child(2)");

            I.Open("http://localhost:52071/Logout")
                .Enter("JohnDoe").In("#NameId")
                .Enter("http://localhost:17009/SamplePath/AuthServices/Logout").In("#DestinationUrl")
                .Click("#submit")
                .Assert.Text("urn:oasis:names:tc:SAML:2.0:status:Success").In("#status");

            I.Open("http://localhost:17009/SamplePath")
                .Assert.Text("not signed in").In("#status");
        }

        [TestMethod]
        public void SignInAndOut_SPInitiated_MVC_via_DiscoveryService()
        {
            I.Open("http://localhost:2181")
                .Click("a[href=\"/Home/Secure\"]")
                .Assert.Text(s => s.StartsWith("http://localhost:2181/AuthServices/SignIn?ReturnUrl=%2FHome%2FSecure&RelayState=")).In("#return");

            I.Click("#submit")
                .Assert.Text("http://localhost:2181/AuthServices/Acs").In("#AssertionModel_AssertionConsumerServiceUrl");

            I.Assert.False(() => string.IsNullOrEmpty(I.Find("#AssertionModel_InResponseTo").Element.Value));

            I.Click("#submit")
                .Assert.Url("http://localhost:2181/Home/Secure");

            I.Assert.Text("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier - JohnDoe").In(".body-content ul li:first-child");

            I.Click("a[href=\"/AuthServices/Logout\"")
                .Wait(2);

            I.Enter("http://localhost:2181/AuthServices/Logout").In("#DestinationUrl")
                .Click("#submit")
                .Wait(2);

            I.Assert.Text("not signed in").In("#status");
        }

        [TestMethod]
        public void SignInAndOut_SPInitiated_HttpModule_via_DiscoveryService()
        {
            I.Open("http://localhost:17009/SamplePath")
                .Click("a[href=\"/SamplePath/AuthServices/SignIn\"]")
                .Assert.Text(s => s.StartsWith("http://localhost:17009/SamplePath/AuthServices/SignIn?RelayState=")).In("#return");

            I.Click("#submit")
                .Assert.Text("http://localhost:17009/SamplePath/AuthServices/Acs").In("#AssertionModel_AssertionConsumerServiceUrl");

            I.Assert.False(() => string.IsNullOrEmpty(I.Find("#AssertionModel_InResponseTo").Element.Value));

            I.Click("#submit")
                .Assert.Text("JohnDoe").In("tbody tr td:nth-child(2)");

            I.Click("#logout");

            I.Enter("http://localhost:17009/SamplePath/AuthServices/Logout").In("#DestinationUrl")
                .Click("#submit");

            I.Assert.Text("not signed in").In("#status");
            I.Assert.Url("http://localhost:17009/SamplePath/?Status=LoggedOut");
        }

        [TestMethod]
        public void SignIn_AuthnRequest_MVC_SpecificIdp()
        {
            I.Open("http://localhost:2181/AuthServices/SignIn?idp=http%3a%2f%2fstubidp.kentor.se%2fMetadata")
                .Assert.Url(u => u.Host == "stubidp.kentor.se");
        }

        [TestMethod]
        public void SignInAndOut_IdpInitiated_Owin()
        {
            I.Open("http://localhost:52071/")
                .Enter("http://localhost:57294/AuthServices/Acs").In("#AssertionModel_AssertionConsumerServiceUrl")
                .Enter("IntegrationTestNameId").In("#AssertionModel_NameId")
                .Enter("http://localhost:57294/AuthServices").In("#AssertionModel_Audience");

            I.Click("#submit")
                .Wait(1);

            if (I.Find("#status").Element.Text == "You've successfully authenticated with http://localhost:52071/Metadata. Please enter a user name for this site below and click the Register button to finish logging in.")
            {
                I.Enter("IntegrationTestUser@example.com").In("#Email")
                    .Click("#submit");
            }

            I.Assert.Text("signed in").In("#status");

            I.Open("http://localhost:52071/Logout")
                .Enter("IntegrationTestNameId").In("#NameId")
                .Enter("http://localhost:57294/AuthServices/Logout").In("#DestinationUrl")
                .Click("#submit")
                .Assert.Text("urn:oasis:names:tc:SAML:2.0:status:Success").In("#status");

            I.Open("http://localhost:57294")
                .Assert.Text("not signed in").In("#status");
        }

        [TestMethod]
        public void SignInAndOut_SPInitiated_Owin_via_DiscoveryService()
        {
            I.Open("http://localhost:57294/Account/Login")
                .Click("#KentorAuthServices")
                .Assert.Text(s => s.StartsWith("http://localhost:57294/AuthServices/SignIn?ReturnUrl=%2FAccount%2FExternalLoginCallback&RelayState=")).In("#return");

            I.Click("#submit")
                .Assert.Text("http://localhost:57294/AuthServices/Acs").In("#AssertionModel_AssertionConsumerServiceUrl");

            I.Assert.False(() => string.IsNullOrEmpty(I.Find("#AssertionModel_InResponseTo").Element.Value));

            I.Enter("IntegrationTestNameId").In("#AssertionModel_NameId")
                .Click("#submit")
                .Wait(2);

            if(I.Find("#status").Element.Text == "You've successfully authenticated with http://localhost:52071/Metadata. Please enter a user name for this site below and click the Register button to finish logging in.")
            {
                I.Enter("IntegrationTestUser@example.com").In("#Email")
                    .Click("#submit");
            }
            
            I.Assert.Text("signed in").In("#status");

            I.Click("#logout").Wait(1);

            I.Enter("http://localhost:57294/AuthServices/Logout").In("#DestinationUrl")
                .Click("#submit");

            I.Assert.Text("not signed in").In("#status");
        }
    }
}
