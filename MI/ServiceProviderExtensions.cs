using MI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static void SetServiceProviderAccessor(this IServiceProvider provider)
        {
            if (provider != null)
                provider.GetRequiredService<IServiceProviderAccessor>().ServiceProvider = provider;
        }
    }
}


namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceProviderExtensions
    {
        public static IServiceScope CreateScopeWithAccessor(this IServiceProvider provider)
        {
            var scope = provider.CreateScope();

            scope.ServiceProvider.SetServiceProviderAccessor();

            return scope;
        }
    }
}
