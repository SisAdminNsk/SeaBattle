using System;
using System.Collections.Generic;

namespace SeaBattleGame.Map
{
    public static class Extentions
    {
        private static HashSet<GameCell> ForbiddenCells = new HashSet<GameCell>();

        private static bool CanPlaceShip(Ship ship, GameCell startCell, ShipOrientation orientation)
        {
            List<GameCell> shipsCells = new();

            for (int i = 0; i < ship.Size; i++)
            {
                if (orientation == ShipOrientation.Horizontal)
                {
                    shipsCells.Add(new GameCell(startCell.X + i, startCell.Y));
                }
                else
                {
                    shipsCells.Add(new GameCell(startCell.X, startCell.Y + i));
                }
            }

            bool canBePlaced = true;

            foreach (var shipCell in shipsCells)
            {
                if (ForbiddenCells.Contains(shipCell))
                {
                    canBePlaced = false;
                    break;
                }
            }

            return canBePlaced;
        }

        private static bool TryAddShip(GameMap map, Ship ship, GameCell gameCell, ShipOrientation orientation)
        {
            var success = map.TryAddShip(ship, gameCell, orientation).Success;

            if (success)
            {
                var shipLocation = map.GetShipLocation(ship);

                foreach (var shipCell in shipLocation)
                {
                    ForbiddenCells.Add(shipCell);
                }

                return true;
            }

            return false;
        }

        private static bool TryAddVerticalOrHorizontal(GameMap map, Ship ship, GameCell gameCell)
        {
            if (CanPlaceShip(ship, gameCell, ShipOrientation.Horizontal))
            {
                if (TryAddShip(map, ship, gameCell, ShipOrientation.Horizontal))
                {
                    return true;
                }
            }

            if (CanPlaceShip(ship, gameCell, ShipOrientation.Vertical))
            {
                if (TryAddShip(map, ship, gameCell, ShipOrientation.Vertical))
                {
                    return true;
                }
            }

            return false;
        }

        private static void ShuffleShips(List<Ship> ships)
        {
            Random rng = new Random();

            int n = ships.Count;

            while (n > 1)
            {
                int k = rng.Next(n--);
                var temp = ships[n];
                ships[n] = ships[k];
                ships[k] = temp;
            }
        }

        public static bool TryAddShipsRandomly(this GameMap map, List<Ship> ships)
        {
            ForbiddenCells.Clear();

            ShuffleShips(ships);

            Random random = new Random();

            foreach (Ship ship in ships)
            {
                GameCell startCell = new GameCell(random.Next(0, map.Size), random.Next(0, map.Size));

                bool shipPlaced = false;

                for (int i = startCell.X; i < map.Size && !shipPlaced; i++)
                {
                    for (int j = startCell.Y; j < map.Size && !shipPlaced; j++)
                    {
                        if(TryAddVerticalOrHorizontal(map, ship, new GameCell(i, j)))
                        {
                            shipPlaced = true;
                        }
                    }
                }

                if (!shipPlaced) 
                {
                    for (int i = 0; i < startCell.X; i++)
                    {
                        for (int j = 0; j < startCell.Y; j++)
                        {
                            if (TryAddVerticalOrHorizontal(map, ship, new GameCell(i, j)))
                            {
                                shipPlaced = true;
                            }
                        }
                    }
                }

                if (!shipPlaced)
                {
                    return false;
                }
            }

            return true;
        }
        public static void PrintGameMap(this GameMap map)
        {
            var cellsToShipMap = map.GetCellsToShipMap();

            List<List<GameCell>> matrixGameMapView = new();

            for (int i = 0; i < map.Size; i++)
            {
                var currentLevel = cellsToShipMap.Keys.Where(c => c.Y == i).ToList();

                matrixGameMapView.Add(currentLevel);
            }

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
