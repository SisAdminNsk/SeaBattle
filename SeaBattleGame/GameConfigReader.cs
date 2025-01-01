namespace SeaBattleGame
{
    class ShipType
    {
        public string? Name { get; set; }
        public int Size { get; set; }
        public int Count { get; set; }
    }
    class GameConfig
    {
        public int GameMapSize { get; set; }
        public List<ShipType>? ShipTypes { get; set; }
    }

    class GameConfigReader
    {
        private string ConfigFilePath = "GameConfig.json";
        //public GameConfig ReadConfig()
        //{

        //}
    }
}
