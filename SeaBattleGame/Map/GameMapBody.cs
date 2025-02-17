﻿using SeaBattleGame.Map.MapResponses;

namespace SeaBattleGame.Map
{
    public class GameMapBody
    {
        public int Size { get; private set; }
        public Dictionary<Ship, List<GameCell>> ShipToLocation { get; private set; } = new();
        public Dictionary<GameCell, Ship?> CellToShip { get; private set; } = new();

        public void InitializeMap(int size)
        {
            Size = size;

            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    var gameCell = new GameCell(i, j);
                    CellToShip.Add(gameCell, null);
                }
            }
        }

        public void OnCellHitted(GameCell gameCell)
        {
            gameCell.Hitted = true; 

            var ship = CellToShip[gameCell];

            if (ship != null)
            {
                ShipToLocation[ship].Find(cell => cell.Equals(gameCell)).Hitted = true;
            }
        }

        public ShipLocationChangedResponse TryChangeShipLocation(Ship ship, GameCell newStartPosition, ShipOrientation newShipOrientation)
        {
            var response = new ShipLocationChangedResponse();

            if (!ShipToLocation.ContainsKey(ship))
            {
                response.ErrorMessage = "Неправильное использование метода, " +
                    "метод используется для изменения положения уже установленного корбаля, но корабль еще не установлен.";

                return response;
            }

            if (!ValidateShipLocation(ship, newStartPosition, newShipOrientation))
            {
                response.ErrorMessage = "Невалидная позиция корабля на карте.";

                return response;
            }

            ClearOldShipLocation(ship);

            var shipLocation = new List<GameCell>();

            for (int i = 0; i < ship.Size; i++)
            {
                var gameCell = new GameCell();

                if (newShipOrientation == ShipOrientation.Horizontal)
                {
                    gameCell = new GameCell(newStartPosition.X + i, newStartPosition.Y);
                }

                if (newShipOrientation == ShipOrientation.Vertical)
                {
                    gameCell = new GameCell(newStartPosition.X, newStartPosition.Y + i);
                }

                CellToShip[gameCell] = ship;

                shipLocation.Add(gameCell);
            }

            ShipToLocation[ship] = shipLocation;

            response.Success = true;
            response.Ship = ship;
            response.OldLocation = ShipToLocation[ship];
            response.NewLocation = shipLocation;

            return response;
        }
        private void ClearOldShipLocation(Ship ship)
        {
            var shipLocation = ShipToLocation[ship];

            foreach (var cell in shipLocation)
            {
                CellToShip[cell] = null;
            }
        }

        private bool ValidateShipLocation(Ship ship, GameCell startPosition, ShipOrientation shipOrientation)
        {
            if (shipOrientation == ShipOrientation.Vertical)
            {
                if (startPosition.Y + ship.Size > Size || startPosition.X >= Size) return false;
            }
            else
            {
                if (startPosition.X + ship.Size > Size || startPosition.Y >= Size) return false;
            }

            List<GameCell> shipCells = GetShipCells(ship, startPosition, shipOrientation);

            foreach (var cell in shipCells)
            {
                if (CellToShip[cell] != null)
                {
                    if (!CellToShip[cell].Equals(ship))
                    {
                        return false;
                    }
                }
            }

            foreach (var existingShipPair in ShipToLocation)
            {
                Ship existingShip = existingShipPair.Key;
                List<GameCell> existingShipCells = existingShipPair.Value;

                if (existingShip == ship) continue;


                foreach (var shipCell in shipCells)
                {
                    foreach (var existingCell in existingShipCells)
                    {
                        if (AreCellsAdjacent(shipCell, existingCell))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
        private List<GameCell> GetShipCells(Ship ship, GameCell startPosition, ShipOrientation shipOrientation)
        {
            List<GameCell> cells = new List<GameCell>();

            if (shipOrientation == ShipOrientation.Vertical)
            {
                for (int i = 0; i < ship.Size; i++)
                {
                    cells.Add(new GameCell { X = startPosition.X, Y = startPosition.Y + i });
                }
            }
            else
            {
                for (int i = 0; i < ship.Size; i++)
                {
                    cells.Add(new GameCell { X = startPosition.X + i, Y = startPosition.Y });
                }
            }

            return cells;
        }

        private bool AreCellsAdjacent(GameCell cell1, GameCell cell2)
        {
            int dx = Math.Abs(cell1.X - cell2.X);
            int dy = Math.Abs(cell1.Y - cell2.Y);

            return (dx <= 1 && dy <= 1) && !(dx == 0 && dy == 0);
        }

        public ShipAddedResponse TryAddShip(Ship ship, GameCell startPosition, ShipOrientation shipOrientation)
        {
            var response = new ShipAddedResponse();

            if (!ValidateShipLocation(ship, startPosition, shipOrientation))
            {
                response.ErrorMessage = "Невалидная позиция корабля на карте.";

                return response;
            }

            if (ShipToLocation.ContainsKey(ship))
            {
                response.ErrorMessage = "Корабль уже добавлен на карту.";

                return response;
            }

            var shipLocation = new List<GameCell>();

            for (int i = 0; i < ship.Size; i++)
            {
                var gameCell = new GameCell();

                if (shipOrientation == ShipOrientation.Horizontal)
                {
                    gameCell = new GameCell(startPosition.X + i, startPosition.Y);
                }

                if (shipOrientation == ShipOrientation.Vertical)
                {
                    gameCell = new GameCell(startPosition.X, startPosition.Y + i);
                }

                CellToShip[gameCell] = ship;

                shipLocation.Add(gameCell);
            }

            ShipToLocation.Add(ship, shipLocation);

            response.ShipOrientation = shipOrientation;
            response.StartCell = startPosition;
            response.Ship = ship;
            response.Success = true;
            response.ShipLocation = shipLocation;

            return response;
        }
    }
}
