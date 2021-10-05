using System;
using System.Runtime.InteropServices;

namespace Sustainsys.Saml2.Metadata.Helpers
{
    internal class EnvironmentHelpers
    {
        public static bool IsNetCore =>
#if NETSTANDARD2_0
        // To support .NET 5.0 and up, specifically check if this is not .NET Framework.
        // Since .NET 5.0, the description no longer starts with ".NET Core", but simply ".NET". 
        !RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework", StringComparison.OrdinalIgnoreCase);
#else
        // If not netstandard, then it is one of the .NET Framework targets. And 
        // obviously not running on core.
        false;
#endif
    }
}
