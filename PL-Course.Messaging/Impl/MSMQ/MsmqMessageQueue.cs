using System;
using System.Collections.Generic;
using PL_Course.Infrastructure;
using msmq = System.Messaging;
using PL_Course.Messaging.Spec;

namespace PL_Course.Messaging.Impl.MSMQ
{
    public class MsmqMessageQueue : MessageQueueBase
    {
        private bool useTemporaryQueue;
        private msmq.MessageQueue queue;

        public override void Dispose()
        {
            queue.Close();
        }

        public override void InitilizeOutbound(string name, MessagePattern messagePattern, Dictionary<string, object> properties = null)
        {
            Initilize(Direction.Outbound, name, messagePattern, properties);
            queue = new msmq.MessageQueue(Address);
        }

        public override void InitilizeInbound(string name, MessagePattern messagePattern, Dictionary<string, object> properties = null)
        {
            Initilize(Direction.Inbound, name, messagePattern, properties);
            switch (messagePattern)
            {
                case MessagePattern.PublishSubscribe:
                    queue = new msmq.MessageQueue(Address);
                    break;
                case MessagePattern.RequestResponse:
                    queue = useTemporaryQueue ? msmq.MessageQueue.Create(Address) : new msmq.MessageQueue(Address);
                    break;
                default:
                    queue = new msmq.MessageQueue(Address);
                    break;
            }
        }

        public override void Send(Message message)
        {
            var outbound = new msmq.Message();
            outbound.BodyStream = message.ToJsonStream();
            outbound.Label = message.GetMessageType();
            if (!string.IsNullOrEmpty(message.ResponseAddress))
            {
                outbound.ResponseQueue = new msmq.MessageQueue(message.ResponseAddress);
            }
            queue.Send(outbound);
        }

        public override void Listen(Action<Message> onMessageReceived)
        {
            while (true)
            {
                Receive(onMessageReceived);
            }
        }

        public override void Receive(Action<Message> onMessageReceived)
        {
            var inbound = queue.Receive();
            var message = Message.FromJson(inbound.BodyStream);
            onMessageReceived(message);
        }

        public override IMessageQueue GetResponseQueue()
        {
            return this;
        }

        public override IMessageQueue GetReplyQueue()
        {
            return this;
        }

        protected override string GetAddress(string name)
        {
            if (Pattern == MessagePattern.RequestResponse && Direction == Spec.Direction.Inbound)
            {
                useTemporaryQueue = true;
                return string.Format(".\\private$\\temp.{0}", Guid.NewGuid().ToString().Substring(0, 6));
            }
            if (name.EndsWith("-event", StringComparison.OrdinalIgnoreCase))
            {
                return "FormatName:MULTICAST=234.1.1.2:8001";
            }
            return string.Format(".\\private$\\{0}", name);
        }
    }
}