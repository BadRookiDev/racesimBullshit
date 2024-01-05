using System;
namespace Model
{
    public class Section
    {
        public SectionTypes SectionType { get; set; }

        public Section(SectionTypes sectiontype)
        {
            SectionType = sectiontype;
        }
    }
}
