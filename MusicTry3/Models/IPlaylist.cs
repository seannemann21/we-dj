using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spotify.ApiObjectModels;

namespace MusicTry3.Models
{
    public interface IPlaylist
    {
        List<OnBoardingSong> onBoardingSongs { get; set; }
        List<Song> playlist { get; set; }
        string name { get; set; }
        string id { get; set; }
        int GetNumberOfTracks();
        bool HasNextSong();
        string GetNextTrackUri();
        void NextSongPlayed();
        bool AddToOnboardingSongs(OnBoardingSong onBoardingSong);
        bool UpdateTrackVote(User user, string trackUri, int rating);
    }
}
