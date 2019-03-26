using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicTry3.Constants
{
    public static class Spotify
    {
        public const string ClientId = "fffa7e259c734e9d9b681b1fbf07f2f9";
        public static readonly string ClientSecret = Environment.GetEnvironmentVariable("WE_DJ_CLIENT_SECRET");
        public const string WebApiBase = "https://api.spotify.com/v1/";
        public const string AccountsBaseApi = "https://accounts.spotify.com/api/";
        public static readonly string RedirectUri = Environment.GetEnvironmentVariable("WE_DJ_REDIRECT_URI");

    }
}