using System;
using System.Collections.Generic;

namespace Model
{
    public class Track
    {
        public string Name { get; set; }
        public LinkedList<Section> Sections { get; set; }

        public Track(string name, SectionTypes[] sections)
        {
            Name = name;
            Sections = ConvertSectionTypesToSections(sections);
        }
        
        private LinkedList<Section> ConvertSectionTypesToSections(SectionTypes[] sections)
        {
            LinkedList<Section> realSections = new LinkedList<Section>();

            for (int i = sections.Length-1; i >= 0; i--)
            {
                realSections.AddFirst(new Section(sections[i]));
            }

            return realSections;
        }
        
        public LinkedListNode<Section> GetLastStartGridNode()
        {
            LinkedListNode<Section> lastStartGridNode = null;
            
            for (var node = Sections.First; node != null; node = node.Next)
            {
                if (node.Value.SectionType == SectionTypes.StartGrid)
                {
                    lastStartGridNode = node;
                }
            }
            
            return lastStartGridNode;
        }
    }
}
