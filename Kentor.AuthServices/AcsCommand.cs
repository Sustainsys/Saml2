using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Kentor.AuthServices
{
    class AcsCommand : ICommand
    {
        public CommandResult Run(NameValueCollection formData)
        {
            if (formData.AllKeys.Contains("SAMLResponse"))
            {
                return new CommandResult();
            }
            return null;
        }
    }
}
