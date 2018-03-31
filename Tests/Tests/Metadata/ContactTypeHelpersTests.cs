using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sustainsys.Saml2.Metadata;
using System;

namespace Sustainsys.Saml2.Tests.Metadata
{
	[TestClass]
    public class ContactTypeHelpersTests
    {
		[TestMethod]
		public void ContactTypeHelpersTests_ParseContactTypeTechnical()
		{
			ContactTypeHelpers.ParseContactType("technical")
				.Should().Be(ContactType.Technical);
		}

		[TestMethod]
		public void ContactTypeHelpersTests_ParseContactTypeSupport()
		{
			ContactTypeHelpers.ParseContactType("support")
				.Should().Be(ContactType.Support);
		}

		[TestMethod]
		public void ContactTypeHelpersTests_ParseContactTypeAdministrative()
		{
			ContactTypeHelpers.ParseContactType("administrative")
				.Should().Be(ContactType.Administrative);
		}

		[TestMethod]
		public void ContactTypeHelpersTests_ParseContactTypeBilling()
		{
			ContactTypeHelpers.ParseContactType("billing")
				.Should().Be(ContactType.Billing);
		}

		[TestMethod]
		public void ContactTypeHelpersTests_ParseContactTypeOther()
		{
			ContactTypeHelpers.ParseContactType("other")
				.Should().Be(ContactType.Other);
		}

		[TestMethod]
		public void ContactTypeHelpersTests_ParseContactTypeNull()
		{
			Action a = () => ContactTypeHelpers.ParseContactType(null);
			a.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("contactType");
		}

		[TestMethod]
		public void ContactTypeHelpersTests_ParseContactTypeWRONG()
		{
			Action a = () => ContactTypeHelpers.ParseContactType("WRONG");
			a.Should().Throw<FormatException>();
		}
	}
}
