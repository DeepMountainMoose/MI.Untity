using MI.Library.EventArgs;
using MI.Library.Integration.Common.Utils;
using MI.Library.Interface;
using MI.Library.Interface.Common;
using MI.Library.Interface.Enum;
using System;

namespace MI.Library.Integration.Common.Service
{
    public sealed class DefaultEnvironmentProvider : IEnvironmentProvider
    {
        private EnvironmentType _currentEnvironment;

#if !NET462
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;

        public DefaultEnvironmentProvider(Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _currentEnvironment = GetEnvironment();
            DefaultEnvironmentProvider_OnEnvironmentChanged(this, new EnvironmentChangedEventArgs(EnvironmentType.Default, _currentEnvironment));
            OnEnvironmentChanged += DefaultEnvironmentProvider_OnEnvironmentChanged;
        }

        private EnvironmentType GetEnvironment()
        {
            var environmentName = _hostingEnvironment.EnvironmentName;

            if (string.IsNullOrEmpty(environmentName))
                return EnvironmentType.Default;
            return GetEnvironmentType(environmentName.ToLowerInvariant());
        }
#else
        public DefaultEnvironmentProvider()
        {
            _currentEnvironment = GetEnvironment();
            DefaultEnvironmentProvider_OnEnvironmentChanged(this, new EnvironmentChangedEventArgs(EnvironmentType.Default, _currentEnvironment));
            OnEnvironmentChanged += DefaultEnvironmentProvider_OnEnvironmentChanged;
        }


        private EnvironmentType GetEnvironment()
        {
            var environmentSetting = System.Configuration.ConfigurationManager.AppSettings["Environment"];
            if (environmentSetting == null)
                return EnvironmentType.Default;
            else
            {
                return GetEnvironmentType(environmentSetting.ToLowerInvariant());
            }
        }
#endif

        private EnvironmentType GetEnvironmentType(string environmentStr)
        {
            return EnvironmentUtils.GetEnvType(environmentStr);
        }

        public EnvironmentType GetCurrentEnvironment()
        {
            return _currentEnvironment;
        }

        public event EventHandler<EnvironmentChangedEventArgs> OnEnvironmentChanged;
        public void SetEnvironment(EnvironmentType environmentType)
        {
            var oldEnvironment = _currentEnvironment;
            _currentEnvironment = environmentType;
            OnEnvironmentChanged?.Invoke(this, new EnvironmentChangedEventArgs(oldEnvironment, environmentType));
        }


        private void DefaultEnvironmentProvider_OnEnvironmentChanged(object sender, EnvironmentChangedEventArgs e)
        {
            switch (e.NewEnvironment)
            {
                case EnvironmentType.Product:
                    StartupConfig.CacheEnvironmentPrefix = "p:";
                    break;
                case EnvironmentType.Demo:
                    StartupConfig.CacheEnvironmentPrefix = "t:";
                    break;
                default:
                    StartupConfig.CacheEnvironmentPrefix = "d:";
                    break;
            }
        }
    }
}
