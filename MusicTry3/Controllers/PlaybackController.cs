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
    [Route("api/session/{sessionId}/playlist/{playlistId}/")]
    [ApiController]
    public class PlaybackController : ControllerBase
    {

        public List<Session> sessions;

        public PlaybackController()
        {
            sessions = SessionRepo.GetSessions();
        }
        
        [Route("play")]
        public IActionResult Play(string sessionId, string playlistId, string deviceId)
        {
            Session session = CommonUtil.GetSession(sessions, sessionId);
            if (session != null)
            {
                session.player.Play(deviceId);
            }

            return Ok();
        }
        
        [Route("pause")]
        public IActionResult Pause(string sessionId, string playlistId)
        {
            Session session = CommonUtil.GetSession(sessions, sessionId);
            if (session != null)
            {
                session.player.Pause();
            }

            return Ok();
        }
        
        [Route("next")]
        public IActionResult Next(string sessionId, string playlistId)
        {
            Session session = CommonUtil.GetSession(sessions, sessionId);
            if (session != null)
            {
                session.player.Next();
            }

            return Ok();
        }

        [Route("load")]
        public IActionResult Play(string sessionId, string playlistId)
        {
            bool playlistLoaded = false;
            Session session = CommonUtil.GetSession(sessions, sessionId);
            if (session != null)
            {
                playlistLoaded = session.player.LoadPlaylist(playlistId);
            }

            return playlistLoaded ? (IActionResult)Ok() : NotFound();
        }

    }
}