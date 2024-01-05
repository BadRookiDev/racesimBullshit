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
        private Dictionary<Section, SectionData> _positions = new Dictionary<Section, SectionData>();

        public Race(Track track, List<IParticipant> participants)
        {   
            Track = track;
            Participants = participants;
            AssignStartingPositions();
        }

        public SectionData GetSectionData(Section section)
        {
            if (!_positions.ContainsKey(section))
            {
                _positions.Add(section, new SectionData());
            }
            return _positions[section];
        }

        public void RandomizeEquipment() {
            foreach (var participant in Participants) {
                participant.Equipment.Performance = _random.Next();
                participant.Equipment.Quality = _random.Next();
            }
        }
        
        private void AssignStartingPositions()
        {
            var participantQueue = new Queue<IParticipant>(Participants);

            var startGridNode = Track.GetLastStartGridNode();

            if (startGridNode == null)
            {
                throw new InvalidOperationException("Cannot place participants while the track has no starting grid!");
            }

            for (var node = startGridNode; ; node = node.Previous)
            {
                node ??= Track.Sections.Last;
                AssignParticipantsToSection(node.Value, participantQueue);
                if (participantQueue.Count == 0) return;
            }
        }

        private void AssignParticipantsToSection(Section section, Queue<IParticipant> participantQueue)
        {
            var sectionData = GetSectionData(section);

            if (participantQueue.Count > 0)
            {
                if (sectionData.Left != null)
                {
                    throw new InvalidOperationException("There are too many participants, the track is full.");
                }
                sectionData.Left = participantQueue.Dequeue();
            }
            if (participantQueue.Count > 0)
            {
                sectionData.Right = participantQueue.Dequeue();
            }
        }

    }
}
