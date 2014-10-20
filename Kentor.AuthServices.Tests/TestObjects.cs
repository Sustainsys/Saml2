using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices
{
    class TestObjects
    {
        internal static readonly AuthServicesUrls authServicesUrls =
            new AuthServicesUrls(new Uri("http://localhost"), "/AuthServices");
    }
}
