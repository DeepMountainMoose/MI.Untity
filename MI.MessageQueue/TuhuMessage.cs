using System;
using System.Collections.Generic;
using System.Text;

namespace MI.MessageQueue
{
    public class TuhuMessage
    {
        private readonly RabbitMQProducerManager _manager;
        private readonly TuhuMessageOptions _options;

        public TuhuMessage(RabbitMQProducerManager manager)
        {
            _manager = manager;
            _options = new TuhuMessageOptions();

            manager.AddExchangeInitialize(_options.ExchageName, producer =>
            {
                producer.ExchangeDeclare(_options.ExchageName);

                producer.QueueBind(_options.SmsQueue, _options.ExchageName);
                producer.QueueBind(_options.MarketingSmsQueue, _options.ExchageName);
                producer.QueueBind(_options.EmailQueue, _options.ExchageName);
                producer.QueueBind(_options.WarnMessageQueue, _options.ExchageName, MessagePriority.Highest);
            });
        }

        #region SendEmail
        [Obsolete("接口已不再使用，请使用EmailProcess服务", true)]
        public void SendEmail(string subject, string to, string message) => SendEmail(subject, to, null, message);

        [Obsolete("接口已不再使用，请使用EmailProcess服务", true)]
        public void SendEmail(string subject, string to, string cc, string message)
        {
            var mm = new Email(_options.EmailQueue, _manager.GetProducer(_options.ExchageName));
            mm.Subject = subject;
            mm.Body = message;

            foreach (var a in to.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                mm.To.Add(a);
            }

            if (!string.IsNullOrEmpty(cc))
                foreach (var a in cc.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    mm.Cc.Add(a);
                }

            mm.Send();
        }
        #endregion

        #region SendWarnMessage
        [Obsolete("接口已不再使用", true)]
        public void SendWarnMessage(string message) => SendWarnMessage(MessagePriority.Normal, message);

        [Obsolete("接口已不再使用", true)]
        public void SendWarnMessage(MessagePriority priority, string message)
        {
            _manager.GetProducer(_options.ExchageName).Send(new Message { To = _options.WarnMessageQueue, Body = message, Priority = priority });
        }
        #endregion
    }
}
