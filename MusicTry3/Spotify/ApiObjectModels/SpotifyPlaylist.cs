using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spotify.ApiObjectModels
{
    public class SpotifyPlaylist
    {
        public string id { get; set; }
        public string name { get; set; }
        public PlaylistTrackResponse tracks { get; set; }
        public string uri { get; set; }
    }
}