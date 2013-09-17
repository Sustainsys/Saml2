using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class CommandFactoryTests
    {
        [TestMethod]
        public void CommandFactory_Invalid_ReturnsNotFound()
        {
            CommandFactory.GetCommand("Invalid").Should().BeOfType<NotFoundCommand>();
        }

        [TestMethod]
        public void CommandFactory_SignIn_ReturnsSignIn()
        {
            CommandFactory.GetCommand("SignIn").Should().BeOfType<SignInCommand>();
        }
    }
}
