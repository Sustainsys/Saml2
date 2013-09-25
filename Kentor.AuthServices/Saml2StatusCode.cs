using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices
{
    public enum Saml2StatusCode
    {
        Success,

        Requester,

        Responder,

        VersionMismatch,
    }
}
