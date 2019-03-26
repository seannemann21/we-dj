using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicTry3.Models;
using MusicTry3.Util;
using Newtonsoft.Json;
using RestSharp;
using Spotify.ApiObjectModels;

namespace MusicTry3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpotifyController : ControllerBase
    {

        public List<Session> sessions;

        public SpotifyController()
        {
            sessions = SessionRepo.GetSessions();
        }


        [Route("exportPlaylist")]
        public IActionResult ExportPlaylist(string sessionId, string playlistId)
        {
            bool playlistExported = false;
            Session session = CommonUtil.GetSession(sessions, sessionId);
            if (session != null)
            {
                IPlaylist playlist = session.player.GetPlaylistById(playlistId);
                if (playlist != null)
                {
                    string accessToken = ((SpotifyPlayer)session.player).credentials.accessToken;
                    SpotifyUser spotifyUser = GetCurrentSpotifyUser(accessToken);
                    SpotifyPlaylist spotifyPlaylist = CreateSpotifyPlaylist(accessToken, spotifyUser.id, playlist.name);
                    foreach (Song song in playlist.playlist)
                    {
                        AddTrackToPlaylist(spotifyPlaylist.id, accessToken, song.trackUri);
                    }
                    playlistExported = true;
                }

            }

            return playlistExported ? (IActionResult)Ok() : NotFound();
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

        private SpotifyUser GetCurrentSpotifyUser(string authorizationToken)
        {
            var client = new RestClient(Constants.Spotify.WebApiBase);
            var request = new RestRequest("me");
            request.AddHeader("Authorization", "Bearer " + authorizationToken);
            IRestResponse response = client.Execute(request);
            SpotifyUser currentUser = null;
            if (response.IsSuccessful)
            {
                string content = response.Content;
                currentUser = JsonConvert.DeserializeObject<SpotifyUser>(content);
            }
            return currentUser;
        }

        private SpotifyPlaylist CreateSpotifyPlaylist(string authorizationToken, string userId, string name)
        {
            var client = new RestClient(Constants.Spotify.WebApiBase + "users/" + userId + "/");
            var request = new RestRequest("playlists", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Authorization", "Bearer " + authorizationToken);
            request.AddHeader("Content-type", "application/json");
            request.AddJsonBody(new PlaylistRequestBody { name = name });
            IRestResponse response = client.Execute(request);
            SpotifyPlaylist playlistResponse = null;
            if (response.IsSuccessful)
            {
                playlistResponse = JsonConvert.DeserializeObject<SpotifyPlaylist>(response.Content, new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore
                });
            }
            return playlistResponse;
        }

        [Route("search")]
        public IActionResult Search(string query, string sessionId)
        {
            List<Track> restResponse = null;
            Session session = CommonUtil.GetSession(sessions, sessionId);
            if (session != null)
            {
                restResponse = SearchSpotify(query, ((SpotifyPlayer)session.player).credentials.accessToken);
            }
            return restResponse != null ? (IActionResult)Ok(restResponse) : NotFound();
        }

        private List<Track> SearchSpotify(string query, string accessToken)
        {
            var client = new RestClient(Constants.Spotify.WebApiBase);
            var request = new RestRequest("search", Method.GET);
            request.AddHeader("Authorization", "Bearer " + accessToken);
            request.AddParameter("q", query);
            request.AddParameter("type", "track");
            IRestResponse response = client.Execute(request);
            TrackResponse trackResponse = new TrackResponse();
            if (response.IsSuccessful)
            {
                Dictionary<String, TrackResponse> searchResponse = JsonConvert.DeserializeObject<Dictionary<String, TrackResponse>>(response.Content);
                if (searchResponse.ContainsKey("tracks"))
                {
                    trackResponse = searchResponse["tracks"];
                }
            }
            if (trackResponse.items == null)
            {
                trackResponse.items = new List<Track>();
            }
            return trackResponse.items;
        }
    }
}