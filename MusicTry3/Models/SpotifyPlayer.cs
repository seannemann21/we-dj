using log4net;
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
    public class SpotifyPlayer : IPlayer
    {

        private static readonly ILog logger = LogManager.GetLogger("SpotifyApp", "Playlist");
        public List<IPlaylist> playlists { get; set; }
        public IPlaylist currentPlaylist { get; set; }
        [JsonIgnore]
        public Thread connectPlayback { get; set; }
        public bool playingThroughConnectAPI { get; set; }
        public string deviceId { get; set; }
        public SpotifyCredentials credentials { get; set; }
        public bool isPaused { get; set; }
        public bool alive { get; set; }
        public SpotifyTokenRefresher tokenRefresher { get; set; }

        public SpotifyPlayer(SpotifyCredentials credentials)
        {
            this.credentials = credentials;
            this.playingThroughConnectAPI = false;
            this.isPaused = false;
            this.playlists = new List<IPlaylist>();
            connectPlayback = new Thread(() =>
            {
                while (alive)
                {
                    if(playingThroughConnectAPI)
                    {
                        PlaybackContext playbackContext;
                        if (credentials != null)
                        {
                            playbackContext = GetPlaybackContext(credentials.accessToken);
                            if(playbackContext != null && this.isPaused)
                            {
                                logger.Info("Resuming playback since user unpaused");
                                this.isPaused = false;
                                ResumePlayback();
                            } else if (playbackContext == null || !playbackContext.is_playing)
                            {
                                logger.Info("Starting playback or last song finished and playing next");
                                // check to make sure there are enough songs
                                if(currentPlaylist.HasNextSong())
                                {
                                    PlayNextSong();
                                }
                            }
                        }
                    }
                    Thread.Sleep(2000);
                }
            });
            alive = true;
            connectPlayback.Start();
            this.tokenRefresher = new SpotifyTokenRefresher(credentials);
            this.tokenRefresher.Start();
        }

        public void Next()
        {
            PausePlayback();
        }

        public void Pause()
        {
            this.playingThroughConnectAPI = false;
            this.isPaused = true;
            PausePlayback();
        }

        public void Play(string deviceId)
        {
            this.playingThroughConnectAPI = true;
            this.deviceId = deviceId;

        }

        private bool PausePlayback()
        {
            var successful = false;
            var client = new RestClient(Constants.Spotify.WebApiBase + "me/");
            var request = new RestRequest("player/pause", Method.PUT);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Authorization", "Bearer " + this.credentials.accessToken);

            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                successful = true;
            }

            return successful;
        }

        private bool ResumePlayback()
        {
            var successful = false;
            var client = new RestClient(Constants.Spotify.WebApiBase + "me/");
            var request = new RestRequest("player/play", Method.PUT);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Authorization", "Bearer " + this.credentials.accessToken);

            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                successful = true;
            }

            return successful;
        }

        private void PlayNextSong()
        {
            //UpdateSpotifyPlaylist();
            if(currentPlaylist.HasNextSong())
            {
                var uriToPlay = currentPlaylist.GetNextTrackUri();
                var client = new RestClient(Constants.Spotify.WebApiBase + "me/");
                var request = new RestRequest("player/play?device_id=" + deviceId, Method.PUT);
                //var request = new RestRequest("player/play?device_id=" + deviceId, Method.PUT);
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("Authorization", "Bearer " + this.credentials.accessToken);
                StartResumeRequestBody body = new StartResumeRequestBody();
                body.uris = new List<string>();
                body.uris.Add(uriToPlay);
                request.AddBody(body);

                IRestResponse response = client.Execute(request);
                if (response.IsSuccessful)
                {
                    currentPlaylist.NextSongPlayed();
                }
            }
        }

        private void Resume()
        {

        }

        


        private void AddTrackToPlaylist(string playlistId, string authorizationToken, string trackUri)
        {
            var client = new RestClient(Constants.Spotify.WebApiBase + "playlists/" + playlistId + "/");
            var request = new RestRequest("tracks", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Authorization", "Bearer " + authorizationToken);
            request.AddHeader("Content-type", "application/json");
            TrackAdditionRequest trackAddition = new TrackAdditionRequest();
            trackAddition.uris = new List<string>();
            trackAddition.uris.Add(trackUri);
            request.AddJsonBody(trackAddition);
            IRestResponse response = client.Execute(request);
        }

        private PlaybackContext GetPlaybackContext(string authorizationToken)
        {
            var client = new RestClient(Constants.Spotify.WebApiBase + "me/");
            var request = new RestRequest("player", Method.GET);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Authorization", "Bearer " + authorizationToken);
            IRestResponse response = client.Execute(request);

            return JsonConvert.DeserializeObject<PlaybackContext>(response.Content);
        }
        
        public IPlaylist CreateNewPlaylist(string name)
        {
            IPlaylist newPlaylist = new Playlist(name);
            playlists.Add(newPlaylist);
            currentPlaylist = newPlaylist;
            return newPlaylist;
        }

        public bool LoadPlaylist(string playlistId)
        {
            bool playlistLoaded = false;
            IPlaylist playlistToLoad = playlists.Find(x => x.id == playlistId);
            if(playlistToLoad != null)
            {
                currentPlaylist = playlistToLoad;
                playlistLoaded = true;
            }

            return playlistLoaded;
        }

        public IPlaylist GetPlaylistById(string id)
        {
            return playlists.Find(x => x.id == id);
        }

        public List<IPlaylist> GetAllPlaylists()
        {
            return playlists;
        }

        public bool AddToOnboardingSongs(string playlistId, OnBoardingSong onBoardingSong)
        {
            bool songAdded = false;
            IPlaylist playlist = GetPlaylistById(playlistId);
            if(playlist != null)
            {
                songAdded = playlist.AddToOnboardingSongs(onBoardingSong);
            }
            return songAdded;
        }

        public bool UpdateTrackVote(User user, string playlistId, string trackUri, int rating)
        {
            bool trackUpdated = false;
            IPlaylist playlist = GetPlaylistById(playlistId);
            if (playlist != null)
            {
                trackUpdated = playlist.UpdateTrackVote(user, trackUri, rating);
            }
            return trackUpdated;
        }

        public void Stop()
        {
            alive = false;
            tokenRefresher.Stop();
        }
    }
}