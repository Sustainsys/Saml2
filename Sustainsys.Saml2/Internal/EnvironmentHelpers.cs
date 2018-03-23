using System;
using System.Runtime.InteropServices;

namespace Sustainsys.Saml2.Internal
{
	public class EnvironmentHelpers
    {
		public static bool IsNetCore => RuntimeInformation.FrameworkDescription.
			StartsWith(".NET Core", StringComparison.OrdinalIgnoreCase);
	}
}
