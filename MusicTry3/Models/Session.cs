using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Spotify.ApiObjectModels;

namespace MusicTry3.Models
{
    public class Session
    {

        private static HashSet<int> existingSessionIds = new HashSet<int>();
        private static Random random = new Random();

        private static readonly int charsInId = 3;
        public static readonly int maxId = 17575;
        public IPlayer player { get; set; }
        public String id { get; set; }
        public List<User> users { get; set; }
        public DateTime lastContactWithMaster { get; set; }
        public String keepAliveToken { get; set; }

        public Session(IPlayer player)
        {
            this.id = GenerateId(existingSessionIds, random);
            this.player = player;
            this.users = new List<User>();
            this.lastContactWithMaster = DateTime.UtcNow;
            this.keepAliveToken = Guid.NewGuid().ToString();
        }

        public void Stop()
        {
            player.Stop();
        }

        public static string GenerateId(HashSet<int> existingIds, Random rand)
        {
            if (existingIds.Count == (maxId + 1))
            {
                return "error";
            }
            int idAsNumber = rand.Next(maxId);
            while (existingIds.Contains(idAsNumber))
            {
                idAsNumber = (idAsNumber + 1) % maxId;
            }
            existingIds.Add(idAsNumber);
            string id = "";
            for(int i = 0; i < charsInId; i++)
            {
                var nextLetter = (char)((idAsNumber % 26) + 'A');
                id += nextLetter;
                idAsNumber = idAsNumber / 26;
            }

            return id;
        }

        public bool UpdateKeepAlive(string keepAlive)
        {
            bool keepAliveUpdated = false;
            if (keepAliveToken == keepAlive)
            {
                lastContactWithMaster = DateTime.UtcNow;
                keepAliveUpdated = true;
            }

            return keepAliveUpdated;
        }

        public bool AddUser(string username)
        {
            bool userCreated = false;
            if (!this.users.Exists(x => x.name == username))
            {
                this.users.Add(new User(username));
                userCreated = true;
            }

            return userCreated;
        }

        public User GetUser(string username)
        {
            return users.Find(x => x.name == username);
        }

        private static int Power(int a, int b)
        {
            int result = 1;
            for(int i = 0; i < b; i++)
            {
                result *= a;
            }

            return result;
        }

        public bool AddTrackToOnboardingList(User submitter, string playlistId, string trackUri, string name, string artist)
        {
            bool trackAdded = false;
            if(submitter != null)
            {
                bool priority = submitter.readyForFreePick;
                trackAdded = player.AddToOnboardingSongs(playlistId, new OnBoardingSong(artist, name, trackUri, priority, submitter));
                if(priority)
                {
                    submitter.freePickTaken();
                }
            }

            return trackAdded;
        }



        public bool UpdateTrackVote(User user, string playlistId, string trackUri, int rating)
        {

            return player.UpdateTrackVote(user, playlistId, trackUri, rating);
        }
    }
}