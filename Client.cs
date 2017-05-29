using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using ZeroMQ;

namespace Examples
{
    static partial class Program
    {
        public static void Main(string[] args)
        {
            if (args == null || args.Length < 1)
            {
                Console.WriteLine("    Endpoint, where client should connect to:");
                Console.WriteLine("              default is tcp://192.168.1.138:4005");
                Console.WriteLine();
                args = new string[] { "tcp://192.168.1.138:4005" };
            }

            string endpoint = args[0];

            // Create
            using (var context = new ZContext())
            using (var requester = new ZSocket(context, ZSocketType.REQ))
            {   
                // Connect
                requester.Connect(endpoint);

               
                Console.WriteLine("Enter the message: \n");
                string requestText = Console.ReadLine();
                Console.WriteLine("Sending {0}…", requestText);
                Console.WriteLine();

                // Send
                requester.Send(new ZFrame(requestText));

                // Receive
                using (ZFrame reply = requester.ReceiveFrame())
                {
                    Console.WriteLine(" You have sent the message: {0}, Its length in bytes: {1}\n", requestText, reply.ReadString());
                }
            }
        }
    }
}
