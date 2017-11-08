using Kentor.AuthServices.Configuration;
using Kentor.AuthServices.Metadata;
using Kentor.AuthServices.WebSso;
using Microsoft.IdentityModel.Tokens.Saml2;
using System;
using System.Globalization;
#if NET47
using System.IdentityModel.Metadata;
#endif
using System.Security.Claims;

namespace Kentor.AuthServices.TestHelpers
{
    public class StubFactory
    {
        public static AuthServicesUrls CreateAuthServicesUrls()
        {
            return new AuthServicesUrls(new Uri("http://localhost"), "/AuthServices");
        }

        public static AuthServicesUrls CreateAuthServicesUrlsPublicOrigin(Uri publicOrigin)
        {
            return new AuthServicesUrls(publicOrigin, "/AuthServices");
        }

        public static SPOptions CreateSPOptions()
        {
#if NET47
            var org = new Organization();

            org.Names.Add(new LocalizedName("Kentor.AuthServices", CultureInfo.InvariantCulture));
            org.DisplayNames.Add(new LocalizedName("Kentor AuthServices", CultureInfo.InvariantCulture));
            org.Urls.Add(new LocalizedUri(
                new Uri("http://github.com/KentorIT/authservices"),
                CultureInfo.InvariantCulture));
#endif
            var options = new SPOptions
            {
                EntityId = new Saml2NameIdentifier("https://github.com/KentorIT/authservices"),
#if NET47
                MetadataCacheDuration = new TimeSpan(0, 0, 42),
                MetadataValidDuration = TimeSpan.FromDays(24),
                WantAssertionsSigned = true,
                Organization = org,
#endif
                DiscoveryServiceUrl = new Uri("https://ds.example.com"),
                ReturnUrl = new Uri("https://localhost/returnUrl")
            };

#if NET47
            AddContacts(options);
            AddAttributeConsumingServices(options);
#endif
            return options;
        }

#if NET47
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
#endif

        public static IOptions CreateOptions(Func<SPOptions, IOptions> factory)
        {
            var options = factory(CreateSPOptions());

            var idp = new IdentityProvider(new EntityId("https://idp.example.com"), options.SPOptions)
            {
                SingleSignOnServiceUrl = new Uri("https://idp.example.com/idp"),
                SingleLogoutServiceUrl = new Uri("https://idp.example.com/logout"),
                AllowUnsolicitedAuthnResponse = true,
                Binding = Saml2BindingType.HttpRedirect,
            };
            idp.SigningKeys.AddConfiguredKey(SignedXmlHelper.TestCert);
            idp.ArtifactResolutionServiceUrls.Add(4660, new Uri("http://localhost:13428/ars"));
            options.IdentityProviders.Add(idp);

            idp = new IdentityProvider(new EntityId("https://idp2.example.com"), options.SPOptions)
            {
                SingleSignOnServiceUrl = new Uri("https://idp2.example.com/idp"),
                AllowUnsolicitedAuthnResponse = false,
                Binding = Saml2BindingType.HttpRedirect,
                WantAuthnRequestsSigned = true
            };
            idp.SigningKeys.AddConfiguredKey(SignedXmlHelper.TestCert);
            options.IdentityProviders.Add(idp);

#if NET47
            idp = new IdentityProvider(new EntityId("localhost:13428/idpMetadata"), options.SPOptions)
            {
                AllowUnsolicitedAuthnResponse = true,
                LoadMetadata = true
            };
            options.IdentityProviders.Add(idp);
#endif

            idp = new IdentityProvider(new EntityId("https://idp4.example.com"), options.SPOptions)
            {
                SingleSignOnServiceUrl = new Uri("https://idp4.example.com/idp"),
                AllowUnsolicitedAuthnResponse = false,
                Binding = Saml2BindingType.HttpPost,
                OutboundSigningAlgorithm = Options.RsaSha256Uri,
                WantAuthnRequestsSigned = true
            };

            idp.SigningKeys.AddConfiguredKey(SignedXmlHelper.TestCert);
            options.IdentityProviders.Add(idp);

            return options;
        }

        public static Options CreateOptions()
        {
            return (Options)CreateOptions(sp => new Options(sp));
        }
    }
}
