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
                    EntityId = new EntityId(KentorAuthServicesSection.Current.EntityId)
                };

                var spsso = new ServiceProviderSingleSignOnDescriptor();
                spsso.AssertionConsumerServices.Add(0, new IndexedProtocolEndpoint()
                {
                });

                return ed;
            }
        }
    }
}
