using System;
using System.Collections.Generic;

namespace PL_Course.Messaging.Spec
{
    public interface IMessageQueue : IDisposable
    {

        string Address { get; }
        Dictionary<string, object> Properties { get; }

        void InitilizeOutbound(string name, MessagePattern messagePattern, Dictionary<string, object> properties = null);
        void InitilizeInbound(string name, MessagePattern messagePattern, Dictionary<string, object> properties = null);
        void Send(Message message);
        void Listen(Action<Message> onMessageReceived);
        void Receive(Action<Message> onMessageReceived);
        IMessageQueue GetResponseQueue();
        IMessageQueue GetReplyQueue();


    }
}