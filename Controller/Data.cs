using System;
using System.Collections.Generic;
using Model;
namespace Controller
{
    
    public static class Data
    {
        public static Competition Competition { get; set; }
        public static Race CurrentRace { get; set; }

        public static void Initialize(){
            Competition = new Competition();
            AddParticipants();
            AddTracks();
        }

        public static void AddParticipants()
        { 
            List<IParticipant> list = new List<IParticipant> {
                new Driver("Razor",0, new Car(1, 1200, 300, false) ,TeamColor.Blue),
                new Driver("Bull", 0, new Car(1, 1200, 300, false),TeamColor.Green),
                new Driver("Ronnie", 0, new Car(1, 1200, 300, false),TeamColor.Yellow)
            };
            Competition.Participants = list;
        }

        public static void AddTracks()
        {
            SectionTypes[] sectionsTrack1 = {
                SectionTypes.StartGrid,
                SectionTypes.Finish,
                SectionTypes.RightCorner,
                SectionTypes.Straight,
                SectionTypes.RightCorner,
                SectionTypes.Straight,
                SectionTypes.Straight,
                SectionTypes.RightCorner,
                SectionTypes.Straight,
                SectionTypes.RightCorner
            };

            SectionTypes[] sectionsTrack2 = {
                SectionTypes.StartGrid,
                SectionTypes.Straight,
                SectionTypes.LeftCorner,
                SectionTypes.LeftCorner,
                SectionTypes.Straight,
                SectionTypes.RightCorner,
                SectionTypes.RightCorner,
                SectionTypes.Straight,
                SectionTypes.Finish
            };

            var track1 = new Track("Discovery Track", sectionsTrack1);
            var track2 = new Track("Resort Circuit", sectionsTrack2);

            Queue<Track> tracks = new Queue<Track>();
            tracks.Enqueue(track1);
            tracks.Enqueue(track2);

            Competition.Tracks = tracks;
        }

        public static void NextRace() {
            Track track = Competition.NextTrack();
            if (track != null) {
                CurrentRace = new Race(track, Competition.Participants);
            }
        }


    }
}
