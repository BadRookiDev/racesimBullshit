using System;

namespace Model
{
    public class DriversChangedEventArgs : EventArgs
    {
        public Track Track { get; private set; }

        public DriversChangedEventArgs(Track track)
        {
            Track = track;
        }
    }
}