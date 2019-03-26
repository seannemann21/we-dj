using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicTry3.Models
{
    public interface IPlayer
    {
        string deviceId { get; set; }
        bool isPaused { get; set; }

        void Stop();
        void Next();
        void Pause();
        void Play(String deviceId);
        IPlaylist CreateNewPlaylist(string name);
        IPlaylist GetPlaylistById(string id);
        List<IPlaylist> GetAllPlaylists();
        bool AddToOnboardingSongs(string playlistId, OnBoardingSong onBoardingSong);
        bool UpdateTrackVote(User user, string playlistId, string trackUri, int rating);
        bool LoadPlaylist(string playlistId);
    }
}
