using CSharpFunctionalExtensions;

namespace SeaBattle.Core
{
    public class ShipsCount : Entity<Guid>
    {
        public ConfigShip ConfigShip { get; set; }
        public int Count { get; set; }
        public ShipsCount(ConfigShip configShip, int count) 
        {
            ConfigShip = configShip;
            Count = count;
        }
    }
}
