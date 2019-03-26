using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spotify.ApiObjectModels
{
    public class Track
    {
        public List<Artist> artists { get; set; }
        public string duration_ms { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string uri { get; set; }
    }
}