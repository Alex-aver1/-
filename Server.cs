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
            localPort = 8005;
            remotePort = 8004;
            Console.WriteLine("Hello! Welcome to server!");

            try
            {
                listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                Task listeningTask = new Task(Listen);
                Listen();


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Close();
            }
        }

        private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);


        // поток для приема подключений
        private static void Listen()
        {
            try
            {
                //Прослушиваем по адресу
                IPEndPoint localIP = new IPEndPoint(IPAddress.Parse("192.168.1.138"), localPort);
                listeningSocket.Bind(localIP);

                while (true)
                {

                    // получаем сообщение
                    StringBuilder builder = new StringBuilder();

                    //адрес, с которого пришли данные
                    EndPoint remoteIp = new IPEndPoint(IPAddress.Parse("192.168.1.37"), remotePort);

                    byte[] data = new byte[sizeof(long)];
                    listeningSocket.ReceiveFrom(data, ref remoteIp);

                    DateTime clientTime = DateTime.FromBinary(BitConverter.ToInt64(data, 0));
                    DateTime currentTime = DateTime.Now;
                    String msg = "разница в миллисекундах " + currentTime.Subtract(clientTime).TotalMilliseconds;
                    Console.WriteLine(msg);

                    // получаем данные о подключении
                    IPEndPoint remoteFullIp = remoteIp as IPEndPoint;

                    listeningSocket.SendTo(Encoding.Unicode.GetBytes(msg), remoteIp);
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Close();
            }
        }
        // закрытие сокета
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
