using JetBrains.Annotations;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Text;

namespace MI
{
    /// <summary>为了解决一些地方实在没办法注入而出此下策，比如log4net、System.Configuration.ConfigurationSection</summary>
    public class ServiceResolver
    {
        private const string Error = "请在Startup.Configure方法中调用app.UseTuhu()或者provider.GetRequiredService<ServiceResolver>()";
        private static IServiceProvider _provider;

        public ServiceResolver(IServiceProvider provider) => _provider = provider;
        //TODO IValidateOptions
        [CanBeNull] //ConditionalWeakTable<>
        public static T GetService<T>() => (_provider ?? throw new Exception(Error)).GetService<T>();

        [NotNull]
        public static T GetRequiredService<T>() => (_provider ?? throw new Exception(Error)).GetRequiredService<T>();

        public static object CreateInstance(Type type, params object[] parameters) =>
            ActivatorUtilities.CreateInstance(_provider ?? throw new Exception(Error), type, parameters);

        public static T CreateInstance<T>(params object[] parameters) =>
            (T)CreateInstance(typeof(T), parameters);
    }
}
