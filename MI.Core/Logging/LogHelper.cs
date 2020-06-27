using Castle.Core.Logging;
using MI.Core.Dependency;
using System;

namespace MI.Core.Logging
{
    public static class LogHelper
    {
        static LogHelper()
        {
            Logger = IocManager.Instance.IsRegistered(typeof(ILogger))
                ? IocManager.Instance.Resolve<ILoggerFactory>().Create(typeof(LogHelper))
                : NullLogger.Instance;
        }

        /// <summary>
        ///     日志记录器引用
        /// </summary>
        public static ILogger Logger { get; }

        public static void LogException(Exception ex)
        {
            LogException(Logger, ex);
        }

        public static void LogException(ILogger logger, Exception ex)
        {
            logger.Error(ex.ToString(), ex);
        }
    }
}
