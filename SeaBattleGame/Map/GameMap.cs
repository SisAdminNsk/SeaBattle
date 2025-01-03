using SeaBattleGame.Exceptions;
using SeaBattleGame.Map.MapResponses;
using System.Drawing;

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

        public void SetCellHitted(GameCell gameCell)
        {
            var cell = CellToShip.Keys.FirstOrDefault(x => x.Equals(gameCell));

            if (cell is null)
            {
                throw new NullReferenceException("gameCell не найдена среди ключей словаря _cellToShip");
            }

            cell.Hitted = true;

            var ship = CellToShip[gameCell];

            if(ship != null)
            {
                ShipToLocation[ship].Find(cell => cell.Equals(gameCell)).Hitted = true;
            }
        }

        public bool TryChangeShipLocation(Ship ship, GameCell newStartPosition, ShipOrientation newShipOrientation)
        {
            if (!ShipToLocation.ContainsKey(ship))
            {
                throw new IncorrectMethodUsage("Неправильное использование метода," +
                    " метод используется для изменения положения уже установленного корбаля, но корабль еще не установлен.");
            }

            if (!ValidateShipLocation(ship, newStartPosition, newShipOrientation))
            {
                return false;
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

            return true;
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

        public bool TryAddShip(Ship ship, GameCell startPosition, ShipOrientation shipOrientation)
        {
            if (!ValidateShipLocation(ship, startPosition, shipOrientation))
            {
                return false;
            }

            if (ShipToLocation.ContainsKey(ship))
            {
                return false;
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

            return true;
        }
    }

    public class GameMap : IGameMap
    {
        private GameMapBody GameMapBody { get; set; }
        public int Size { get; private set; }
        public Dictionary<GameCell, Ship?> GetCellsToShipMap()
        {
            return new Dictionary<GameCell, Ship?>(GameMapBody.CellToShip);
        }

        private void InitializeMap(int size)
        {
            GameMapBody = new GameMapBody();
            GameMapBody.InitializeMap(size);
        }
        private void FillDestroyedShipArea(Ship destroyedShip)
        {
            var neighbours = GetNeighboursCells(destroyedShip);

            foreach (var neighbour in neighbours)
            {
                var cell = _cellToShip.Keys.FirstOrDefault(x => x.Equals(neighbour));

                if(cell != null)
                {
                    cell.Hitted = true;
                }
            }
        }

        public List<GameCell> GetShipLocation(Ship ship)
        {
            return _shipToLocation[ship];
        }

        List<GameCell> GetNeighboursCells(Ship ship)
        {
            var shipLocation = _shipToLocation[ship];

            HashSet<GameCell> neighbours = new();

            foreach (var cell in shipLocation)
            {
                List<GameCell> potentialNeighbours = new()
                {
                    new GameCell(cell.X - 1, cell.Y),
                    new GameCell(cell.X + 1, cell.Y),
                    new GameCell(cell.X, cell.Y - 1),
                    new GameCell(cell.X, cell.Y + 1)
                };

                foreach (var neighbour in potentialNeighbours)
                {
                    if (shipLocation.TrueForAll(x => !x.Equals(neighbour)) && IsCellOnGameMap(neighbour))
                    {
                        neighbours.Add(neighbour);
                    }
                }
            }

            return neighbours.ToList();
        }
        private bool IsCellOnGameMap(GameCell cell)
        {
            return (cell.X >= 0 && cell.X <= Size) && (cell.Y >= 0 && cell.Y <= Size);
        }
        public bool TryAddShip(Ship ship, GameCell startPosition, ShipOrientation shipOrientation)
        {
            if (!ValidateShipLocation(ship, startPosition, shipOrientation))
            {
                return false;
            }

            if (_shipToLocation.ContainsKey(ship))
            {
                return false;
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

                _cellToShip[gameCell] = ship;

                shipLocation.Add(gameCell);
            }

            _shipToLocation.Add(ship, shipLocation);

            return true;
        }

        public HitGameMapResponse Hit(GameCell gameCell)
        {
            // переписать 

            var hitResponse = new HitGameMapResponse();

            hitResponse.HittedCell = gameCell;
            hitResponse.HittedCell.Hitted = true;

            var cell = _cellToShip.Keys.FirstOrDefault(x => x.Equals(gameCell));

            if (cell.Hitted == true)
            {
                hitResponse.IsCellAlreadyHitted = true;

                return hitResponse;
            }

            cell.Hitted = true;

            Ship? ship = IsShipOnCell(gameCell);

            if (ship != null)
            {
                hitResponse.HittedShip = ship;

                hitResponse.HitStatus = HitStatus.Hitted;

                _shipToLocation[ship].Find(cell => cell.Equals(gameCell)).Hitted = true;

                if (IsShipDestroyed(ship))
                {
                    ship.Killed = true;

                    FillDestroyedShipArea(ship);
                }
            }
            else
            {
                hitResponse.HitStatus = HitStatus.Missed;
            }

            return hitResponse;
        }

        private bool IsShipDestroyed(Ship ship)
        {
            var shipLocation = _shipToLocation[ship];

            if (shipLocation.TrueForAll(cell => cell.Hitted))
            {
                return true;
            }

            return false;
        }

        public Ship? IsShipOnCell(GameCell gameCell)
        {
            return _cellToShip[gameCell];
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
                if (_cellToShip[cell] != null)
                {
                    if (!_cellToShip[cell].Equals(ship))
                    {
                        return false;
                    }
                }
            }

            foreach (var existingShipPair in _shipToLocation)
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

        public GameMap(int size)
        {
            InitializeMap(size);
        }
        public bool TryChangeShipLocation(Ship ship, GameCell newStartPosition, ShipOrientation newShipOrientation)
        {
            if (!_shipToLocation.ContainsKey(ship))
            {
                throw new IncorrectMethodUsage("Неправильное использование метода," +
                    " метод используется для изменения положения уже установленного корбаля, но корабль еще не установлен.");
            }

            if (!ValidateShipLocation(ship, newStartPosition, newShipOrientation))
            {
                return false;
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

                _cellToShip[gameCell] = ship;

                shipLocation.Add(gameCell);
            }

            _shipToLocation[ship] = shipLocation;

            return true;
        }
        private void ClearOldShipLocation(Ship ship)
        {
            var shipLocation = GetShipLocation(ship);

            foreach (var cell in shipLocation)
            {
                _cellToShip[cell] = null;
            }
        }
    }
}
