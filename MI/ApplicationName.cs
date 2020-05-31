using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MI
{
    public class ApplicationName
    {
        private static string AppId { get; set; }

        private readonly IEntryAssemblyResolver _resolver;
        private readonly IHostingEnvironment _environment;

        public ApplicationName(IEntryAssemblyResolver resolver) => _resolver = resolver;
        public ApplicationName(IEntryAssemblyResolver resolver, IHostingEnvironment environment) : this(resolver) => _environment = environment;

        public virtual string GetName() =>
            AppId ??
            Environment.GetEnvironmentVariable("_AppId_") ??
            _environment?.ApplicationName ??
            _resolver.GetEntryAssembly()?.GetName().Name ??
            AppDomain.CurrentDomain.FriendlyName;


        static ApplicationName()
        {
            try
            {
#if NETFRAMEWORK
                if (HostingEnvironment.IsHosted)
                    AppId = File.ReadAllText("App_Data/appid.txt");
                else
#endif
                AppId = File.ReadAllText("appid.txt");
            }
            catch
            {
                // ignored
            }
        }
    }

    /// <summary>
    /// Extension methods for <see cref="IHostingEnvironment"/>.
    /// </summary>
    public static class HostingEnvironmentExtensions
    {
        public static bool IsTuhuTest(this IHostingEnvironment hostingEnvironment)
        {
            if (hostingEnvironment == null)
            {
                throw new ArgumentNullException(nameof(hostingEnvironment));
            }

            return hostingEnvironment.IsEnvironment("MITest");
        }
    }
}
