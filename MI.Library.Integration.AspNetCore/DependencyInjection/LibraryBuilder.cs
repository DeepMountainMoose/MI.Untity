using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Library.Integration.AspNetCore.DependencyInjection
{
    public class LibraryBuilder : ILibraryBuilder
    {
        public IServiceCollection Services { get; set; }

        public LibraryBuilder(IServiceCollection services)
        {
            Services = services ?? throw new Exception("services is null");
        }
    }
}
