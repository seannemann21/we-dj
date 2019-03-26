using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicTry3.Models
{
    public class OnBoardingSong
    {
        public Song song { get; set; }
        public bool priority { get; set; }
        public User submitter { get; set; }
        public List<Vote> votes { get; set; }

        public OnBoardingSong(string name, string artist, string trackUri, bool priority, User submitter)
        {
            song = new Song(name, artist, trackUri);
            this.priority = priority;
            this.votes = new List<Vote>();
            this.submitter = submitter;
        }

        public double GetAverageRating()
        {
            double totalRating = votes.Sum(x => x.score);
            return totalRating / votes.Count;
        }

    }
}