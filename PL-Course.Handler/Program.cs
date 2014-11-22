using System;
using System.Messaging;
using PL_Course.Infrastructure;
using PL_Course.Messages.Commands;

namespace PL_Course.Handler
{
    class Program
    {
        static void Main(string[] args)
        {

            using (var queue = new MessageQueue(".\\private$\\unsubscribe"))
            {
                while (true)
                {
                    var message = queue.Receive();
                    var unsubscibeMessage = message.BodyStream.ConvertFromJsonStream<UnsubscribeCommand>();
                    var workflow = new UnsubscribeWorkflow(unsubscibeMessage.Email);
                    Console.WriteLine("Starting unsubscribe workflow for: {0}", unsubscibeMessage.Email);
                    workflow.Run();
                    Console.WriteLine("Unsubscribe workflow completed for: {0}", unsubscibeMessage.Email);
                }
            }
        }
    }
}
