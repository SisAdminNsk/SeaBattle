using CSharpFunctionalExtensions;

namespace SeaBattle.Core
{
    public class ConfigShip : Entity<Guid>
    {
        public string Name { get; set; }
        public ConfigShip(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
