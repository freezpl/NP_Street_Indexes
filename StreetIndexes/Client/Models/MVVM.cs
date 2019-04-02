using Client.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Models
{
    public class MVVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<string> Places { get; set; }
        const string REQ_AUTOCOMP = @"https://maps.googleapis.com/maps/api/place/autocomplete/json?key=AIzaSyCRuMrU5eZqvCXaxRg8FOacV9iea6SBEkI&input=";

        private string searchText;
        public string SearchText
        {
            get
            {
                return searchText;
            }
            set
            {
                searchText = value;
                if (!selectPlace)
                {
                    if (t != null && t.Status == TaskStatus.Running)
                    {
                        cts.Cancel();
                        t.Wait();
                    }
                    SearchPlaces();
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchText)));
                selectPlace = false;
            }
        }

        bool selectPlace;
        private bool isDropdOpen;
        public bool IsDropdOpen
        {
            get { return isDropdOpen; }
            set
            {
                isDropdOpen = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDropdOpen)));
            }
        }


        public MVVM()
        {
            Places = new ObservableCollection<string>();
            SearchText = string.Empty;
        }

        Task t;
        CancellationTokenSource cts;
        CancellationToken token;
        WebRequest request;
        WebResponse response;
        AllPlaces all;

        void SearchPlaces()
        {
            cts = new CancellationTokenSource();
            token = cts.Token;
            t = new Task(() =>
            {
                //grab data from google\
                string req = REQ_AUTOCOMP + searchText;
                try
                {
                    request = WebRequest.Create(req);
                    response = request.GetResponse();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }

                if (token.IsCancellationRequested)
                    return;
            }, token);

            t.Start();
            t.Wait();

            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                try
                {
                    string responseText = reader.ReadToEnd();
                    all = JsonConvert.DeserializeObject<AllPlaces>(responseText);
                    Places.Clear();
                    if (all.predictions.Count > 0)
                    {
                        foreach (var pl in all.predictions)
                            Places.Add(pl.description);
                    }
                    IsDropdOpen = Convert.ToBoolean(Places.Count);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message + "\n" + e.StackTrace);
                }
            }
        }

        AppCommand selPlaceCmd;
        public AppCommand SelectPlaceCmd
        {
            get
            {
                return selPlaceCmd ?? (selPlaceCmd = new AppCommand((o)=> {
                    selectPlace = true;
                }));
            }
        }

        IPAddress ip;

        AppCommand sendDataCmd;
        public AppCommand SendDataCmd
        {
            get
            {
                return sendDataCmd ?? (sendDataCmd = new AppCommand((o) => {
                    searchText = searchText.Trim(' ');
                    //if (searchText.Length == 0)
                    //{
                    //    MessageBox.Show("No adress to search!");
                    //    return;
                    //}

                    IPHostEntry myHost = Dns.GetHostEntry(Dns.GetHostName());
                    foreach (IPAddress address in myHost.AddressList)
                    {
                        if (address.AddressFamily == AddressFamily.InterNetwork)
                            ip = address;
                    }
                    if (ip == null)
                        return;

                    try
                    {
                        IPEndPoint endPoint = new IPEndPoint(ip, 5630);
                        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        socket.Connect(endPoint);
                        byte[] buffer = Encoding.Unicode.GetBytes("Hello!");
                        socket.Send(buffer);
                    }
                    catch(Exception e)
                    {
                        MessageBox.Show(e.Message + "\n" + e.StackTrace);
                    }

                    
                    //MessageBox.Show(ip.ToString());
                }));
            }
        }
    }
}