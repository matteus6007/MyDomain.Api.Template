using System.Diagnostics;
using System.Reflection;

using Microsoft.Extensions.Options;

namespace MyDomain.Api.Options;

public class AssemblyOptionsProvider : IConfigureOptions<AssemblyOptions>
{
    public void Configure(AssemblyOptions options)
    {
        var assembly = Assembly.GetEntryAssembly();

        if (assembly == null)
        {
            return;
        }

        var version = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;

        options.Version = version ?? "1.0.0.0";
    }
}