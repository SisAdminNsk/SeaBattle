using Microsoft.EntityFrameworkCore;
using SeaBattle.Core;

namespace SeaBattle.Context
{
    public interface IContext
    {
        public DbSet<ConfigShip> ConfigShips { get; }
        public DbSet<ShipsCount> ShipsCount { get; }
        public DbSet<GameConfig> GameConfigs { get; }
    }
}
