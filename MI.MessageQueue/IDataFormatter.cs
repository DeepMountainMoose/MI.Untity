using System;
using System.Collections.Generic;
using System.Text;

namespace MI.MessageQueue
{
    public interface IDataFormatter
    {
        /// <summary>反序列化</summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="data">数据</param>
        /// <returns>对象</returns>
        T Deserialize<T>(byte[] data);

        /// <summary>序列化</summary>
        /// <param name="objData">对象</param>
        /// <returns>数据</returns>
        byte[] Serialize(object objData);
    }


}
