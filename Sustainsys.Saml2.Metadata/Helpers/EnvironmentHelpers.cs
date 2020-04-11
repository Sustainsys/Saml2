using System;
using System.Runtime.InteropServices;

namespace Sustainsys.Saml2.Metadata.Helpers
{
    internal class EnvironmentHelpers
    {
        public static bool IsNetCore =>
#if NETSTANDARD2_0
            RuntimeInformation.FrameworkDescription.
            StartsWith(".NET Core", StringComparison.OrdinalIgnoreCase);

#else
        // If not netstandard, then it is one of the .NET Framework targets. And
        // obviously not running on core.
        false;
#endif
    }
}