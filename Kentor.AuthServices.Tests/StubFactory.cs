using Kentor.AuthServices.Configuration;
using Kentor.AuthServices.Metadata;
using Kentor.AuthServices.Owin;
using Kentor.AuthServices.WebSso;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Tests
{
    class StubFactory
    {
        internal static AuthServicesUrls CreateAuthServicesUrls()
        {
            return new AuthServicesUrls(new Uri("http://localhost"), "/AuthServices");
        }

        internal static SPOptions CreateSPOptions()
        {
            var org = new Organization();

            org.Names.Add(new LocalizedName("Kentor.AuthServices", CultureInfo.InvariantCulture));
            org.DisplayNames.Add(new LocalizedName("Kentor AuthServices", CultureInfo.InvariantCulture));
            org.Urls.Add(new LocalizedUri(
                new Uri("http://github.com/KentorIT/authservices"),
                CultureInfo.InvariantCulture));

            var options = new SPOptions
            {
                EntityId = new EntityId("https://github.com/KentorIT/authservices"),
                MetadataCacheDuration = new TimeSpan(0, 0, 42),
                Organization = org,
                DiscoveryServiceUrl = new Uri("https://ds.example.com"),
                ReturnUrl = new Uri("https://localhost/returnUrl"),
            };

            options.SystemIdentityModelIdentityConfiguration.ClaimsAuthenticationManager
                = new ClaimsAuthenticationManagerStub();

            AddContacts(options);
            AddAttributeConsumingServices(options);

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

            var a2 = new RequestedAttribute("someName");

            var acs = new AttributeConsumingService("attributeServiceName")
            {
                IsDefault = true
            };
            acs.RequestedAttributes.Add(a1);
            acs.RequestedAttributes.Add(a2);

            options.AttributeConsumingServices.Add(acs);
        }

        private static void AddContacts(SPOptions options)
        {
            var supportContact = new ContactPerson(ContactType.Support)
            {
                Company = "Kentor",
                GivenName = "Anders",
                Surname = "Abel",
            };

            supportContact.TelephoneNumbers.Add("+46 8 587 650 00");
            supportContact.TelephoneNumbers.Add("+46 708 96 50 63");
            supportContact.EmailAddresses.Add("info@kentor.se");
            supportContact.EmailAddresses.Add("anders.abel@kentor.se");

            options.Contacts.Add(supportContact);
            options.Contacts.Add(new ContactPerson(ContactType.Technical)); // Deliberately void of info.
        }

        private static IOptions CreateOptions(Func<ISPOptions, IOptions> factory)
        {
            var options = factory(CreateSPOptions());

            KentorAuthServicesSection.Current.IdentityProviders.RegisterIdentityProviders(options);
            KentorAuthServicesSection.Current.Federations.RegisterFederations(options);

            return options;
        }

        internal static Options CreateOptions()
        {
            return (Options)CreateOptions(sp => new Options(sp));
        }

        internal static KentorAuthServicesAuthenticationOptions CreateOwinOptions()
        {
            return (KentorAuthServicesAuthenticationOptions)CreateOptions(
                sp => new KentorAuthServicesAuthenticationOptions(false)
                {
                    SPOptions = sp,
                    SignInAsAuthenticationType = "AuthType"
                });
        }
    }
}
