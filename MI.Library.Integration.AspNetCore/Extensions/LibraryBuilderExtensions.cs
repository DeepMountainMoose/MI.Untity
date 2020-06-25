using MI.AspNetCore;
using MI.Core;
using MI.Library.Integration.AspNetCore.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using FeIModule = MI.Core.Modules.Module;

namespace MI.Library.Integration.AspNetCore.Extensions
{
    public static class LibraryBuilderExtensions
    {
        /// <summary>
        ///     增加FeI
        /// </summary>
        /// <typeparam name="TStartupModule"></typeparam>
        /// <param name="builder"></param>
        /// <param name="optionsAction"></param>
        /// <returns></returns>
        public static IServiceProvider AddEhiFeI<TStartupModule>(this ILibraryBuilder builder,
            Action<BootstrapperOptions> optionsAction = null)
            where TStartupModule : FeIModule
        {
            return builder.Services.AddFeI<TStartupModule>(optionsAction);
        }

        /// <summary>
        ///     增加Mvc
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="optionsAction"></param>
        /// <returns></returns>
        public static ILibraryBuilder AddEhiMvc(this ILibraryBuilder builder, Action<MvcOptions> optionsAction = null)
        {
            if (optionsAction == null) optionsAction = options => { options.InitLibraryOption(); };

            builder.Services.AddMvc(a =>
            {
                //if (StartupConfig.StartupMode.HasAnyFlags(StartupModeType.Ui))
                //{
                //    var jsonFormatter =
                //        (JsonOutputFormatter)a.OutputFormatters.FirstOrDefault(f => f is JsonOutputFormatter);
                //    if (jsonFormatter != null)
                //        jsonFormatter.PublicSerializerSettings.NullValueHandling = NullValueHandling.Include;
                //}

                optionsAction?.Invoke(a);
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            return builder;
        }
    }
}
