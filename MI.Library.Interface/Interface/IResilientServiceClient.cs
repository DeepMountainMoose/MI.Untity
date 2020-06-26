using ServiceClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Library.Interface
{
    /// <summary>
    ///     带重试功能的<see cref="ServiceClient"/>
    /// <remarks>
    ///     查询类操作建议使用, 请勿用于写入类接口调用
    /// </remarks>
    /// </summary>
    public interface IResilientServiceClient : IServiceClient
    {
    }
}
