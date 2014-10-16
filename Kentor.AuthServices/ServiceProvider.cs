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
    static class ServiceProvider
    {
        public static EntityDescriptor Metadata
        {
            get
            {
                var ed = new EntityDescriptor()
                {
                    EntityId = KentorAuthServicesSection.Current.EntityId
                };

                var spsso = new ServiceProviderSingleSignOnDescriptor();

                spsso.ProtocolsSupported.Add(new Uri("urn:oasis:names:tc:SAML:2.0:protocol"));

                spsso.AssertionConsumerServices.Add(0, new IndexedProtocolEndpoint()
                {
                    Index = 0,
                    IsDefault = true,
                    Binding = Saml2Binding.HttpPostUri,
                    Location = KentorAuthServicesSection.Current.AssertionConsumerServiceUrl
                });

                ed.RoleDescriptors.Add(spsso);

                return ed;
            }
        }
    }
}
