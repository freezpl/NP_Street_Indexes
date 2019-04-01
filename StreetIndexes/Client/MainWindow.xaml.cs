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

            Places = new ObservableCollection<string>();
            SearchBox.ItemsSource = Places;
            
        }

        Task t;

        void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (SearchBox.Text.Length == 0)
            //{
            //    return;
            //}
            if(SearchBox.IsEnabled)
            {
                SearchBox.IsEnabled = false;
                string reqStr = REQ_AUTOCOMP + SearchBox.Text;
                t = new Task(()=>Search(reqStr));
                t.Start();
                t.Wait();
                SearchBox.IsEnabled = true;
            }
            
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
            counter++;
            Dispatcher.Invoke(()=>{ Title = "Hi!"; });
            //MessageBox.Show("search");
            //WebRequest request = WebRequest.Create(req);
            //WebResponse response = request.GetResponse();
        }
    }
}
