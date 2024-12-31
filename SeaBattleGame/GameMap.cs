using SeaBattleGame.Exceptions;

namespace SeaBattleGame
{
    public class GameMap : IGameMap
    {
        private int _size;

        private Dictionary<Ship, List<GameCell>> _shipToLocation = new();
        private Dictionary<GameCell, Ship?> _cellToShip = new(); 

        public event IGameMap.OnShipHitted? ShipHitted;
        public event IGameMap.OnShipDestroyed? ShipDestroyed;
        public event IGameMap.OnHitMissed? HitMissed;
        private void InitializeMap(int size)
        {
            _size = size;
        }
        private void FillDestroyedShipArea(Ship? destroyedShip)
        {
            var neighbours = GetNeighboursCells(destroyedShip);

            foreach (var neighbour in neighbours)
            {
                _cellToShip.Keys.FirstOrDefault(x => x.CompareValue(neighbour)).Hitted = true;
            }
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
                    if (shipLocation.TrueForAll(x => !x.CompareValue(neighbour)) && IsCellOnGameMap(neighbour))
                    {
                        neighbours.Add(neighbour);
                    }
                }
            }

            return neighbours.ToList();
        }
        private bool IsCellOnGameMap(GameCell cell)
        {
            return (cell.X >= 0 && cell.X <= _size) && (cell.Y >= 0 && cell.Y <= _size);
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

                _cellToShip.Add(gameCell, ship);

                shipLocation.Add(gameCell);
            }

            _shipToLocation.Add(ship, shipLocation);

            return true;
        }

        public void Hit(GameCell gameCell)
        {
            var shipOrNull = IsShipOnCell(gameCell);

            if (shipOrNull is not null)
            {
                var cell = _shipToLocation[shipOrNull].Find(cell => cell.Equals(gameCell));

                if (!cell.Hitted)
                {
                    cell.Hitted = true;
                    ShipHitted?.Invoke(shipOrNull, this, gameCell);
                }

                if (IsShipDestroyed(shipOrNull))
                {
                    ShipDestroyed?.Invoke(shipOrNull, this, gameCell);
                }
            }
            else
            {
                HitMissed?.Invoke(this, gameCell);
            }
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
                if (startPosition.Y + ship.Size > _size || startPosition.X >= _size) return false;
            }
            else
            {
                if (startPosition.X + ship.Size > _size || startPosition.Y >= _size) return false; 
            }

            List<GameCell> shipCells = GetShipCells(ship, startPosition, shipOrientation);

            foreach (var cell in shipCells)
            {
                if (_cellToShip.ContainsKey(cell) && _cellToShip[cell] != null)
                {
                    return false; 
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
        public bool ChangeShipLocation(Ship ship, GameCell newStartPosition, ShipOrientation newShipOrientation)
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
    }
}
