using Kentor.AuthServices.Saml2P;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.WebSso
{
    /// <summary>
    /// Extension method for binding an <see cref="ISaml2Message"/> to a Saml2Binding
    /// </summary>
    public static class MessageBinder
    {
        /// <summary>
        /// Binds a message to a binding
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static CommandResult Bind(this Saml2Binding binding, ISaml2Message message)
        {
            if (binding == null)
            {
                throw new ArgumentNullException(nameof(binding));
            }
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            return binding.Bind(message.ToXml(), message.DestinationUrl, message.MessageName);
        }
    }
}
