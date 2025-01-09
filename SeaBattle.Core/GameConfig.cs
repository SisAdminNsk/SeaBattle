using CSharpFunctionalExtensions;

namespace SeaBattle.Core
{
    public class GameConfig : Entity<Guid>
    {
        public string Name { get; set; }

        public List<ShipsCount> ShipsConfigs { get; set; }

        public GameConfig(string name, List<ShipsCount> shipsConfigs) 
        {
            Name = name;
            ShipsConfigs = shipsConfigs;
        }
    }
}
