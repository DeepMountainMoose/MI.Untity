using MI.AspNetCore.AspNetCore.Mvc.Conventions;
using Microsoft.AspNetCore.Mvc;
using IServiceCollection = Microsoft.Extensions.DependencyInjection.IServiceCollection;

namespace MI.AspNetCore
{
    public static class MvcOptionsExtensions
    {
        public static void AddFeI(this MvcOptions options, IServiceCollection services)
        {
            AddConventions(options, services);
        }

        private static void AddConventions(MvcOptions options, IServiceCollection services)
        {
            options.Conventions.Add(new AppServiceConvention(services));
        }
    }
}
