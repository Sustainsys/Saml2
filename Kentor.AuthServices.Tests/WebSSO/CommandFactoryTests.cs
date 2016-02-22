using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Kentor.AuthServices.WebSso;

namespace Kentor.AuthServices.Tests.WebSso
{
    [TestClass]
    public class CommandFactoryTests
    {
        [TestMethod]
        public void CommandFactory_GetCommand_Invalid_ReturnsNotFound()
        {
            CommandFactory.GetCommand("Invalid").Should().BeOfType<NotFoundCommand>();
        }

        [TestMethod]
        public void CommandFactory_GetCommand_SignIn_ReturnsSignIn()
        {
            CommandFactory.GetCommand("SignIn").Should().BeOfType<SignInCommand>();
        }

        [TestMethod]
        public void CommandFactory_GetCommand_IsCaseInsensitive()
        {
          CommandFactory.GetCommand("signin").Should().BeOfType<SignInCommand>();
        }

        [TestMethod]
        public void CommandFactory_GetCommand_Acs_ReturnsAcs()
        {
            CommandFactory.GetCommand("Acs").Should().BeOfType<AcsCommand>();
        }

        [TestMethod]
        public void CommandFactory_GetCommand_Root_ReturnsMetadata()
        {
            CommandFactory.GetCommand("").Should().BeOfType<MetadataCommand>();
        }

        [TestMethod]
        public void CommnandFactory_GetCommand_StripsOffLeadingSlash()
        {
            CommandFactory.GetCommand("/Acs").Should().BeOfType<AcsCommand>();
        }

        [TestMethod]
        public void CommandFactory_GetCommand_SignOut_ReturnsSignOut()
        {
            CommandFactory.GetCommand("Logout").Should().BeOfType<LogoutCommand>();
        }

        [TestMethod]
        public void CommandFactory_GetCommand_NullCheckCommandName()
        {
            Action a = () => CommandFactory.GetCommand(null);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("commandName");
        }
    }
}
