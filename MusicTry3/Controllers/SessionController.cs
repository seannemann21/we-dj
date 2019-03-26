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
    [Route("api/session")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        public List<Session> sessions;
        string grantType = "authorization_code";

        public SessionController()
        {
            sessions = SessionRepo.GetSessions();
        }

        [Route("redirect-uri")]
        public IActionResult GetRedirectURI()
        {
            return Ok(Constants.Spotify.RedirectUri);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(sessions);
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            Session session = sessions.Find(x => x.id.Equals(id, StringComparison.InvariantCultureIgnoreCase));
            return session != null ? (IActionResult)Ok(session) : NotFound();
        }

        [HttpPost]
        public IActionResult Create(string code)
        {
            Session session = null;
            var client = new RestClient(Constants.Spotify.AccountsBaseApi);
            var request = new RestRequest("token", Method.POST);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", $"client_id={Constants.Spotify.ClientId}&client_secret={Constants.Spotify.ClientSecret}&grant_type={grantType}&code={code}&redirect_uri={Constants.Spotify.RedirectUri}", ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                SpotifyTokenResponse responseBody = JsonConvert.DeserializeObject<SpotifyTokenResponse>(response.Content);
                if (responseBody.scope != null && responseBody.access_token != null)
                {
                    SpotifyCredentials credentials = new SpotifyCredentials(responseBody.access_token, responseBody.refresh_token, new List<string>(responseBody.scope.Split(' ')));
                    IPlayer player = new SpotifyPlayer(credentials);
                    session = new Session(player);
                    sessions.Add(session);
                }
            }

            return session != null ? (IActionResult)Ok(session) : NotFound();
        }

        [Route("keepalive")]
        public IActionResult keepalive(string sessionId, string keepAlive)
        {
            bool keepAliveUpdated = false;
            Session session = CommonUtil.GetSession(sessions, sessionId);
            if (session != null)
            {
                keepAliveUpdated = session.UpdateKeepAlive(keepAlive);
            }
            return keepAliveUpdated ? (IActionResult)Ok() : NotFound();
        }

        [Route("createuser")]
        public IActionResult Createuser(string username, string sessionId)
        {
            bool userCreated = false;
            Session session = CommonUtil.GetSession(sessions, sessionId);
            if (session != null)
            {
                userCreated = session.AddUser(username);
            }

            IActionResult result;
            if (userCreated)
            {
                result = Ok();
            }
            else if (session == null)
            {
                result = NotFound();
            }
            else
            {
                result = Conflict();
            }

            return result;
        }

    }
}