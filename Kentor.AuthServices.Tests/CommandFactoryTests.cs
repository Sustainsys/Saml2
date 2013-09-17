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
            CommandFactory.GetCommand("foo").Should().BeOfType<NotFoundCommand>();
        }
    }
}
