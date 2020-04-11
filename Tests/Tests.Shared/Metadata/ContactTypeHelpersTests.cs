using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sustainsys.Saml2.Metadata;
using System;
using Sustainsys.Saml2.Metadata.Helpers;

namespace Sustainsys.Saml2.Tests.Metadata
{
	[TestClass]
    public class ContactTypeHelpersTests
    {
		[TestMethod]
		public void ContactTypeHelpersTests_ParseContactTypeTechnical()
		{
			ContactTypeHelpers.Parse("technical")
				.Should().Be(ContactType.Technical);
		}

		[TestMethod]
		public void ContactTypeHelpersTests_ParseContactTypeSupport()
		{
			ContactTypeHelpers.Parse("support")
				.Should().Be(ContactType.Support);
		}

		[TestMethod]
		public void ContactTypeHelpersTests_ParseContactTypeAdministrative()
		{
			ContactTypeHelpers.Parse("administrative")
				.Should().Be(ContactType.Administrative);
		}

		[TestMethod]
		public void ContactTypeHelpersTests_ParseContactTypeBilling()
		{
			ContactTypeHelpers.Parse("billing")
				.Should().Be(ContactType.Billing);
		}

		[TestMethod]
		public void ContactTypeHelpersTests_ParseContactTypeOther()
		{
			ContactTypeHelpers.Parse("other")
				.Should().Be(ContactType.Other);
		}

		[TestMethod]
		public void ContactTypeHelpersTests_ParseContactTypeNull()
		{
			Action a = () => ContactTypeHelpers.Parse(null);
			a.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("contactType");
		}

		[TestMethod]
		public void ContactTypeHelpersTests_ParseContactTypeWRONG()
		{
			Action a = () => ContactTypeHelpers.Parse("WRONG");
			a.Should().Throw<FormatException>();
		}

		[TestMethod]
		public void ContactTypeHelpersTests_ToStringTechnical()
		{
			ContactTypeHelpers.ToString(ContactType.Technical)
				.Should().Be("technical");
		}

		[TestMethod]
		public void ContactTypeHelpersTests_ToStringSupport()
		{
			ContactTypeHelpers.ToString(ContactType.Support)
				.Should().Be("support");
		}

		[TestMethod]
		public void ContactTypeHelpersTests_ToStringAdministrative()
		{
			ContactTypeHelpers.ToString(ContactType.Administrative)
				.Should().Be("administrative");
		}

		[TestMethod]
		public void ContactTypeHelpersTests_ToStringBilling()
		{
			ContactTypeHelpers.ToString(ContactType.Billing)
				.Should().Be("billing");
		}

		[TestMethod]
		public void ContactTypeHelpersTests_ToStringOther()
		{
			ContactTypeHelpers.ToString(ContactType.Other)
				.Should().Be("other");
		}

		[TestMethod]
		public void ContactTypeHelpersTests_ToStringINVALID()
		{
			Action a = () => ContactTypeHelpers.ToString((ContactType)1000);
			a.Should().Throw<InvalidOperationException>();
		}
	}
}
