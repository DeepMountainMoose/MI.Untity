using System;
using System.Collections.Generic;
using System.Text;

namespace MI.MessageQueue
{
    internal class Email
    {
        private readonly string _emailQueue;
        private readonly IMessageProducer _producer;
        public Email() { }
        public Email(string emailQueue, IMessageProducer producer)
        {
            _emailQueue = emailQueue;
            _producer = producer;
        }

        public virtual string Subject { get; set; }
        public virtual ICollection<string> To { get; } = new List<string>();
        public virtual ICollection<string> Cc { get; } = new List<string>();
        public virtual string Body { get; set; }

        public virtual void Send()
        {
            _producer.Send(_emailQueue, this);
        }
    }
}
