using System;
using System.Messaging;
using PL_Course.Infrastructure;
using PL_Course.Integration.Workflows;
using PL_Course.Messages.Commands;
using PL_Course.Messages.Queries;

namespace PL_Course.Handler
{
    class Program
    {
        static void Main(string[] args)
        {
            var queueAddress = args != null && args.Length == 1
                ? args[0]
                : ".\\private$\\unsubscribe";

            using (var queue = new MessageQueue(queueAddress))
            {
                while (true)
                {
                    using (var tx = new MessageQueueTransaction())
                    {
                        Console.WriteLine("Listening on {0}", queueAddress);
                        //tx.Begin();
                        var message = queue.Receive();
                        var messageBody = message.BodyStream.ReadFromJsonStream(message.Label);
                        if (messageBody.GetType() == typeof(UnsubscribeCommand))
                        {
                            Unsubscribe((UnsubscribeCommand)messageBody);
                        }
                        else if (messageBody.GetType() == typeof(DoesUserExistRequest))
                        {
                            CheckDoesUserExist((DoesUserExistRequest)messageBody, message);
                        }
                    }
                }
            }
        }

        private static void CheckDoesUserExist(DoesUserExistRequest doesUserExistRequest, Message message)
        {
            Console.WriteLine("Starting DoesUserExist for: {0}, at {1}", doesUserExistRequest.Email, DateTime.Now);
            var doesUserExistResponse = new DoesUserExistResponse()
            {
                Exists = new DoesUserExistWorkflow().DoesUserExists(doesUserExistRequest.Email)
            };
            using (var responseQueue = message.ResponseQueue)
            {
                var response = new Message();
                response.BodyStream = doesUserExistResponse.ToJsonStream();
                response.Label = doesUserExistResponse.GetMessageType();
                responseQueue.Send(response);
            }
            Console.WriteLine("Returned {0} for DoesUserExist for: {1}, at {2}", doesUserExistResponse.Exists, doesUserExistRequest.Email, DateTime.Now);
        }

        private static void Unsubscribe(UnsubscribeCommand command)
        {
            Console.WriteLine("Starting Unsubscribe for: {0}, at {1}", command.Email, DateTime.Now);
            var workflow = new UnsubscribeWorkflow(command.Email);
            workflow.Run();
            Console.WriteLine("Unsubscribe completed for: {0}, at {1}", command.Email, DateTime.Now);
            //tx.Commit();
        }
    }
}
