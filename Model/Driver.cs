namespace Model
{
    public class Driver : IParticipant
    {
        private string _name;
        private int _points;
        private IEquipment _equipment;
        private TeamColor _teamColor;
        public string Name { get => _name; set => _name = value; }
        public int Points { get => _points; set => _points = value; }
        public IEquipment Equipment { get => _equipment; set => _equipment = value; }
        public TeamColor TeamColor { get => _teamColor; set => _teamColor = value; }
        

        public Driver(string name, int points, IEquipment equipment, TeamColor teamColor) 
        {
            Name = name;
            Points = points;
            Equipment = equipment;
            TeamColor = teamColor;
        }
    }
}
