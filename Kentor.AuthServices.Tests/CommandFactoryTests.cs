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

        [TestMethod]
        public void CommandFactory_IsCaseInsensitive()
        {
          CommandFactory.GetCommand("signin").Should().BeOfType<SignInCommand>();
        }

        [TestMethod]
        public void CommandFactory_Acs_ReturnsAcs()
        {
            CommandFactory.GetCommand("Acs").Should().BeOfType<AcsCommand>();
        }

        [TestMethod]
        public void CommandFactory_Root_ReturnsMetadata()
        {
            CommandFactory.GetCommand("").Should().BeOfType<MetadataCommand>();
        }        
    }
}
