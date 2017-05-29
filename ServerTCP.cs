using System; 
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets; 
using System.Threading; 

namespace tcpserver
{
    class Program
    {
        static void Main(string[] args)
        {
            string cmd;
            Console.Write("Port to listen: ");
            int port = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Creating server...");
            Server Serv = new Server(); 
            Serv.Create(port);

            while (true)
            {
                cmd = Console.ReadLine(); 
                if (cmd == "EXIT")
                {
                    Serv.Close(); 
                    return;
                }
            }
        }

        public class Server 
        {
            private int LocalPort;
            private Thread ServThread; 
            TcpListener Listener; 

            public void Create(int port)
            {
                LocalPort = port;
                ServThread = new Thread(new ThreadStart(ServStart));
                ServThread.Start(); 
            }

            public void Close() 
            {
                Listener.Stop();
                ServThread.Abort();
                return;
            }

            private void ServStart()
            {
                Socket ClientSock; 
                string data;
                byte[] cldata = new byte[1024]; 
                Listener = new TcpListener(LocalPort);
                Listener.Start(); 
                Console.WriteLine("Waiting connections [" + Convert.ToString(LocalPort) + "]...");
                try
                {
                    ClientSock = Listener.AcceptSocket(); 
                }
                catch
                {
                    ServThread.Abort(); 
                    return;
                }
                int i = 0;

                if (ClientSock.Connected)
                {
                    while (true)
                    {
                        while (true)
                        {
                            for (int index = 0; index < cldata.Length; index++)
                            cldata[index] = 0;
                            try
                            {
                                i = ClientSock.Receive(cldata);
                            }
                            catch
                            {
                            }

                            try
                            {
                                if (i > 0)
                                {
                                    data = Encoding.ASCII.GetString(cldata, 0, i).Trim();
                                    Console.WriteLine("<" + data);
                                    if (data == "CLOSE") 
                                    {
                                        ClientSock.Send(Encoding.ASCII.GetBytes("Closing the server..."));
                                        ClientSock.Close();
                                        Listener.Stop();
                                        Console.WriteLine("Server closed. Reason: client wish! Type EXIT to quit the application.");
                                        ServThread.Abort();
                                        return;
                                    }
                                    else
                                    { 
                                        StringBuilder sb = new StringBuilder();
                                        for (int index = data.Length - 1; index >= 0; index--)
                                            sb.Append(data[index]);
                                        string rev = sb.ToString();
                                        Console.WriteLine("data: " + data + "\nrev: " + rev);
                                        ClientSock.Send(Encoding.ASCII.GetBytes("Your data: " + rev));
                                    }
                                }
                            }
                            catch
                            {
                                ClientSock.Close(); 
                                Listener.Stop();
                                Console.WriteLine("Server closing. Reason: client offline. Type EXIT to quit the application.");
                                ServThread.Abort();
                            }
                        }
                    }
                }
            }
        }
    }
}
