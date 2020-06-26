using MI.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;

namespace MI.Library.Interface.Common
{
    /// <summary>
    ///     请求上下文对象
    ///     标识当前请求中的平台/操作人/渠道/企业Id等信息
    /// </summary>
    public sealed class RequestContext
    {
        private static readonly AsyncLocal<string> CurrentUserId = new AsyncLocal<string>();
        private static readonly AsyncLocal<RequestContext> RequestContent = new AsyncLocal<RequestContext>();
        private static readonly AsyncLocal<bool> IsTrack = new AsyncLocal<bool>();

        /// <summary>
        ///     获取当前操作的用户Id
        /// </summary>
        public static string GetCurrentUserId => CurrentUserId.Value;

        public static bool GetIsTrack => IsTrack.Value;

        public static void SetIsTrack(bool isTrack)
        {
            IsTrack.Value = isTrack;
        }

        /// <summary>
        ///     设置当前操作的用户Id
        /// </summary>
        /// <param name="userId"></param>
        public static void SetCurrentUserId(string userId)
        {
            CurrentUserId.Value = userId;
        }

        /// <summary>
        ///     获取当前请求上下文
        /// </summary>
        /// <returns></returns>
        public static RequestContext GetData()
        {
            if (RequestContent.Value == null)
                RequestContent.Value = new RequestContext(Platform.None, string.Empty);
            return RequestContent.Value;
        }

        /// <summary>
        ///     设置当前访问上下文
        /// </summary>
        /// <param name="requestContext"></param>
        public static void SetData(RequestContext requestContext)
        {
            RequestContent.Value = requestContext;
        }

        /// <summary>
        ///     初始化上下文信息
        /// </summary>
        /// <param name="platform">系统运行平台</param>
        /// <param name="operatorId">操作人编号</param>
        public RequestContext(Platform platform, string operatorId) : this(platform, operatorId, "", "", "", "", "")
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="platform">系统运行平台</param>
        /// <param name="operatorId">操作人编号</param>
        /// <param name="channelId">渠道Id</param>
        /// <param name="enterpriseId">企业Id</param>
        public RequestContext(Platform platform, string operatorId, string channelId, string enterpriseId) : this(platform, operatorId, channelId, enterpriseId, "", "", "")
        {

        }

        /// <summary>
        ///     初始化上下文信息
        /// </summary>
        /// <param name="platform">系统运行平台</param>
        /// <param name="operatorId">操作人编号</param>
        /// <param name="channelId">渠道Id</param>
        /// <param name="enterpriseId">企业Id</param>
        /// <param name="remark">备注</param>
        /// <param name="command">特殊命令</param>
        /// <param name="data">附加数据</param>
        public RequestContext(Platform platform, string operatorId, string channelId = null, string enterpriseId = null,
            string remark = null, string command = null) : this(platform, operatorId, channelId, enterpriseId, remark, command, "")
        {

        }


        /// <summary>
        ///     初始化上下文信息
        /// </summary>
        /// <param name="platform">系统运行平台</param>
        /// <param name="operatorId">操作人编号</param>
        /// <param name="channelId">渠道Id</param>
        /// <param name="enterpriseId">企业Id</param>
        /// <param name="remark">备注</param>
        /// <param name="command">特殊命令</param>
        /// <param name="data">附加数据</param>
        public RequestContext(Platform platform, string operatorId, string channelId, string enterpriseId, string remark, string command, string data = "")
        {
            Platform = platform;
            OperatorId = operatorId;
            ChannelId = channelId;
            EnterpriseId = enterpriseId;
            Remark = remark;
            Command = CommandType.None;
            Data = new Dictionary<string, string>();

            SetCommand(command);
            SetData(data);

        }

        /// <summary>
        ///     所处平台
        /// </summary>
        [DefaultValue(Platform.None)]
        public Platform Platform { get; }

        /// <summary>
        ///     操作人
        /// </summary>
        public string OperatorId { get; }

        /// <summary>
        ///     操作备注
        /// </summary>
        public string Remark { get; internal set; }

        /// <summary>
        ///     渠道Id
        /// </summary>
        public string ChannelId { get; private set; }

        /// <summary>
        ///     企业Id
        /// </summary>
        public string EnterpriseId { get; private set; }

        /// <summary>
        ///     特殊命令
        /// </summary>
        [DefaultValue(CommandType.None)]
        public CommandType Command { get; private set; }

        /// <summary>
        ///     附加的数据信息
        /// </summary>
        public IDictionary<string, string> Data { get; private set; }

        /// <summary>
        /// 命令
        /// </summary>
        public string CommandStr { get; private set; }

        /// <summary>
        ///     设置附属数据
        /// </summary>
        /// <param name="data"></param>
        internal void SetData(string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                data = Uri.UnescapeDataString(data);
                if (Data == null)
                    Data = new Dictionary<string, string>();
                foreach (var item in data.Split('&'))
                {
                    var d = item.Split(':');
                    if (d.Length != 2)
                        continue;
                    var key = d[0].ToLowerInvariant();
                    if (Data.ContainsKey(key))
                        continue;
                    if (!string.IsNullOrEmpty(d[1]))
                        Data.Add(key, d[1]);
                }
            }
        }

        /// <summary>
        ///     设置特殊命令
        /// </summary>
        /// <param name="commandType"></param>
        internal void SetCommand(CommandType commandType)
        {
            Command = Command | commandType;
        }

        /// <summary>
        ///     设置特殊命令
        /// </summary>
        /// <param name="command"></param>
        internal void SetCommand(string command)
        {
            CommandStr = command;
            Command = CommandTypeConvert.GetCommandType(command);
        }

        /// <summary>
        ///     设置当前的特殊命令
        /// </summary>
        /// <param name="command"></param>
        public static void SetCurrentCommand(string command)
        {
            GetData().SetCommand(command);
        }

        /// <summary>
        ///     设置当前的备注
        /// </summary>
        /// <param name="remark"></param>
        public static void SetCurrentRemark(string remark)
        {
            GetData().Remark = remark;
        }

        public static void SetCurrentChannelId(string channelId)
        {
            GetData().ChannelId = channelId;
        }

        public static void SetCurrentEnterpriseId(string enterprise)
        {
            GetData().EnterpriseId = enterprise;
        }

        /// <summary>Creates a new object that is a copy of the current instance.</summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            return MemberwiseClone();
        }

    }
}
