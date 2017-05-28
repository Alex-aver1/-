using System; /////////////////////////////////////////////////
using System.Collections.Generic;///// Консольный стандарт ////
using System.Text;/////////////////////////////////////////////
using System.Net.Sockets; // Вот он, родимый коллекшн классов /
using System.Threading; // Коллекшн для работы с потоками /////

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
            Server Serv = new Server(); // Создаем новый экземпляр класса 
                                        // сервера
            Serv.Create(port);

            while (true)
            {
                cmd = Console.ReadLine(); // Ждем фразы EXIT когда 
                                          // понадобится выйти из приложения.
                                          // типа интерактивность.
                if (cmd == "EXIT")
                {
                    Serv.Close(); // раз выход – значит выход. Серв-нах.
                    return;
                }
            }
        }

        public class Server // класс сервера.
        {
            private int LocalPort;
            private Thread ServThread; // экземпляр потока
            TcpListener Listener; // листенер))))

            public void Create(int port)
            {
                LocalPort = port;
                ServThread = new Thread(new ThreadStart(ServStart));
                ServThread.Start(); // запустили поток. Стартовая функция – 
                                    // ServStart, как видно выше
            }

            public void Close() // Закрыть серв?
            {
                Listener.Stop();
                ServThread.Abort();
                return;
            }

            private void ServStart()
            {
                Socket ClientSock; // сокет для обмена данными.
                string data;
                byte[] cldata = new byte[1024]; // буфер данных
                Listener = new TcpListener(LocalPort);
                Listener.Start(); // начали слушать
                Console.WriteLine("Waiting connections [" + Convert.ToString(LocalPort) + "]...");
                try
                {
                    ClientSock = Listener.AcceptSocket(); // пробуем принять 
                                                          // клиента
                }
                catch
                {
                    ServThread.Abort(); // нет – жаль(
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
                                i = ClientSock.Receive(cldata); // попытка чтения 
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
                                    if (data == "CLOSE") // если CLOSE – 
                                                         // вырубимся
                                    {
                                        ClientSock.Send(Encoding.ASCII.GetBytes("Closing the server..."));
                                        ClientSock.Close();
                                        Listener.Stop();
                                        Console.WriteLine("Server closed. Reason: client wish! Type EXIT to quit the application.");
                                        ServThread.Abort();
                                        return;
                                    }
                                    else
                                    { // нет – шлем данные взад.
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
                                ClientSock.Close(); // ну эт если какая хрень..
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