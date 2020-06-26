namespace MI.Library.Enum
{
    /// <summary>
    ///     优先使用的自动附加平台
    /// </summary>
    public enum PlatformPriority
    {
        /// <summary>
        ///     优先使用<see cref="StartupConfig.CurrentPlatform"/>
        /// </summary>
        CurrentPlatform,
        /// <summary>
        ///     优先使用<see cref="RequestContext"/>里面通过SetData的Platform
        /// </summary>
        SetDataPlatform
    }
}
