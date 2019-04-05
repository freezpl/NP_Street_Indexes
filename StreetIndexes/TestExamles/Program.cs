using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace TestExamles
{
    class Program
    {
        static void Main(string[] args)
        {
            //IPAddress ip = IPAddress.Loopback;

            //Get ip by hostname
            //IPHostEntry myHost = Dns.GetHostEntry(Dns.GetHostName());

            //IPHostEntry myHost = Dns.GetHostEntry("microsoft.com");
            //Console.WriteLine(myHost.HostName + "\n");
            //foreach (IPAddress ip in myHost.AddressList)
            //{
            //    if (ip.AddressFamily == AddressFamily.InterNetwork)
            //        Console.WriteLine(ip);
            //}

            ////Downloading files
            //WebClient client = new WebClient();
            //string path = @"http://terra.rv.ua/price/download.php?price";
            //client.DownloadFile(path, "price.zip");

            //string requestStr = @"https://maps.googleapis.com/maps/api/place/autocomplete/json?key=AIzaSyCRuMrU5eZqvCXaxRg8FOacV9iea6SBEkI&input=Rivn";
            //WebRequest request = WebRequest.Create(requestStr);
            //WebResponse response = request.GetResponse();

            //using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            //{

            //    string responseText = reader.ReadToEnd();
            //   // Console.WriteLine(responseText);

            //    Cityes c = JsonConvert.DeserializeObject<Cityes>(responseText);

            //    foreach (var item in c.predictions)
            //    {
            //        Console.WriteLine(item.description);
            //    }
            //}

            //Console.WriteLine(ip);
            // Console.ReadLine();

            Work();
            Console.WriteLine("End program");
        }

        async static void Work()
        {
            Thread.Sleep(1000);

            Console.WriteLine("Start method");
            await Task.Run(() => {
                Thread.Sleep(1000);
                Console.WriteLine("End task");
            });
            Console.WriteLine("End method");
        }
    }
    

    //public class Cityes
    //{
    //    public List<City> predictions { get; set; }

    //    public Cityes()
    //    {
    //        predictions = new List<City>();


    //    }
    //}

    //public class City
    //{
    //    public string description { get; set; }
    //}
}
