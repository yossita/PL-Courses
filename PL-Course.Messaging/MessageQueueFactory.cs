using System.Collections.Generic;
using PL_Course.Messaging.Impl.MSMQ;
using PL_Course.Messaging.Spec;

namespace PL_Course.Messaging
{
    public class MessageQueueFactory
    {

        private static readonly Dictionary<string, IMessageQueue> queues = new Dictionary<string, IMessageQueue>();

        public static IMessageQueue CreateOutbound(string name, MessagePattern pattern, Dictionary<string, object> properties = null)
        {
            var key = string.Format("{0}:{1}:{2}", Direction.Outbound, name, pattern);
            if (queues.ContainsKey(key)) return queues[key];

            var queue = Create();
            queue.InitilizeOutbound(name, pattern, properties);
            queues.Add(key, queue);
            return queues[key];
        }

        public static IMessageQueue CreateInbound(string name, MessagePattern pattern, Dictionary<string, object> properties = null)
        {
            var key = string.Format("{0}:{1}:{2}", Direction.Inbound, name, pattern);
            if (queues.ContainsKey(key)) return queues[key];

            var queue = Create();
            queue.InitilizeInbound(name, pattern, properties);
            queues.Add(key, queue);
            return queues[key];
        }

        private static IMessageQueue Create()
        {
            return new MsmqMessageQueue();
        }
    }
}