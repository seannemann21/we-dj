using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicTry3.Models
{
    public class User
    {
        public string name { get; set; }
        public int totalSongsSelected { get; set; }
        public int songsUntilFreePick { get; set; }
        public bool readyForFreePick { get; set;  }
        public static readonly int SONGS_NEEDED_FOR_FREE_PICK = 2;

        public User(String name)
        {
            this.name = name;
            this.totalSongsSelected = 0;
            this.songsUntilFreePick = SONGS_NEEDED_FOR_FREE_PICK;
            this.readyForFreePick = false;
        }

        public void SongSelected()
        {
            totalSongsSelected++;
            songsUntilFreePick--;
            if(songsUntilFreePick == 0)
            {
                readyForFreePick = true;
            }
        }

        public void freePickTaken()
        {
            songsUntilFreePick = SONGS_NEEDED_FOR_FREE_PICK;
            readyForFreePick = false;
        }
    }
}