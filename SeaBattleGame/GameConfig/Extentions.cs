namespace SeaBattleGame.GameConfig
{
    public static class Extentions
    {
        public static List<Ship> GetShipsFromConfig(this GameModeConfig gameModeConfig)
        {
            List<Ship> ships = new();

            foreach(var configShip in gameModeConfig.Ships)
            {
                for(int i = 0; i < configShip.Count; i++)
                {
                    ships.Add(new Ship(configShip.Size));
                }
            }

            return ships;
        }
    }
}
