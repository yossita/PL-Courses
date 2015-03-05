using System;
using System.Collections.Generic;
using PL_Course.Infrastructure;
using msmq = System.Messaging;
using PL_Course.Messaging.Spec;

namespace PL_Course.Messaging.Impl.MSMQ
{
    public class MsmqMessageQueue : MessageQueueBase
    {

        private msmq.MessageQueue queue;
        private bool createQueueIfNotExists = true;
        private string multicastAddress = "234.1.1.2:8001";

        public override void Dispose()
        {
            queue.Close();
        }

        public override void InitilizeOutbound(string name, MessagePattern messagePattern, Dictionary<string, object> properties = null)
        {
            Initilize(Direction.Outbound, name, messagePattern, properties);
            queue = GetOutboundQueue();
        }

        public override void InitilizeInbound(string name, MessagePattern messagePattern, Dictionary<string, object> properties = null)
        {
            Initilize(Direction.Inbound, name, messagePattern, properties);
            queue = GetInboundQueue();
        }

        private msmq.MessageQueue GetInboundQueue()
        {
            msmq.MessageQueue queue;
            if (createQueueIfNotExists && !msmq.MessageQueue.Exists(Address))
            {
                if (Pattern == MessagePattern.PublishSubscribe)
                {
                    queue = msmq.MessageQueue.Create(Address);
                    queue.MulticastAddress = multicastAddress;
                    queue.SetPermissions("anonymous logon", msmq.MessageQueueAccessRights.ReceiveMessage | msmq.MessageQueueAccessRights.PeekMessage);
                }
                else
                {
                    queue = msmq.MessageQueue.Create(Address);
                }
            }
            else
            {
                queue = new msmq.MessageQueue(Address);
            }
            return queue;
        }

        private msmq.MessageQueue GetOutboundQueue()
        {
            if (createQueueIfNotExists && Pattern != MessagePattern.PublishSubscribe && !msmq.MessageQueue.Exists(Address))
            {
                return msmq.MessageQueue.Create(Address);
            }
            return new msmq.MessageQueue(Address);
        }

        public override void Send(Message message)
        {
            var outbound = new msmq.Message();
            outbound.BodyStream = message.ToJsonStream();
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
            if (!(Pattern == MessagePattern.RequestResponse && Direction == Direction.Outbound))
            {
                throw new InvalidOperationException("Cannot get a reply queue except for outbound request/response queue");
            }
            var responseQueueName = string.Format("response.tmp.{0}", Guid.NewGuid().ToString().Substring(0, 6));
            var responseQueue = MessageQueueFactory.CreateInbound(responseQueueName, MessagePattern.RequestResponse);
            return responseQueue;
        }

        public override IMessageQueue GetReplyQueue(Message message)
        {
            if (!(Pattern == MessagePattern.RequestResponse && Direction == Direction.Inbound))
            {
                throw new InvalidOperationException("Cannot get a reply queue except for inbound request/response queue");
            }
            var responseQueue = MessageQueueFactory.CreateOutbound(message.ResponseAddress, MessagePattern.RequestResponse);
            return responseQueue;
        }

        protected override string GetAddress(string name)
        {
            if (Pattern == MessagePattern.PublishSubscribe && Direction == Direction.Outbound)
            {
                return string.Format("FormatName:MULTICAST={0}", multicastAddress);
            }
            if (name.StartsWith(".\\private$\\")) return name;
            return string.Format(".\\private$\\messagequeue.{0}", name);
        }
    }
}