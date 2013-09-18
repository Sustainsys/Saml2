using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Configuration
{
    public class IdentityProviderCollection : ConfigurationElementCollection, IEnumerable<IdentityProviderElement>
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new IdentityProviderElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((IdentityProviderElement)element).Name;
        }

        public new IEnumerator<IdentityProviderElement> GetEnumerator()
        {
            return base.GetEnumerator().AsGeneric<IdentityProviderElement>();
        }
    }
}
