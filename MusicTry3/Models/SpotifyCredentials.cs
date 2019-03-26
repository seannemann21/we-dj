using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicTry3.Models
{
    public class SpotifyCredentials
    {
        public string accessToken { get; set; }
        public string refreshToken { get; set; }
        public List<string> scope { get; set; }

        public SpotifyCredentials() { }

        public SpotifyCredentials(string accessToken, string refreshToken, List<string> scope)
        {
            this.accessToken = accessToken;
            this.refreshToken = refreshToken;
            this.scope = scope;
        }
    }
}