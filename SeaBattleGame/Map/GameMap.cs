using SeaBattleGame.GameConfig;
using SeaBattleGame.Map.MapResponses;

namespace SeaBattleGame.Map
{
    public class GameMap : IGameMap
    {
        private GameModeConfig _gameModeConfig;

        private bool _isAllShipsPlaced = false;
        private bool _isAllShipsDestroyed = false;
        private Dictionary<int, int> _shipsSizeToCount = new();
        private GameMapBody GameMapBody { get; set; }

        public int Size
        {
            get { return GameMapBody.Size; }
        }

        public event IGameMap.OnAllShipsDestroyed AllShipsDestroyed;

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
                GameMapBody.OnCellHitted(neighbour);
            }
        }
        public List<GameCell> GetNeighboursCells(Ship ship)
        {
            var shipLocation = GameMapBody.ShipToLocation[ship];

            HashSet<GameCell> neighbours = new();

            foreach (var cell in shipLocation)
            {
                List<GameCell> potentialNeighbours = new()
                {
                    new GameCell(cell.X - 1, cell.Y),
                    new GameCell(cell.X + 1, cell.Y),
                    new GameCell(cell.X, cell.Y - 1),
                    new GameCell(cell.X, cell.Y + 1),
                    new GameCell(cell.X + 1, cell.Y + 1),
                    new GameCell(cell.X - 1, cell.Y - 1),
                    new GameCell(cell.X -1, cell.Y + 1),
                    new GameCell(cell.X + 1, cell.Y -1)
                };

                foreach (var neighbour in potentialNeighbours)
                {
                    if (shipLocation.TrueForAll(x => !x.Equals(neighbour)) && IsCellOnGameMap(neighbour))
                    {
                        neighbours.Add(neighbour);
                    }
                }
            }

            List<GameCell> neighboursGameCells = new();

            foreach(var neighbour in neighbours)
            {
                neighboursGameCells.Add(GameMapBody.CellToShip.Keys.FirstOrDefault(c => c.Equals(neighbour)));
            }

            return neighboursGameCells;
        }
        private bool IsCellOnGameMap(GameCell cell)
        {
            return (cell.X >= 0 && cell.X < Size) && (cell.Y >= 0 && cell.Y < Size);
        }

        public HitGameMapResponse Hit(GameCell gameCell)
        {
            if (!_isAllShipsDestroyed)
            {
                var hitResponse = new HitGameMapResponse(gameCell);

                var cell = GameMapBody.CellToShip.Keys.FirstOrDefault(x => x.Equals(gameCell));

                if (cell.Hitted == true)
                {
                    hitResponse.HitStatus = HitStatus.AlreadyHitted;
                    hitResponse.Success = false;
                    hitResponse.ErrorMessage = "Клетка была поражена раннее.";

                    return hitResponse;
                }

                GameMapBody.OnCellHitted(cell);

                var ship = IsShipOnCell(cell);

                if (ship != null)
                {
                    hitResponse.HittedShip = ship;
                    hitResponse.HitStatus = HitStatus.Hitted;

                    if (IsShipDestroyed(ship))
                    {
                        ship.Killed = true;

                        FillDestroyedShipArea(ship);
                        InvokeEventIfAllShipsDestroyed();
                    }
                }
                else
                {
                    hitResponse.HitStatus = HitStatus.Missed;
                }

                hitResponse.Success = true;

                return hitResponse;
            }

            return new HitGameMapResponse("Все корабли уже уничтожены.");
        }

        private void InvokeEventIfAllShipsDestroyed()
        {
            foreach (var ship in GameMapBody.ShipToLocation.Keys) 
            {
                if (!ship.Killed)
                {
                    return;
                }
            }

            _isAllShipsDestroyed = true;

            AllShipsDestroyed?.Invoke(this);
        }

        public bool IsShipDestroyed(Ship ship)
        {
            var shipLocation = GameMapBody.ShipToLocation[ship];

            if (shipLocation.TrueForAll(cell => cell.Hitted))
            {
                return true;
            }

            return false;
        }

        public ShipAddedResponse TryAddShip(Ship ship, GameCell startPosition, ShipOrientation shipOrientation)
        {
            if (!_isAllShipsPlaced)
            {
                var response = GameMapBody.TryAddShip(ship, startPosition, shipOrientation);

                if (response.Success)
                {
                    _shipsSizeToCount[ship.Size]++;

                    SetFlagIfAllShipsPlaced();
                }

                return response;
            }

            return new ShipAddedResponse("Достигнуто максимальное количество кораблей на поле при данной конфигурации.");
        }

        private void SetFlagIfAllShipsPlaced()
        {
            foreach (var shipsSize in _gameModeConfig.Ships)
            {
                if (_shipsSizeToCount[shipsSize.Size] != shipsSize.Count){

                    return;
                }
            }

            _isAllShipsPlaced = true;
        }

        public ShipLocationChangedResponse TryChangeShipLocation(Ship ship, GameCell newStartPosition, ShipOrientation newShipOrientation)
        {
            return GameMapBody.TryChangeShipLocation(ship, newStartPosition, newShipOrientation);
        }

        public Ship? IsShipOnCell(GameCell gameCell)
        {
            return GameMapBody.CellToShip[gameCell];
        }

        public List<GameCell> GetShipLocation(Ship ship)
        {
            return GameMapBody.ShipToLocation[ship];
        }

        public void Clear()
        {
            InitializeMap(Size);
        }

        public ShipsAddedResponse TryAddShipsRandomly(List<Ship> ships)
        {
            var response = new ShipsAddedResponse();

            HashSet<GameCell> forbiddenCells = new();

            ShuffleShips(ships);

            Random random = new Random();

            foreach (Ship ship in ships)
            {
                GameCell startCell = new GameCell(random.Next(0, Size), random.Next(0, Size));

                bool shipPlaced = false;

                for (int i = startCell.X; i < Size && !shipPlaced; i++)
                {
                    for (int j = startCell.Y; j < Size && !shipPlaced; j++)
                    {
                        var addedResponse = TryAddVerticalOrHorizontal(ship, new GameCell(i, j), forbiddenCells);

                        if(addedResponse != null)
                        {
                            response.Ships.Add(addedResponse);

                            shipPlaced = true;
                        }
                    }
                }

                if (!shipPlaced)
                {
                    for (int i = 0; i < Size && !shipPlaced; i++)
                    {
                        for (int j = 0; j < Size && !shipPlaced; j++)
                        {
                            var addedResponse = TryAddVerticalOrHorizontal(ship, new GameCell(i, j), forbiddenCells);

                            if (addedResponse != null)
                            {
                                response.Ships.Add(addedResponse);

                                shipPlaced = true;
                            }
                        }
                    }
                }

                if (!shipPlaced)
                {
                    Clear();
                    response.ErrorMessage = "Не удалость расставить все корабли.";
                    return response;

                }
            }

            _isAllShipsPlaced = true;

            response.Success = true;

            //foreach(var ship in ships)
            //{
            //    var shipAddedResponse = new ShipAddedResponse(ship, true, GetShipLocation(ship));
            //    response.Ships.Add(shipAddedResponse);
            //}

            return response;
        }

        private bool CanPlaceShip(Ship ship, GameCell startCell, ShipOrientation orientation, HashSet<GameCell> forbiddenCells)
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
                if (forbiddenCells.Contains(shipCell))
                {
                    canBePlaced = false;
                    break;
                }
            }

            return canBePlaced;
        }

        private ShipAddedResponse TryAddShip(Ship ship, GameCell gameCell, ShipOrientation orientation, HashSet<GameCell> forbiddenCells)
        {
            var response = TryAddShip(ship, gameCell, orientation);

            if (response.Success)
            {
                var shipLocation = GetShipLocation(ship);
                var neighboursCells = GetNeighboursCells(ship);

                foreach (var shipCell in shipLocation)
                {
                    forbiddenCells.Add(shipCell);
                }

                foreach(var neighbourCell in neighboursCells)
                {
                    forbiddenCells.Add(neighbourCell);
                }
            }

            return response;
        }
        private ShipAddedResponse? TryAddVerticalOrHorizontal(Ship ship, GameCell gameCell, HashSet<GameCell> forbiddenCells)
        {
            if (CanPlaceShip(ship, gameCell, ShipOrientation.Horizontal, forbiddenCells))
            {
                var tryAddHorizontalResponse = TryAddShip(ship, gameCell, ShipOrientation.Horizontal, forbiddenCells);

                if (tryAddHorizontalResponse.Success)
                {
                    return tryAddHorizontalResponse;
                }
            }

            if (CanPlaceShip(ship, gameCell, ShipOrientation.Vertical, forbiddenCells))
            {
                var tryAddVerticalResponse = TryAddShip(ship, gameCell, ShipOrientation.Vertical, forbiddenCells);

                if (tryAddVerticalResponse.Success)
                {
                    return tryAddVerticalResponse;
                }
            }

            return null;
        }

        private void ShuffleShips(List<Ship> ships)
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

        public GameMap(GameModeConfig gameModeConfig)
        {
            _gameModeConfig = gameModeConfig;

            InitializeMap(gameModeConfig.GameMapSize);
            InitializeShipsToCount(gameModeConfig.Ships);
        }

        private void InitializeShipsToCount(List<ConfigShip> configShips)
        {
            foreach(var configShip in configShips)
            {
                _shipsSizeToCount[configShip.Size] = 0;
            }
        }

        int IGameMap.Size()
        {
            return Size;
        }
    }
}