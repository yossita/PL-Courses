using System;
using System.Collections.Generic;
using PL_Course.Messaging.Spec;

namespace PL_Course.Messaging.Impl
{
    public abstract class MessageQueueBase : IMessageQueue
    {
        public abstract void Dispose();
        public string Address { get; private set; }
        public Dictionary<string, object> Properties { get; private set; }
        public abstract void InitilizeOutbound(string name, MessagePattern messagePattern, Dictionary<string, object> properties = null);
        public abstract void InitilizeInbound(string name, MessagePattern messagePattern, Dictionary<string, object> properties = null);
        public abstract void Send(Message message);
        public abstract void Listen(Action<Message> onMessageReceived);
        public abstract void Receive(Action<Message> onMessageReceived);
        public abstract IMessageQueue GetResponseQueue();
        public abstract IMessageQueue GetReplyQueue();

        protected Direction Direction;
        protected MessagePattern Pattern;


        protected void Initilize(Direction direction, string name, MessagePattern messagePattern,
            Dictionary<string, object> properties = null)
        {
            Direction = direction;
            Pattern = messagePattern;
            Properties = properties ?? new Dictionary<string, object>();
            Address = GetAddress(name);
        }

        protected abstract string GetAddress(string name);
    }
}