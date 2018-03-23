using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2.Tests.Helpers;

namespace Tests
{
	[TestClass]
    public class Startup
    {
		[AssemblyInitialize]
		public static void Initialize(TestContext testContext)
		{
			// This is needed because testhost.exe uses its own location for the config file
			// There appears to be no way to set it in .NET Core.  See:
			// https://stackoverflow.com/questions/47752271/app-config-not-beeing-loaded-in-net-core-mstests-project
			// https://github.com/Microsoft/testfx/issues/348
			// https://github.com/dotnet/corefx/issues/26626
			string asmPath = Assembly.GetExecutingAssembly().Location;
			SustainsysSaml2Section.Configuration = ConfigurationManager.OpenExeConfiguration(asmPath);
			StubServer.Start(testContext);
		}

		[AssemblyCleanup]
		public static void Shutdown()
		{
			StubServer.Stop();
		}

	}
}
