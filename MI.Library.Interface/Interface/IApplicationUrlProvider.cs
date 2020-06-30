using MI.Library.Interface.Enum;

namespace MI.Library.Interface
{
    public interface IApplicationUrlProvider
    {
        /// <summary>
        ///     获取应用程序Url
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        string GetUrl(ApplicationType type);
    }
}
