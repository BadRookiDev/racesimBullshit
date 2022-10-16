using System;
using System.Collections.Generic;

namespace Model
{
    public class Competition
    {

        public List<IParticipant> Participants { get; set; }
        public Queue<Track> Tracks { get; set; } = new Queue<Track>();

        public Competition()
        {
            
        }

        public Track NextTrack()
        {
            if (Tracks.Count > 0)
            {
                return Tracks.Dequeue();
            }
            return null;
        }
    }
}
