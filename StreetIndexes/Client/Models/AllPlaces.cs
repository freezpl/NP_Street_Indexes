using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class AllPlaces
    {
        public List<Place> predictions { get; set; }

        public AllPlaces()
        {
            predictions = new List<Place>();
        }
    }
}
