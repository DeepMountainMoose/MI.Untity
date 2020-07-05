using MI.Library.Interface;
using MI.Library.Interface.Enum;
using System.Collections.Generic;

namespace MI.Library.Service
{
    public sealed class DefaultApplicationUrlProvider : IApplicationUrlProvider
    {
        private IReadOnlyDictionary<int, string> _urlMapper;

        public DefaultApplicationUrlProvider(IEnvironmentProvider environmentProvider)
        {
            var environment = environmentProvider.GetCurrentEnvironment();
            InitUrl(environment);
            environmentProvider.OnEnvironmentChanged += EnvironmentProvider_OnEnvironmentChanged;
        }

        private void EnvironmentProvider_OnEnvironmentChanged(object sender, EventArgs.EnvironmentChangedEventArgs e)
        {
            InitUrl(e.NewEnvironment);
        }

        private void InitUrl(EnvironmentType environment)
        {
            switch (environment)
            {
                case EnvironmentType.Product:
                    ProductionConfig();
                    break;
                case EnvironmentType.Demo:
                    DemoConfig();
                    break;
                default:
                    ConfigDefault();
                    break;
            }
        }

        private void ProductionConfig()
        {
            _urlMapper = new Dictionary<int, string>
            {
                {(int)ApplicationType.Interface,"http://interface.1hai.cn/api/" },
                {(int)ApplicationType.Picture,"http://localhost:7002/" }
            };
        }

        private void DemoConfig()
        {
            _urlMapper = new Dictionary<int, string>
            {
                {(int)ApplicationType.Interface,"http://interface.demo.ehi.com.cn/api/" },
                {(int)ApplicationType.Picture,"http://localhost:7002/" }
            };
        }

        private void ConfigDefault()
        {
            _urlMapper = new Dictionary<int, string>
            {
                {(int)ApplicationType.Interface,"http://interface.dev.ehi.com.cn/api/" },
                {(int)ApplicationType.Picture,"http://localhost:7002/" }
        };
        }

        public string GetUrl(ApplicationType type)
        {
            return _urlMapper[(int)type];
        }
    }
}
