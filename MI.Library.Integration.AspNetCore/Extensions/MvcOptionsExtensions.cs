using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Library.Integration.AspNetCore.Extensions
{
    public static class MvcOptionsExtensions
    {
        public static MvcOptions InitLibraryOption(this MvcOptions options)
        {
            //options.Filters.Add<AppInsightsRequestFilter>(5);
            //options.Filters.Add<ExceptionFilter>();
            //options.Filters.Add<RequestContextFilter>(0);
            //options.Filters.Add<ResultFilter>();
            return options;
        }
    }
}
