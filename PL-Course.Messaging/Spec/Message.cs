using System;
using System.IO;
using PL_Course.Infrastructure;

namespace PL_Course.Messaging.Spec
{
    public class Message
    {
        private object body;

        public object Body
        {
            get { return body; }
            set
            {
                body = value;
                MessageType = body.GetMessageType();
            }
        }

        public Type BodyType { get { return Body.GetType(); } }

        public string ResponseAddress { get; set; }

        public string MessageType { get; set; }

        public TBody BodyAs<TBody>()
        {
            return (TBody)Body;
        }


        public static Message FromJson(Stream stream)
        {
            return stream.ReadFromJsonStream<Message>();
        }
    }
}
