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
            List<IParticipant> _list = new List<IParticipant> {
                new Driver("Razor",0, new Car(1, 1200, 300, false) ,TeamColor.Blue),
                new Driver("Bull", 0, new Car(1, 1200, 300, false),TeamColor.Grey),
                new Driver("Ronnie", 0, new Car(1, 1200, 300, false),TeamColor.Red)
            };
            Competition.Participants = _list;
        }

        public static void AddTracks()
        {
            SectionTypes[] SectionsA = new SectionTypes[] {
                SectionTypes.StartGrid,
                SectionTypes.Straight,
                SectionTypes.LeftCorner,
                SectionTypes.RightCorner,
                SectionTypes.Straight,
                SectionTypes.Finish
            };

            SectionTypes[] SectionsB = new SectionTypes[] {
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

            Track _a = new Track("T-1", SectionsA);
            Track _b = new Track("T-2", SectionsB);

            Queue<Track> tracks = new Queue<Track>();
            tracks.Enqueue(_a);
            tracks.Enqueue(_b);

            Competition.Tracks = tracks;
        }

        public static void NextRace() {
            Track _track = Competition.NextTrack();
            if (_track != null) {
                CurrentRace = new Race(_track, Competition.Participants);
            }
        }


    }
}
