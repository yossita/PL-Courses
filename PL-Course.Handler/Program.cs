using System;
using System.Reflection;
using PL_Course.Infrastructure;
using PL_Course.Integration.Workflows;
using PL_Course.Messages.Commands;
using PL_Course.Messages.Queries;
using PL_Course.Messaging;
using PL_Course.Messaging.Spec;
using Message = System.Messaging.Message;

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
                    StartListening("doesuserexists", MessagePattern.RequestResponse);
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
                    CheckDoesUserExist(m.BodyAs<DoesUserExistRequest>(), queue);
                }
            });
        }

        private static void CheckDoesUserExist(DoesUserExistRequest doesUserExistRequest, IMessageQueue queue)
        {
            Console.WriteLine("Starting DoesUserExist for: {0}, at {1}", doesUserExistRequest.Email, DateTime.Now);
            var doesUserExistResponse = new DoesUserExistResponse()
            {
                Exists = new DoesUserExistWorkflow().DoesUserExists(doesUserExistRequest.Email)
            };

            var responseQueue = queue.GetReplyQueue();
            responseQueue.Send(new Messaging.Spec.Message() { Body = doesUserExistResponse });
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
