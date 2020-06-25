using System;
using System.Collections.Generic;
using System.Text;

namespace MI.AspNetCore.AspNetCore
{
    public class ApplicationBuilderOptions
    {
        /// <summary>
        /// 默认值: true.
        /// </summary>
        public bool UseCastleLoggerFactory { get; set; }

        public ApplicationBuilderOptions()
        {
            UseCastleLoggerFactory = true;
        }
    }
}
