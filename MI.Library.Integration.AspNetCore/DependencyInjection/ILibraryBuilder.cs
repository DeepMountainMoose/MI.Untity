using Microsoft.Extensions.DependencyInjection;

namespace MI.Library.Integration.AspNetCore.DependencyInjection
{
    public interface ILibraryBuilder
    {
        IServiceCollection Services { get; }
    }
}
