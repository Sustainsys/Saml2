using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Kentor.AuthServices.Configuration;

namespace Kentor.AuthServices
{
    static class SPOptionsExtensions
    {
        public static EntityDescriptor CreateMetadata(this ISPOptions spOptions, AuthServicesUrls urls)
        {
            var ed = new EntityDescriptor()
            {
                EntityId = spOptions.EntityId,
                Organization = spOptions.Organization
            };

            foreach (var contact in spOptions.Contacts)
            {
                ed.Contacts.Add(contact);
            }

            var spsso = new ServiceProviderSingleSignOnDescriptor();

            spsso.ProtocolsSupported.Add(new Uri("urn:oasis:names:tc:SAML:2.0:protocol"));

            spsso.AssertionConsumerServices.Add(0, new IndexedProtocolEndpoint()
            {
                Index = 0,
                IsDefault = true,
                Binding = Saml2Binding.HttpPostUri,
                Location = urls.AssertionConsumerServiceUrl
            });

            ed.RoleDescriptors.Add(spsso);

            return ed;
        }
    }
}
