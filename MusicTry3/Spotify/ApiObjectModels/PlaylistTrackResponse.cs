using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spotify.ApiObjectModels
{
    public class PlaylistTrackResponse
    {
        public string href { get; set; }
        public List<TrackWrapper> items { get; set; }
        public long limit { get; set; }
        public string next { get; set; }
        public int offset { get; set; }
        public string previous { get; set; }
        public long total { get; set; }

        public List<Track> GetSpotifyTracks()
        {
            return items.ConvertAll<Track>(trackWrapper => trackWrapper.track);
        }

    }
}