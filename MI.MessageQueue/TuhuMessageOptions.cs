using System;
using System.Collections.Generic;
using System.Text;

namespace MI.MessageQueue
{
    public class TuhuMessageOptions
    {
        public string ExchageName { get; set; } = "direct.messageExchage";
        public string SmsQueue { get; set; } = "message.sms";
        public string MarketingSmsQueue { get; set; } = "message.marketingSms";
        public string EmailQueue { get; set; } = "message.email";
        public string WarnMessageQueue { get; set; } = "message.warnMessage";
    }
}
