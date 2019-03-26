using Spotify.ApiObjectModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace MusicTry3.Models
{
    public class Playlist : IPlaylist
    {
        //string spotifyBaseApi = "https://api.spotify.com/v1/";
        //public SpotifyPlaylist spotifyPlaylist { get; set; }
        public List<OnBoardingSong> onBoardingSongs { get; set; }
        public List<Song> playlist { get; set; }
        public Thread onBoardingSelector { get; set; }
        public bool active { get; set; }
        public int nextSongOffset { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public Playlist(string name)
        {
            this.name = name;
            nextSongOffset = 0;
            onBoardingSongs = new List<OnBoardingSong>();
            playlist = new List<Song>();
            id = Guid.NewGuid().ToString();
        }

        // returns true if there are more unplayedSongs in queue than bufferSongs, false if equal to or less than buffer songs or the current song played isn't in playlist
        private bool AtLeastThisManySongsInPlaylistQueue(int bufferSongs)
        {
            return playlist.Count >= (nextSongOffset + bufferSongs);
        }

        private OnBoardingSong SelectNextSong()
        {
            OnBoardingSong nextSong = onBoardingSongs.Find(x => x.priority == true);
            if (nextSong == null)
            {
                nextSong = GetHighestVotedSong(onBoardingSongs);
                if (nextSong != null)
                {
                    nextSong.submitter.SongSelected();
                }
            }

            if (nextSong != null)
            {
                onBoardingSongs.Remove(nextSong);
            }

            return nextSong;
        }

        private OnBoardingSong GetHighestVotedSong(List<OnBoardingSong> onBoardingSongs)
        {
            // update currently just first
            OnBoardingSong highestVotedSong = null;
            if (onBoardingSongs != null && onBoardingSongs.Count > 0)
            {
                // should make onboardingsongs a priority queue
                highestVotedSong = onBoardingSongs.OrderByDescending(x => x.GetAverageRating()).ToList()[0];
            }

            return highestVotedSong;
        }

        public int GetNumberOfTracks()
        {
            return playlist.Count;
        }

        public bool HasNextSong()
        {
            return nextSongOffset < GetNumberOfTracks();
        }

        public void NextSongPlayed()
        {
            nextSongOffset++;
            AddTrackToPlaylist();
        }

        private void AddTrackToPlaylist()
        {
            if(!AtLeastThisManySongsInPlaylistQueue(1))
            {
                var nextTrack = SelectNextSong();
                if (nextTrack != null)
                {
                    playlist.Add(nextTrack.song);
                }
            }
        }

        public string GetNextTrackUri()
        {
            return playlist[nextSongOffset].trackUri;
        }

        public bool AddToOnboardingSongs(OnBoardingSong onBoardingSong)
        {
            bool songAdded = false;
            // probably should've used some sort of ordered set
            if (null == onBoardingSongs.Find(x => (x.song.name == onBoardingSong.song.name && x.song.artist == onBoardingSong.song.artist))) {
                onBoardingSongs.Add(onBoardingSong);
                AddTrackToPlaylist();
                songAdded = true;
            }
            return songAdded;
        }

        public bool UpdateTrackVote(User user, string trackUri, int rating)
        {
            bool trackUpdated = false;
            OnBoardingSong track = onBoardingSongs.Find(x => x.song.trackUri == trackUri);
            if (track != null && user != null)
            {
                // username is unique id
                Vote vote = track.votes.Find(x => x.user.name == user.name);
                if (vote == null)
                {
                    vote = new Vote();
                    vote.user = user;
                    track.votes.Add(vote);
                }
            
                vote.score = rating;
                trackUpdated = true;
            }

            return trackUpdated;
        }
    }
}