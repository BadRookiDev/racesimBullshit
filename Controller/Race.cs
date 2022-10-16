using System;
using System.Collections.Generic;
using Model;

namespace Controller
{
    public class Race
    {
        public Track Track { get; set; }
        public List<IParticipant> Participants { get; set; }
        public DateTime StartTime { get; set; }
        private Random _random = new Random(DateTime.Now.Millisecond);
        private Dictionary<Section, SectionData> _positions;

        public Race(Track _track, List<IParticipant> _participants)
        {   
            Track = _track;
            Participants = _participants;
        }

        public SectionData GetSectionData(Section _section) {
            try
            {
                return _positions[_section];
            }
            catch (KeyNotFoundException)
            {
                _positions.Add(_section, new SectionData());
                return _positions[_section];
            }
        }

        public void RandomizeEquipment() {
            foreach (IParticipant _participant in Participants) {
                _participant.Equipment.Performance = _random.Next();
                _participant.Equipment.Quality = _random.Next();
            }
        }
    }
}
