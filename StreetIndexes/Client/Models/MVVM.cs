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
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;

namespace Client.Models
{
    public class MVVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<Place> Places { get; set; }
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
            Places = new ObservableCollection<Place>();
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
                            Places.Add(pl);
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
                return selPlaceCmd ?? (selPlaceCmd = new AppCommand((o) =>
                {
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
                return sendDataCmd ?? (sendDataCmd = new AppCommand((o) =>
                {
                    searchText = searchText.Trim(' ');
                    if (searchText.Length == 0)
                    {
                        MessageBox.Show("Empty field");
                        return;
                    }

                    Place place = Places.Where(p => p.description == searchText).FirstOrDefault();
                    if (place == null)
                    {
                        MessageBox.Show("This place not from our planet :-)");
                        return;
                    }

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
                        byte[] buffer = Encoding.Unicode.GetBytes(place.place_id);
                        
                        socket.Send(buffer);

                        StringBuilder fullPlaseStr = new StringBuilder();
                        byte[] respBuffer = new byte[256];
                        int bytes = 0;
                        do
                        {
                            bytes = socket.Receive(respBuffer, respBuffer.Length, 0);
                            fullPlaseStr.Append(Encoding.Unicode.GetString(respBuffer, 0, bytes));
                        }
                        while (socket.Available > 0);
                        socket.Shutdown(SocketShutdown.Both);
                        socket.Close();
                        LoadFullInfo(fullPlaseStr.ToString());
                    }
                    catch (Exception e)
                {
                    MessageBox.Show("Somthing went wrong with load image.");
                }
            }));
            }
        }

        PlaceFull placeFull;

        void LoadFullInfo(string str)
        {
            placeFull = JsonConvert.DeserializeObject<PlaceFull>(str);
            if (placeFull.result.photos != null && placeFull.result.photos.Count > 0)
            {
                LoadPhotoURL(placeFull.result.photos[0]);
                current = 0;
            }
        }
        private BitmapImage img;
        public BitmapImage Img
        {
            get { return img; }
            set
            {
                img = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Img)));
            }
        }

        void LoadPhotoURL(Photo photo)
        {
            string queryStr = @"https://maps.googleapis.com/maps/api/place/photo?maxwidth=800&key=AIzaSyCRuMrU5eZqvCXaxRg8FOacV9iea6SBEkI&photoreference=" + photo.photo_reference;

            WebRequest request = WebRequest.Create(queryStr);
            WebResponse response = request.GetResponse();
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = response.GetResponseStream();
            bitmapImage.EndInit();
            Img = bitmapImage;
        }

        int current = 0;
        AppCommand nextCmd;
        public AppCommand NextCmd
        {
            get
            {
                return nextCmd ?? (nextCmd = new AppCommand((o) =>
                {
                    if (placeFull == null || placeFull.result.photos == null || placeFull.result.photos.Count == 0)
                        return;

                    if (Convert.ToBoolean(o))
                    {
                        current++;
                        if (current >= placeFull.result.photos.Count)
                            current = 0;
                    }
                    else
                    {
                        current--;
                        if (current <= 0)
                            current = placeFull.result.photos.Count - 1;
                    }
                    LoadPhotoURL(placeFull.result.photos[current]);
                }));
            }
        }
    }

}