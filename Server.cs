using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

using ZeroMQ;

namespace Examples
{
    static partial class Program
    {
        public static void Main(string[] args)
        {
            if (args == null || args.Length < 1)
            {
                Console.WriteLine("Hello! Welcome to server!");
                Console.WriteLine();
            }
            
            // Create
            using (var context = new ZContext())
            using (var responder = new ZSocket(context, ZSocketType.REP))
            {
                // Bind
                responder.Bind("tcp://*:4005");

                while (true)
                {
                    // Receive
                    using (ZFrame request = responder.ReceiveFrame())
                    {
                        string receivedString = request.ReadString();
                        Console.WriteLine("Received {0}", receivedString);

                        // Do some work
                        Thread.Sleep(1);

                        // Send
                        string a = Encoding.UTF8.GetBytes(receivedString).Length.ToString();
                        responder.Send(new ZFrame(a));
                    }
                }
            }
        }
    }
}
