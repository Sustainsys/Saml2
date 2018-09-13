using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sustainsys.Saml2.Configuration;
using FluentAssertions;
using Sustainsys.Saml2.Metadata;
using System.Globalization;
using System.Linq;
using Sustainsys.Saml2.TestHelpers;

namespace Sustainsys.Saml2.Tests.Configuration
{
#if FALSE
    [TestClass]
    public class SustainsysSaml2SectionTests
    {
        Uri organizationSectionUrl;
        string organizationSectionName, organizationSectionDisplayName;

        [TestInitialize]
        public void BackupOrganizationSection()
        {
            organizationSectionUrl = SustainsysSaml2Section.Current.Metadata.Organization.Url;
            organizationSectionName = SustainsysSaml2Section.Current.Metadata.Organization.Name;
            organizationSectionDisplayName = SustainsysSaml2Section.Current.Metadata.Organization.DisplayName;
        }

        [TestCleanup]
        public void RestoreOrganizationSection()
        {
            if(!SustainsysSaml2Section.Current.Metadata.Organization.IsReadOnly())
            {
                SustainsysSaml2Section.Current.Metadata.Organization.Url = organizationSectionUrl;
                SustainsysSaml2Section.Current.Metadata.Organization.Name = organizationSectionName;
                SustainsysSaml2Section.Current.Metadata.Organization.DisplayName = organizationSectionDisplayName;
                SustainsysSaml2Section.Current.Metadata.Organization.AllowConfigEdits(false);
            }
        }

        [TestMethod]
        public void SustainsysSaml2Section_Organization_LoadedFromConfig()
        {
            var subject = SustainsysSaml2Section.Current.Organization;

            Organization expected = new Organization();
            expected.DisplayNames.Add(new LocalizedName("displayName", CultureInfo.GetCultureInfo("sv")));
            expected.Names.Add(new LocalizedName("name", CultureInfo.GetCultureInfo("sv")));
            expected.Urls.Add(new LocalizedUri(new Uri("http://url.example.com"), CultureInfo.GetCultureInfo("sv")));

            subject.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void SustainsysSaml2Section_Organization_HandlesMissing()
        {
            // If the organization element is missing in the config file, it will
            // still be present in the read config (which is stupid) and the strings
            // will be empty and the Url null. So let's pretend that the element
            // is missing from the config file by setting those values.
            SustainsysSaml2Section.Current.Metadata.Organization.AllowConfigEdits(true);
            SustainsysSaml2Section.Current.Metadata.Organization.Url = null;
            SustainsysSaml2Section.Current.Metadata.Organization.Name = "";
            SustainsysSaml2Section.Current.Metadata.Organization.DisplayName = "";

            // Reset the cached organization instance to force a reevaluation.
            SustainsysSaml2Section.Current.organization = null;

            SustainsysSaml2Section.Current.Organization.Should().BeNull();
        }

        [TestMethod]
        public void SustainsysSaml2Section_Contacts_LoadedFromConfig()
        {
            var subject = SustainsysSaml2Section.Current.Contacts;

            var expected = StubFactory.CreateSPOptions().Contacts;

            // The config file only supports one phone number and one e-mail for each
            // contact, so let's remove the other ones that are added by the stub factory.
            expected.First().TelephoneNumbers.Remove(expected.First().TelephoneNumbers.First());
            expected.First().EmailAddresses.Remove(expected.First().EmailAddresses.First());

            var secondTech = new ContactPerson(ContactType.Technical);
            secondTech.EmailAddresses.Add("info@Sustainsys.se");
            expected.Add(secondTech);

            subject.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void SustainsysSaml2Section_Attributes_LoadedFromConfig()
        {
            var expected = new AttributeConsumingService("SP")
                {
                    IsDefault = true
                };

            expected.RequestedAttributes.Add(
                new RequestedAttribute("urn:someName")
                {
                    FriendlyName = "Some Name",
                    NameFormat = RequestedAttribute.AttributeNameFormatUri,
                    IsRequired = true
                });
            expected.RequestedAttributes.Add(
                new RequestedAttribute("Minimal")
                {
                    FriendlyName = null,
                    NameFormat = RequestedAttribute.AttributeNameFormatUnspecified,
                    IsRequired = false
                });

            var subject = SustainsysSaml2Section.Current.AttributeConsumingServices.Single();

            subject.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void SustainsysSaml2Section_Attributes_EmptyIfNotConfigured()
        {
            var subject = new SustainsysSaml2Section();
            subject.AllowChange = true;
            subject.Metadata = new MetadataElement();

            subject.AttributeConsumingServices.Should().BeEmpty();
        }
    }
#endif
}
