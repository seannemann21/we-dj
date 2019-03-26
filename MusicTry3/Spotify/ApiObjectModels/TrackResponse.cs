using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spotify.ApiObjectModels
{
    public class TrackResponse
    {
        public string href { get; set; }
        public List<Track> items { get; set; }
        public long limit { get; set; }
        public string next { get; set; }
        public int offset { get; set; }
        public string previous { get; set; }
        public long total { get; set; }

    }
}