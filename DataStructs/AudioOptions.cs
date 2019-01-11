using Discord;
using Victoria.Entities;

namespace SuccBot_master.DataStructs
{
    public class AudioOptions
    {
        public bool Shuffle { get; set; }
        public bool RepeatTrack { get; set; }
        public IUser Summoner { get; set; }
    }
}