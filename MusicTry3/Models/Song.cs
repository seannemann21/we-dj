using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicTry3.Models
{
    public class Song
    {
        public string name { get; set; }
        public string artist { get; set; }
        public string trackUri { get; set; }

        public Song(string name, string artist, string trackUri)
        {
            this.name = name;
            this.artist = artist;
            this.trackUri = trackUri;
        }

    }
}