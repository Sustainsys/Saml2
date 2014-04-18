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
            Settings.DefaultWaitTimeout = TimeSpan.FromSeconds(5);
            Settings.DefaultWaitUntilThreadSleep = TimeSpan.FromSeconds(5);
            Settings.DefaultWaitUntilTimeout = TimeSpan.FromSeconds(5);
        }

        [TestMethod]
        public void SignIn_Unsolicited()
        {
            I.Open("http://localhost:52071/")
                .Click("#main form button")
                .Click("a[href=\"/Home/Secure\"]")
                .Assert.Text("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier - JohnDoe").In(".body-content ul li:first-child");

            I.Click("a[href=\"/AuthServices/SignOut\"");
        }
    }
}
