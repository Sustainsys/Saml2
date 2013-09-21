using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Kentor.AuthServices
{
    interface ICommand
    {
        CommandResult Run(NameValueCollection formData = null);
    }
}
