using Kentor.AuthServices.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices
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

            var options = new SPOptions
            {
                EntityId = new EntityId("https://github.com/KentorIT/authservices"),
                MetadataCacheDuration = new TimeSpan(0, 0, 42),
                Organization = org,
                DiscoveryServiceUrl = new Uri("https://ds.example.com"),
                ReturnUri = new Uri("https://localhost/returnUri"),
            };

            options.Contacts.Add(supportContact);
            options.Contacts.Add(new ContactPerson(ContactType.Technical)); // Deliberately void of info.

            return options;
        }

        internal static Options CreateOptions()
        {
            var options = new Options(CreateSPOptions());

            KentorAuthServicesSection.Current.IdentityProviders.RegisterIdentityProviders(options);
            KentorAuthServicesSection.Current.Federations.RegisterFederations(options);

            return options;
        }
    }
}
