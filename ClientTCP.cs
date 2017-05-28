using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace tcpclient
{
    class Program
    {
        static void Main(string[] args)
        {
            string data; 
            byte[] remdata = new byte[1024];
            TcpClient Client = new TcpClient();
            Console.Write("IP to connect to: ");
            string ip = Console.ReadLine();
            Console.Write("\nPort: ");
            int port = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("\nConnecting to server...");
            try
            {
                Client.Connect(ip, port);
            }
            catch
            {
                Console.WriteLine("Cannot connect to remote host!");
                return;
            }
            Console.Write("done\nTo end, type 'END'");
            Socket Sock = Client.Client;
            int i = 0;
            while (true)
            {
                Console.WriteLine(">");
                data = Console.ReadLine();
                if (data == "END")
                    break;
                Sock.Send(Encoding.ASCII.GetBytes(data));
                i = Sock.Receive(remdata);
                if (i > 0)
                {
                    string otvet = Encoding.ASCII.GetString(remdata).Trim();
                    Console.WriteLine("<" + otvet);
                }
            }
            Sock.Close();
            Client.Close();
        }
    }
}
