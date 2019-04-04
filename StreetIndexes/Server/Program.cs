using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Configuration;
using System.Threading;
using System.IO;
using Newtonsoft.Json;
using Server.Models;

namespace Server
{
    class Program
    {
        static IPAddress ip;
        static string connection;

        static void Main(string[] args)
        {
            connection = ConfigurationSettings.AppSettings[0];

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

                while (true)
                {
                    Socket socket = listenSocket.Accept();
                    LoadPlace(socket);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\n" + e.StackTrace);
            }
        }

        async static void LoadPlace(Socket socket)
        {
            StringBuilder placeId = new StringBuilder();
            int bytes = 0;
            byte[] data = new byte[256];
            do
            {
                bytes = socket.Receive(data);
                placeId.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (socket.Available > 0);

            await Task.Run(() =>
            {
                try
                {
                    WebRequest request = WebRequest.Create(connection + placeId);
                    WebResponse response = request.GetResponse();
                    
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string responseText = reader.ReadToEnd();
                        socket.Send(Encoding.Unicode.GetBytes(responseText));
                        socket.Shutdown(SocketShutdown.Both);
                        socket.Close();
                        Console.WriteLine("Send data and close socket");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                
            });
        }
    }
}
