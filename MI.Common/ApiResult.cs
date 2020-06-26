using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Common
{
    /// <summary>
    ///     接口结果基类型 
    /// </summary>
    public class ApiResult
    {
        /// <summary>
        ///     构造函数 
        /// </summary>
        public ApiResult(int errorCode, string message)
        {
            ErrorCode = errorCode;
            Message = message;
        }

        /// <summary>
        ///     接口执行成功标识
        ///     <c>true</c>:执行成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        ///     错误代码
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        ///     错误信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     设置成功状态
        /// </summary>
        public virtual void SetSuccess()
        {
            IsSuccess = true;
        }

        /// <summary>
        ///     设置失败状态
        /// </summary>
        /// <param name="errorCode">错误编号</param>
        public virtual void SetFailed(int errorCode)
        {
            IsSuccess = false;
            ErrorCode = errorCode;
        }

        /// <summary>
        ///     设置失败消息
        /// </summary>
        public virtual void SetFailed(string msg)
        {
            IsSuccess = false;
            ErrorCode = -1;
            Message = msg;
        }

        /// <summary>
        ///     设置失败消息
        /// </summary>
        /// <param name="errorCode">错误编号</param>
        /// <param name="msg">错误内容</param>
        public virtual void SetFailed(int errorCode, string msg)
        {
            IsSuccess = false;
            ErrorCode = errorCode;
            Message = msg;
        }

        /// <summary>
        ///     重写tostring
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0},ErrorCode:{1},Message:{2}", IsSuccess, ErrorCode, Message);
        }
    }

    /// <summary>
    ///     统一函数返回值
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class ApiResult<TResult> : ApiResult
    {
        /// <summary>
        ///     构造函数
        /// </summary>
        public ApiResult(int errorCode, string message, TResult result = default(TResult))
            : base(errorCode, message)
        {
            Result = result;
        }


        /// <summary>
        ///     扩展数据
        /// </summary>
        public TResult Result { get; set; }

        /// <summary>
        ///     设置返回数据
        /// </summary>
        public virtual void SetSuccess(TResult data)
        {
            IsSuccess = true;
            Result = data;
        }

        /// <summary>
        ///     设置返回数据
        /// </summary>
        public virtual ApiResult<TResult> SetApiResult(TResult data)
        {
            Result = data;
            return this;
        }
    }
}
