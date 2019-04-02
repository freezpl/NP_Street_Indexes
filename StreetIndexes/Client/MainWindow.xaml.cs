using Client.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<string> Places { get; set; }
        const string REQ_AUTOCOMP = @"https://maps.googleapis.com/maps/api/place/autocomplete/json?key=AIzaSyCRuMrU5eZqvCXaxRg8FOacV9iea6SBEkI&input=";

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MVVM();


            Places = new ObservableCollection<string>();
            SearchBox.ItemsSource = Places;

            //Task tt = new Task(() =>
            //{
            //    App.Current.Dispatcher.Invoke(() => { Wind.Title = "Hi!"; });
            //    Thread.Sleep(1000);
            //});
            //tt.Start();
            //tt.Wait();
            
            //MessageBox.Show(tt.Status.ToString());
            
        }

        Task t;
        CancellationTokenSource cts;

        async void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (SearchBox.Text.Length == 0)
            //{
            //    return;
            //}

            //string reqStr = REQ_AUTOCOMP + SearchBox.Text;
            //t = new Task(()=>Search(reqStr));
            //t.Start();
            //t.Wait();
            //cts = new CancellationTokenSource();
            //await Task.Run(() => Search(reqStr), cts.Token);

            //using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            //{
            //    string responseText = reader.ReadToEnd();

            //    AllPlaces all = JsonConvert.DeserializeObject<AllPlaces>(responseText);

            //    Places.Clear();
            //    if (all.predictions.Count > 0)
            //    {

            //        foreach (var pl in all.predictions)
            //            Places.Add(pl.description);
            //        SearchBox.IsDropDownOpen = true;
            //    }
            //    else
            //        SearchBox.IsDropDownOpen = false;
            //}
        }

        int counter = 0;
        void Search(string req)
        {
            if (cts.Token.IsCancellationRequested)
            {
                return;
            }
            counter++;
            Thread.Sleep(1000);
            App.Current.Dispatcher.Invoke(() => { Wind.Title = counter.ToString(); });

            //MessageBox.Show("search");
            //WebRequest request = WebRequest.Create(req);
            //WebResponse response = request.GetResponse();
        }
    }
}
