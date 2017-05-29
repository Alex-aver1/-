using System;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace SocketUdpClient
{
    class Program
    {
        static int localPort; // порт приема сообщений
        static int remotePort; // порт для отправки сообщений
        static Socket listeningSocket;

        static void Main(string[] args)
        {
            localPort = 8004;
            remotePort = 8005;


            try
            {
                listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                //Прослушиваем по адресу
                IPEndPoint localIP = new IPEndPoint(IPAddress.Parse("192.168.1.37"), localPort);
                listeningSocket.Bind(localIP);

                // отправка сообщений на разные порты

                DateTime time = DateTime.Now;

                byte[] data = BitConverter.GetBytes(time.ToBinary());
                EndPoint remotePoint = new IPEndPoint(IPAddress.Parse("192.168.1.138"), remotePort);
                listeningSocket.SendTo(data, remotePoint);

                Console.WriteLine("IP-адрес сервера: " + remotePoint);


                // получаем сообщение


                StringBuilder builder = new StringBuilder();
                int bytes = 0; // количество полученных байтов
                data = new byte[256]; // буфер для получаемых данных

                //адрес, с которого пришли данные
                EndPoint remoteIp = new IPEndPoint(IPAddress.Any, 0);
                do {
                    bytes = listeningSocket.ReceiveFrom(data, ref remoteIp);  
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (listeningSocket.Available > 0);
                // получаем данные о подключении
                IPEndPoint remoteFullIp = remoteIp as IPEndPoint;

                // выводим сообщение
                Console.WriteLine("{0}:{1} - {2}", remoteFullIp.Address.ToString(), remoteFullIp.Port, builder.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Close();
            }

            Console.WriteLine("Нажмите любую кнопку для выхода");
            Console.ReadKey();
        }
        
        private static void Close()
        {
            if (listeningSocket != null)
            {
                listeningSocket.Shutdown(SocketShutdown.Both);
                listeningSocket.Close();
                listeningSocket = null;
            }
        }
    }
}