using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Common
{
    /// <summary>
    ///     枚举扩展
    /// </summary>
    public static class EnumExtensions
    {
        public static TeamType ToTeamType(this Platform platform)
        {
            switch (platform)
            {
                case Platform.MI:
                case Platform.TestApi:
                    return TeamType.MI;
                default:
                    return TeamType.MI;
            }
        }
    }
}
