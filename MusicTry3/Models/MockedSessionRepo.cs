using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Spotify.ApiObjectModels;

namespace MusicTry3.Models
{
    /*
    public class MockedSessionRepo : ISessionRepo
    {
        static List<Session> mockedSessions = new List<Session>();

        public List<Session> GetSessions()
        {
            
            Session session = new Session(new Credentials("accessToken", "refreshToken", new List<string>()), new SpotifyUser());
            session.id = "AAA";
            IPlaylist playlist = new MockPlaylist();
            playlist.spotifyPlaylist = new SpotifyPlaylist();
            playlist.spotifyPlaylist.id = "BBB";
            playlist.spotifyPlaylist.name = "Sean's Bangers";

            AddTrackToSpotifyPlaylist(playlist.spotifyPlaylist, "Sean Paul", "Temperature");
            AddTrackToSpotifyPlaylist(playlist.spotifyPlaylist, "Sean Kingston", "Beautiful Girls");
            AddTrackToSpotifyPlaylist(playlist.spotifyPlaylist, "Creedence Clearwater Revival", "Run Through the Jungle");
            AddTrackToSpotifyPlaylist(playlist.spotifyPlaylist, "Rage Against the Machine", "Killing in the Name");

            AddTrackToOnboardingPlaylist(playlist, "Vampire Weekend", "Unbelievers", "Sean", 5);
            AddTrackToOnboardingPlaylist(playlist, "Vampire Weekend2", "Unbelievers", "Sean", 4);
            AddTrackToOnboardingPlaylist(playlist, "Vampire Weekend3", "Unbelievers", "Sean", 5);
            AddTrackToOnboardingPlaylist(playlist, "Vampire Weekend4", "Unbelievers", "Sean", 3);
            AddTrackToOnboardingPlaylist(playlist, "Vampire Weekend5", "Unbelievers", "Sean", 4);
            AddTrackToOnboardingPlaylist(playlist, "Vampire Weekend6", "Unbelievers", "Sean", 5);
            AddTrackToOnboardingPlaylist(playlist, "Vampire Weekend7", "Unbelievers", "Sean", 5);
            AddTrackToOnboardingPlaylist(playlist, "Vampire Weekend8", "Unbelievers", "Sean", 2);


            session.users = new List<User>();
            session.users.Add(new User("Sean"));
            session.users.Add(new User("Andrew"));
            session.users.Add(new User("Curtis"));
            session.users.Add(new User("Chase"));
            session.users.Add(new User("Ryan"));
            session.users.Add(new User("Ben"));
            session.users.Add(new User("Sean"));
            session.users.Add(new User("Andrew"));
            session.users.Add(new User("Curtis"));
            session.users.Add(new User("Chase"));
            session.users.Add(new User("Ryan"));
            session.users.Add(new User("Ben"));
            session.users.Add(new User("Sean"));
            session.users.Add(new User("Andrew"));
            session.users.Add(new User("Curtis"));
            session.users.Add(new User("Chase"));
            session.users.Add(new User("Ryan"));
            session.users.Add(new User("Ben"));
            session.users.Add(new User("Sean"));
            session.users.Add(new User("Andrew"));
            session.users.Add(new User("Curtis"));
            session.users.Add(new User("Chase"));
            session.users.Add(new User("Ryan"));
            session.users.Add(new User("Ben"));

            session.users.Add(new User("Last"));

            session.playlists.Add(playlist);
            mockedSessions.Add(session);
            
            return mockedSessions;
        }

        private void AddTrackToOnboardingPlaylist(IPlaylist playlist, string artist, string name, string voter, int vote)
        {
            if(playlist.onBoardingSongs == null)
            {
                playlist.onBoardingSongs = new List<OnBoardingSong>();
            }
            
            List<Vote> newVotes = new List<Vote>();
            newVotes.Add(new Vote { user = new User(voter), score = vote });
            playlist.onBoardingSongs.Add(new OnBoardingSong(artist, name, "1234", false, new User("Sean")));

        }

        private void AddTrackToSpotifyPlaylist(SpotifyPlaylist spotifyPlaylist, string artist, string name)
        {
            if (spotifyPlaylist.tracks == null)
            {
                spotifyPlaylist.tracks = new PlaylistTrackResponse();
            }

            if(spotifyPlaylist.tracks.items == null)
            {
                spotifyPlaylist.tracks.items = new List<TrackWrapper>();
            }

            TrackWrapper wrapper = new TrackWrapper();
            wrapper.track = new Track();
            Artist artistObj = new Artist();
            artistObj.name = artist;
            wrapper.track.artists = new List<Artist>();
            wrapper.track.artists.Add(artistObj);
            wrapper.track.name = name;
            spotifyPlaylist.tracks.items.Add(wrapper);
        }

    }
    */
}