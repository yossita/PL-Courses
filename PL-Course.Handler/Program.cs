using System;
using System.Reflection;
using PL_Course.Integration.Workflows;
using PL_Course.Messages.Commands;
using PL_Course.Messages.Queries;
using PL_Course.Messaging;
using PL_Course.Messaging.Spec;

namespace PL_Course.Handler
{
    class Program
    {
        static void Main(string[] args)
        {
            switch (args[0])
            {
                case "unsubscribe":
                    StartListening("unsubscribe", MessagePattern.FireAndForget);
                    break;
                case "doesuserexist":
                    StartListening("doesuserexist", MessagePattern.RequestResponse);
                    break;
                default:
                    Console.WriteLine("Usage: {0} [unsubscribe|doesuserexist]", Assembly.GetExecutingAssembly().FullName);
                    break;
            }
        }

        private static void StartListening(string name, MessagePattern messagePattern)
        {
            var queue = MessageQueueFactory.CreateInbound(name, messagePattern);
            Console.WriteLine("Listening on: {0}", queue.Address);
            queue.Listen(m =>
            {
                if (m.BodyType == typeof(UnsubscribeCommand))
                {
                    Unsubscribe(m.BodyAs<UnsubscribeCommand>());
                }
                else if (m.BodyType == typeof(DoesUserExistRequest))
                {
                    CheckDoesUserExist(m, queue);
                }
                else
                {
                    Console.WriteLine("Received message with message type '{0} which doesn't have a handler", m.BodyType);
                }
            });
        }

        private static void CheckDoesUserExist(Message doesUserExistRequestMessage, IMessageQueue queue)
        {
            var doesUserExistRequest = doesUserExistRequestMessage.BodyAs<DoesUserExistRequest>();
            Console.WriteLine("Starting DoesUserExist for: {0}, at {1}", doesUserExistRequest.Email, DateTime.Now);
            var doesUserExistResponse = new DoesUserExistResponse()
            {
                Exists = new DoesUserExistWorkflow().DoesUserExists(doesUserExistRequest.Email)
            };

            var responseQueue = queue.GetReplyQueue(doesUserExistRequestMessage);
            responseQueue.Send(new Message() { Body = doesUserExistResponse });
            Console.WriteLine("Returned {0} for DoesUserExist for: {1}, at {2}", doesUserExistResponse.Exists, doesUserExistRequest.Email, DateTime.Now);
        }

        private static void Unsubscribe(UnsubscribeCommand command)
        {
            Console.WriteLine("Starting Unsubscribe for: {0}, at {1}", command.Email, DateTime.Now);
            var workflow = new UnsubscribeWorkflow(command.Email);
            workflow.Run();
            Console.WriteLine("Unsubscribe completed for: {0}, at {1}", command.Email, DateTime.Now);
        }
    }
}
