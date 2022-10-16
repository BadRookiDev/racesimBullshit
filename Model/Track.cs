using System;
using System.Collections.Generic;

namespace Model
{
    public class Track
    {
        public string Name { get; set; }
        public LinkedList<Section> Sections { get; set; }

        public Track(string _name, SectionTypes[] _sections)
        {
            Name = _name;
        }
    }
}
