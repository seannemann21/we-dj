using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicTry3.Constants
{
    public static class Spotify
    {
        public static readonly string ClientId = Environment.GetEnvironmentVariable("WE_DJ_CLIENT_ID");
        public static readonly string ClientSecret = Environment.GetEnvironmentVariable("WE_DJ_CLIENT_SECRET");
        public const string WebApiBase = "https://api.spotify.com/v1/";
        public const string AccountsBaseApi = "https://accounts.spotify.com/api/";
        public static readonly string RedirectUri = Environment.GetEnvironmentVariable("WE_DJ_REDIRECT_URI");

    }
}