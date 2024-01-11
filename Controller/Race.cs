using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using Model;

namespace Controller
{
    public class Race
    {
        public event EventHandler<DriversChangedEventArgs> DriversChanged;
        private int called = 0;
        
        public Track Track { get; set; }
        public List<IParticipant> Participants { get; set; }
        public DateTime StartTime { get; set; }
        
        public readonly Timer raceTimer = new Timer(Data.raceTimerMs);
        private Random _random = new Random(DateTime.Now.Millisecond);
        private Dictionary<Section, SectionData> _positions = new Dictionary<Section, SectionData>();

        public Race(Track track, List<IParticipant> participants)
        {   
            Track = track;
            Participants = participants;
            AssignStartingPositions();
            
            raceTimer.Elapsed += OnTimedEvent;
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
                participant.Equipment.Speed = _random.Next(5, 10);
                participant.Equipment.Quality = _random.Next(10, 30);
                participant.Equipment.Performance = _random.Next(4, 8);
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
        
        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            called+=1;
            const int sectionLength = 100;
            var driversChangedPosition = false;

            var tempParticipants = Participants;
            var tempCalled = called;
            foreach (var par in Participants)
            {
                if (!par.Equipment.IsBroken)
                {
                    var reactionTimeFactorRandom = _random.NextDouble() * (1.5 - 0.5) + 0.5;
                    var distanceMoved = Convert.ToInt32(
                        (par.Equipment.Performance + par.Equipment.Speed) * 3 * reactionTimeFactorRandom);
                    
                    driversChangedPosition = driversChangedPosition || UpdateParticipantPosition(par, 
                        Convert.ToInt32(distanceMoved), sectionLength);  
                }
            }

            if (!driversChangedPosition) return;
            DriversChanged?.Invoke(this, new DriversChangedEventArgs(Track));   
        }

        private bool UpdateParticipantPosition(IParticipant participant, int distanceMoved, int sectionLength)
        {
            foreach (var section in Track.Sections)
            {
                var sectionData = GetSectionData(section);

                // Check if the participant is in the current section and update distance
                var isLeft = sectionData.Left == participant;
                var isRight = sectionData.Right == participant;
                if (isLeft || isRight)
                {
                    if (isLeft) sectionData.DistanceLeft += distanceMoved;
                    if (isRight) sectionData.DistanceRight += distanceMoved;

                    // Check if the participant has completed the section
                    if ((isLeft && sectionData.DistanceLeft >= sectionLength) || 
                        (isRight && sectionData.DistanceRight >= sectionLength))
                    {
                        return MoveToNextSection(participant, section, sectionData, isLeft);
                    }
                    break;
                }
            }
            return false; // Participant has not changed sections
        }

        private bool MoveToNextSection(IParticipant participant, Section currentSection, SectionData currentSectionData, bool isLeft)
        {
            var nextSectionNode = Track.Sections.Find(currentSection)?.Next;
            if (nextSectionNode != null)
            {
                var nextSectionData = GetSectionData(nextSectionNode.Value);
                if (nextSectionData.Left == null || nextSectionData.Right == null)
                {
                    // Clear current section data
                    if (isLeft) { currentSectionData.Left = null; currentSectionData.DistanceLeft = 0; }
                    else { currentSectionData.Right = null; currentSectionData.DistanceRight = 0; }

                    // Assign to next section
                    if (nextSectionData.Left == null) nextSectionData.Left = participant;
                    else nextSectionData.Right = participant;

                    return true; // Participant has changed sections
                }
            }
            // Reset distance if participant can't move to next section
            if (isLeft) currentSectionData.DistanceLeft = 0;
            else currentSectionData.DistanceRight = 0;

            return false; // Next section is full, participant can't move
        }


        public void Start()
        {
            if (!Debugger.IsAttached)
            {
                raceTimer.Start();
                return;
            }

            while (true)
            {
                OnTimedEvent(null, null);
            }
        }

    }
}
