using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicTry3.Models
{
    public class Vote
    {
        public User user { get; set; }
        public int score { get; set; }
    }
}