using System.Reflection;
using System.Runtime.InteropServices;

[assembly: Guid("535b6be7-847b-45ab-bdaa-68e1e52be508")]
[assembly: System.CLSCompliant(false)]

#if NETCOREAPP
[assembly: AssemblyMetadata("ProjectUrl", "https://github.com/DKorablin/Plugin.EventLog")]
#else

[assembly: AssemblyDescription("Simple plugin to monitor EventLog events")]
[assembly: AssemblyCopyright("Copyright © Danila Korablin 2015-2017")]
#endif