using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spotify.ApiObjectModels
{
    public class TrackAdditionRequest
    {
        public List<string> uris { get; set; }
    }
}