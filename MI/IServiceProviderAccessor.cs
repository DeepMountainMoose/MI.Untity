using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MI
{
    public interface IServiceProviderAccessor
    {
        [CanBeNull]
        IServiceProvider ServiceProvider { get; set; }
    }

    internal class ServiceProviderAccessor:IServiceProviderAccessor
    {
        private readonly AsyncLocal<IServiceProvider> _provider = new AsyncLocal<IServiceProvider>();

        public virtual IServiceProvider ServiceProvider
        {
            get
            {
                var provider = _provider.Value;
                if (provider == null)
                    return null;

                return provider.GetRequiredService(typeof(IServiceProvider)) == null ? (_provider.Value = null) : provider;
            }
            set => _provider.Value = value as ServiceProviderWrapper ?? new ServiceProviderWrapper(value);
        }
    }

    internal class ServiceProviderWrapper:IServiceProvider, ISupportRequiredService
    {
        private IServiceProvider _provider;

        public ServiceProviderWrapper(IServiceProvider provider) => _provider = provider;

        public object GetRequiredService(Type serviceType)
        {
            try
            {
                return _provider?.GetRequiredService(serviceType);
            }
            catch (ObjectDisposedException)
            {
                return _provider = null;
            }
        }

        public object GetService(Type serviceType)
        {
            try
            {
                return _provider?.GetService(serviceType);
            }
            catch (ObjectDisposedException)
            {
                return _provider = null;
            }
        }
    }
}
