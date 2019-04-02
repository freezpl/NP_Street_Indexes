using Client.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Models
{
    public class MVVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        //commands
        AppCommand typeCmd;
        public AppCommand TypeCmd
        {
            get
            {
                return typeCmd ?? (typeCmd = new AppCommand((o) =>
                {
                    MessageBox.Show("ccc");
                }));
            }
        }


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
                MessageBox.Show(searchText);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchText)));
            }
        }

        public MVVM()
        {

        }

    }
}
