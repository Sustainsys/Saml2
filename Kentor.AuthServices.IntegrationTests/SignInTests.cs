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
            Settings.WaitOnAllCommands = false;
        }

        [TestMethod]
        public void SignIn_Unsolicited()
        {
            I.Open("http://localhost:52071/")
                .Enter("http://localhost:2181/AuthServices/Acs").In("#AssertionConsumerServiceUrl")
                .Click("#main form button")
                .Click("a[href=\"/Home/Secure\"]")
                .Assert.Text("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier - JohnDoe").In(".body-content ul li:first-child");

            I.Click("a[href=\"/AuthServices/SignOut\"");
        }
    }
}
