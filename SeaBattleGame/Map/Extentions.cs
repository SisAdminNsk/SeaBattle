namespace SeaBattleGame.Map
{
    public static class Extentions
    {
        private static List<List<GameCell>> GetMapMatrixView(IGameMap map)
        {
            var cellsToShipMap = map.GetCellsToShipMap();

            List<List<GameCell>> matrixGameMapView = new();

            for (int i = 0; i < map.Size(); i++)
            {
                var currentLevel = cellsToShipMap.Keys.Where(c => c.Y == i).ToList();

                matrixGameMapView.Add(currentLevel);
            }

            return matrixGameMapView;
        }

        public static void PrintOpponentGameMap(this IGameMap map)
        {
            var matrixGameMapView = GetMapMatrixView(map);

            for (int i = 0; i < matrixGameMapView.Count; i++)
            {
                for (int j = 0; j < matrixGameMapView.Count; j++)
                {
                    var currnetCell = matrixGameMapView[i][j];

                    Ship? ship = map.IsShipOnCell(currnetCell);

                    if (ship is not null)
                    {
                        if (currnetCell.Hitted)
                        {
                            if (ship.Killed)
                            {
                                Console.Write("k");
                            }
                            else
                            {
                                Console.Write("h");
                            }
                        }
                        else
                        {
                            Console.Write("o");
                        }
                    }
                    else
                    {
                        if (currnetCell.Hitted)
                        {
                            Console.Write("m");
                        }
                        else
                        {
                            Console.Write("o");
                        }
                    }
                }

                Console.WriteLine();
            }
        }

        public static void PrintGameMap(this IGameMap map)
        {
            var matrixGameMapView = GetMapMatrixView(map);

            for (int i = 0; i < matrixGameMapView.Count; i++)
            {
                for (int j = 0; j < matrixGameMapView.Count; j++)
                {
                    var currnetCell = matrixGameMapView[i][j];

                    Ship? ship = map.IsShipOnCell(currnetCell);

                    if (ship is not null)
                    {
                        if (currnetCell.Hitted)
                        {
                            if (ship.Killed)
                            {
                                Console.Write("k");
                            }
                            else
                            {
                                Console.Write("h");
                            }
                        }
                        else
                        {
                            Console.Write("#");
                        }
                    }
                    else
                    {
                        if (currnetCell.Hitted)
                        {
                            Console.Write("m");
                        }
                        else
                        {
                            Console.Write("o");
                        }
                    }
                }

                Console.WriteLine();
            }
        }
    }
}
