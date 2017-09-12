using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kentor.AuthServices.Configuration;
using FluentAssertions;
using System.IdentityModel.Metadata;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using Kentor.AuthServices.Metadata;

namespace Kentor.AuthServices.Tests.Configuration
{
    [TestClass]
    public class KentorAuthServicesSectionTests
    {
        Uri organizationSectionUrl;
        string organizationSectionName, organizationSectionDisplayName;

        [TestInitialize]
        public void BackupOrganizationSection()
        {
            organizationSectionUrl = KentorAuthServicesSection.Current.Metadata.Organization.Url;
            organizationSectionName = KentorAuthServicesSection.Current.Metadata.Organization.Name;
            organizationSectionDisplayName = KentorAuthServicesSection.Current.Metadata.Organization.DisplayName;
        }

        [TestCleanup]
        public void RestoreOrganizationSection()
        {
            if(!KentorAuthServicesSection.Current.Metadata.Organization.IsReadOnly())
            {
                KentorAuthServicesSection.Current.Metadata.Organization.Url = organizationSectionUrl;
                KentorAuthServicesSection.Current.Metadata.Organization.Name = organizationSectionName;
                KentorAuthServicesSection.Current.Metadata.Organization.DisplayName = organizationSectionDisplayName;
                KentorAuthServicesSection.Current.Metadata.Organization.AllowConfigEdits(false);
            }
        }

        [TestMethod]
        public void KentorAuthServicesSection_Organization_LoadedFromConfig()
        {
            var subject = KentorAuthServicesSection.Current.Organization;

            Organization expected = new Organization();
            expected.DisplayNames.Add(new LocalizedName("displayName", CultureInfo.GetCultureInfo("sv")));
            expected.Names.Add(new LocalizedName("name", CultureInfo.GetCultureInfo("sv")));
            expected.Urls.Add(new LocalizedUri(new Uri("http://url.example.com"), CultureInfo.GetCultureInfo("sv")));

            subject.ShouldBeEquivalentTo(expected);
        }

        [TestMethod]
        public void KentorAuthServicesSection_Organization_HandlesMissing()
        {
            // If the organization element is missing in the config file, it will
            // still be present in the read config (which is stupid) and the strings
            // will be empty and the Url null. So let's pretend that the element
            // is missing from the config file by setting those values.
            KentorAuthServicesSection.Current.Metadata.Organization.AllowConfigEdits(true);
            KentorAuthServicesSection.Current.Metadata.Organization.Url = null;
            KentorAuthServicesSection.Current.Metadata.Organization.Name = "";
            KentorAuthServicesSection.Current.Metadata.Organization.DisplayName = "";

            // Reset the cached organization instance to force a reevaluation.
            KentorAuthServicesSection.Current.organization = null;

            KentorAuthServicesSection.Current.Organization.Should().BeNull();
        }

        [TestMethod]
        public void KentorAuthServicesSection_Contacts_LoadedFromConfig()
        {
            var subject = KentorAuthServicesSection.Current.Contacts;

            var expected = StubFactory.CreateSPOptions().Contacts;

            // The config file only supports one phone number and one e-mail for each
            // contact, so let's remove the other ones that are added by the stub factory.
            expected.First().TelephoneNumbers.Remove(expected.First().TelephoneNumbers.First());
            expected.First().EmailAddresses.Remove(expected.First().EmailAddresses.First());

            var secondTech = new ContactPerson(ContactType.Technical);
            secondTech.EmailAddresses.Add("info@kentor.se");
            expected.Add(secondTech);

            subject.ShouldBeEquivalentTo(expected);
        }

        [TestMethod]
        public void KentorAuthServicesSection_Attributes_LoadedFromConfig()
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

            var subject = KentorAuthServicesSection.Current.AttributeConsumingServices.Single();

            subject.ShouldBeEquivalentTo(expected);
        }

        [TestMethod]
        public void KentorAuthServicesSection_Attributes_EmptyIfNotConfigured()
        {
            var subject = new KentorAuthServicesSection();
            subject.AllowChange = true;
            subject.Metadata = new MetadataElement();

            subject.AttributeConsumingServices.Should().BeEmpty();
        }
    }
}
