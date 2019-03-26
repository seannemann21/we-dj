using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spotify.ApiObjectModels
{
    public class Context
    {
        public string type { get; set; }
        public string href { get; set; }
        public string uri { get; set; }
    }
}