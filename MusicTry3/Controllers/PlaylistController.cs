using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicTry3.Models;
using MusicTry3.Util;

namespace MusicTry3.Controllers
{
    [Route("api/session/{sessionId}/[controller]")]
    [ApiController]
    public class PlaylistController : ControllerBase
    {

        public List<Session> sessions;

        public PlaylistController()
        {
            sessions = SessionRepo.GetSessions();
        }

        [HttpGet]
        public IActionResult Get(string sessionId)
        {
            Session session = sessions.Find(x => x.id.Equals(sessionId, StringComparison.InvariantCultureIgnoreCase));
            return session != null ? (IActionResult)Ok(session.player.GetAllPlaylists()) : NotFound();
        }

        [HttpGet("{id}")]
        public IActionResult Get(string sessionId, string id)
        {
            IPlaylist playlist = null;
            Session session = CommonUtil.GetSession(sessions, sessionId);
            if (session != null)
            {
                playlist = session.player.GetPlaylistById(id);
            }

            return playlist != null ? (IActionResult)Ok(playlist) : NotFound();
        }
        
        [HttpPost]
        public IActionResult Post(string sessionId, Name name)
        {
            IPlaylist playlist = null;
            IPlayer player = null;
            Session currentSession = CommonUtil.GetSession(sessions, sessionId);
            if (currentSession != null)
            {
                //SpotifyPlaylist spotifyPlaylist = CreateSpotifyPlaylist(currentSession.spotifyCredentials.accessToken, currentSession.spotifyUser.id, name);
                player = currentSession.player;
                playlist = player.CreateNewPlaylist(name.name);
            }

            return playlist != null ? (IActionResult)Ok(playlist) : NotFound();
        }

        [HttpPut("{id}/add")]
        public IActionResult Put(string sessionId, string id, string trackUri, string name, string artist, string submitter)
        {
            // add track to onboarding track list
            bool trackAdded = false;
            Session currentSession = CommonUtil.GetSession(sessions, sessionId);
            if (currentSession != null)
            {
                User currentUser = currentSession.GetUser(submitter);
                if (currentUser != null)
                {
                    trackAdded = currentSession.AddTrackToOnboardingList(currentUser, id, trackUri, name, artist);
                }
            }

            return trackAdded ? (IActionResult)Ok() : BadRequest();
        }

        [HttpPut("{id}/update")]
        public IActionResult Put(string sessionId, string id, string trackUri, int rating, string username)
        {
            // update track vote in onboarding track list
            bool trackUpdated = false;
            Session currentSession = CommonUtil.GetSession(sessions, sessionId);
            if (currentSession != null)
            {
                User user = currentSession.GetUser(username);
                trackUpdated = currentSession.UpdateTrackVote(user, id, trackUri, rating);

            }

            return trackUpdated ? (IActionResult)Ok() : NotFound();
        }
        
    }
}