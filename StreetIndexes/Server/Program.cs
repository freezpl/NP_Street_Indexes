using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Server
{
    class Program
    {
        static IPAddress ip;

        static void Main(string[] args)
        {
            IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var adr in hostEntry.AddressList)
                if (adr.AddressFamily == AddressFamily.InterNetwork)
                    ip = adr;

            if (ip == null)
                return;

            try
            {
                IPEndPoint endPoint = new IPEndPoint(ip, 5630);
                Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                listenSocket.Bind(endPoint);

                listenSocket.Listen(100);
                Console.WriteLine("Server running...");

                while(true)
                {
                    Socket handler = listenSocket.Accept();
                    StringBuilder strBuilder = new StringBuilder();
                    int bytes = 0;
                    byte[] data = new byte[2];
                    do
                    {
                        bytes = handler.Receive(data);
                        Console.WriteLine(bytes);
                        strBuilder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (handler.Available > 0);

                    Console.WriteLine(strBuilder);
                    Console.ReadLine();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message + "\n" + e.StackTrace);
            }
            //Console.WriteLine(ip);
        }
    }
}
