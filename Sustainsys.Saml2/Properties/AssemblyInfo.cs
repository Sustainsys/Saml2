using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("6ca3099a-e209-4c65-aaf4-1a73d8d9bd48")]

[assembly: CLSCompliant(true)]

[assembly: InternalsVisibleTo("Tests.NET47")]
[assembly: InternalsVisibleTo("Tests.NetCore2")]
[assembly: InternalsVisibleTo("TestHelpers")]
[assembly: InternalsVisibleTo("Owin.Tests")]
[assembly: InternalsVisibleTo("HttpModule.Tests")]
[assembly: InternalsVisibleTo("AspNetCore2.Tests.Net47")]
[assembly: InternalsVisibleTo("AspNetCore2.Tests.NetCore2")]
#warning Remove this
[assembly: InternalsVisibleTo("AspNetCore2.Tests")]

// Required for NSubstitute to be able to generate stub for internal interface.
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
