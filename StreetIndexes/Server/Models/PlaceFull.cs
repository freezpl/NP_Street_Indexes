using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    public class PlaceFull
    {
        public Result result { get; set; }
    }

    public class Result
    {
        public string name { get; set; }
        public string formatted_address { get; set; }
        public List<Photo> photos { get; set; }
        public string url { get; set; }
    }

    public class Photo
    {
        public string photo_reference { get; set; }
    }

}
