using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Tokens;
using Sustainsys.Saml2.WebSso;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Sustainsys.Saml2.Metadata.Services;

namespace Sustainsys.Saml2.TestHelpers
{
    public class StubFactory
    {
        public static Saml2Urls CreateSaml2Urls()
        {
            return new Saml2Urls(new Uri("http://localhost"), "/Saml2");
        }

        public static Saml2Urls CreateSaml2UrlsPublicOrigin(Uri publicOrigin)
        {
            return new Saml2Urls(publicOrigin, "/Saml2");
        }

        public static SPOptions CreateSPOptions()
        {
            var org = new Organization();

            org.Names.Add(new LocalizedName("Sustainsys.Saml2", "en"));
            org.DisplayNames.Add(new LocalizedName("Sustainsys Saml2", "en"));
            org.Urls.Add(new LocalizedUri(
                new Uri("http://github.com/Sustainsys/Saml2"),
				"en"));

            var options = new SPOptions
            {
                EntityId = new EntityId("https://github.com/Sustainsys/Saml2"),
                MetadataCacheDuration = new XsdDuration(seconds: 42),
                MetadataValidDuration = TimeSpan.FromDays(24),
                WantAssertionsSigned = true,
                Organization = org,
                DiscoveryServiceUrl = new Uri("https://ds.example.com"),
                ReturnUrl = new Uri("https://localhost/returnUrl")
            };

            AddContacts(options);
            AddAttributeConsumingServices(options);

            return options;
        }

        public static SPOptions CreateSPOptions(Uri publicOrigin)
        {
            var options = CreateSPOptions();
            options.PublicOrigin = publicOrigin;

            return options;
        }

        private static void AddAttributeConsumingServices(SPOptions options)
        {
            var a1 = new RequestedAttribute("urn:attributeName")
            {
                FriendlyName = "friendlyName",
                NameFormat = RequestedAttribute.AttributeNameFormatUri,
                AttributeValueXsiType = ClaimValueTypes.String,
                IsRequired = true
            };
            a1.Values.Add("value1");
            a1.Values.Add("value2");

			var a2 = new RequestedAttribute("someName")
			{
				IsRequired = false
			};

            var acs = new AttributeConsumingService
            {
                IsDefault = true
            };
			acs.ServiceNames.Add(new LocalizedName(
				"attributeServiceName", "en"));
            acs.RequestedAttributes.Add(a1);
            acs.RequestedAttributes.Add(a2);

            options.AttributeConsumingServices.Add(acs);
        }

        private static void AddContacts(SPOptions options)
        {
            var supportContact = new ContactPerson(ContactType.Support)
            {
                Company = "Sustainsys",
                GivenName = "Anders",
                Surname = "Abel",
            };

            supportContact.TelephoneNumbers.Add("+46 8 587 650 00");
            supportContact.TelephoneNumbers.Add("+46 708 96 50 63");
            supportContact.EmailAddresses.Add("info@Sustainsys.se");
            supportContact.EmailAddresses.Add("anders.abel@Sustainsys.se");

            options.Contacts.Add(supportContact);
            options.Contacts.Add(new ContactPerson(ContactType.Technical)); // Deliberately void of info.
        }

        public static IOptions CreateOptions(Func<SPOptions, IOptions> factory)
        {
            var options = factory(CreateSPOptions());

            SustainsysSaml2Section.Current.IdentityProviders.RegisterIdentityProviders(options);

            return options;
        }

        public static Options CreateOptions()
        {
            return (Options)CreateOptions(sp => new Options(sp));
        }

        private static IOptions CreateOptionsPublicOrigin(Func<SPOptions, IOptions> factory, Uri publicOrigin)
        {
            var options = factory(CreateSPOptions(publicOrigin));

            SustainsysSaml2Section.Current.IdentityProviders.RegisterIdentityProviders(options);
            SustainsysSaml2Section.Current.Federations.RegisterFederations(options);

            return options;
        }
        public static Options CreateOptionsPublicOrigin(Uri publicOrigin)
        {
            return (Options)CreateOptionsPublicOrigin(sp => new Options(sp), publicOrigin);
        }
    }
}
