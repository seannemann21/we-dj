using MusicTry3.Util;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Spotify.ApiObjectModels;

namespace MusicTry3.Models
{
    public class SpotifyTokenRefresher
    {
        Thread tokenRefresher;
        bool running;
        SpotifyCredentials credentials;

        public SpotifyTokenRefresher(SpotifyCredentials credentials)
        {
            this.credentials = credentials;
            SetRefresherThread();
        }

        public void Start()
        {
            this.running = true;
            this.tokenRefresher.Start();
        }

        public void Stop()
        {
            this.running = false;
        }

        private void SetRefresherThread()
        {
            tokenRefresher = new Thread(() =>
            {
                while (running)
                {
                    RefreshToken();
                    // 45 minutes
                    Thread.Sleep(45 * 60 * 1000);
                }
            });
        }

        private bool RefreshToken()
        {
            var successful = false;
            var client = new RestClient(Constants.Spotify.AccountsBaseApi);
            var request = new RestRequest("token", Method.POST);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", $"grant_type=refresh_token&refresh_token={credentials.refreshToken}", ParameterType.RequestBody);
            request.AddHeader("Authorization", "Basic " + CommonUtil.Base64Encode(Constants.Spotify.ClientId + ":" + Constants.Spotify.ClientSecret));
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                successful = true;
                TokenRefreshResponse refreshResponse = JsonConvert.DeserializeObject<TokenRefreshResponse>(response.Content);
                credentials.accessToken = refreshResponse.access_token;
            }

            return successful;
        }
        
    }
}