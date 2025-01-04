using SeaBattleGame.Map.MapResponses;

namespace SeaBattleGame.Map
{
    public class GameMap : IGameMap
    {
        private GameMapBody GameMapBody { get; set; }

        public int Size
        {
            get { return GameMapBody.Size; }
        }

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
                var cell = GameMapBody.CellToShip.Keys.FirstOrDefault(x => x.Equals(neighbour));

                if (cell != null)
                {
                    GameMapBody.OnCellHitted(cell);
                }
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
            return (cell.X >= 0 && cell.X < Size) && (cell.Y >= 0 && cell.Y < Size);
        }

        public HitGameMapResponse Hit(GameCell gameCell)
        {
            var hitResponse = new HitGameMapResponse(gameCell);

            var cell = GameMapBody.CellToShip.Keys.FirstOrDefault(x => x.Equals(gameCell));

            if (cell.Hitted == true)
            {
                hitResponse.HitStatus = HitStatus.AlreadyHitted;

                return hitResponse;
            }

            GameMapBody.OnCellHitted(gameCell);

            var ship = IsShipOnCell(gameCell);

            if (ship != null)
            {
                hitResponse.HittedShip = ship;
                hitResponse.HitStatus = HitStatus.Hitted;

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
            return GameMapBody.TryAddShip(ship, startPosition, shipOrientation);
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
                        if (TryAddVerticalOrHorizontal(ship, new GameCell(i, j), forbiddenCells))
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
                            if (TryAddVerticalOrHorizontal(ship, new GameCell(i, j), forbiddenCells))
                            {
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

            response.Success = true;

            foreach(var ship in ships)
            {
                var shipAddedResponse = new ShipAddedResponse(ship, true, GetShipLocation(ship));
                response.Ships.Add(shipAddedResponse);
            }

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

        private bool TryAddShip(Ship ship, GameCell gameCell, ShipOrientation orientation, HashSet<GameCell> forbiddenCells)
        {
            var success = TryAddShip(ship, gameCell, orientation).Success;

            if (success)
            {
                var shipLocation = GetShipLocation(ship);

                foreach (var shipCell in shipLocation)
                {
                    forbiddenCells.Add(shipCell);
                }

                return true;
            }

            return false;
        }
        private bool TryAddVerticalOrHorizontal(Ship ship, GameCell gameCell, HashSet<GameCell> forbiddenCells)
        {
            if (CanPlaceShip(ship, gameCell, ShipOrientation.Horizontal, forbiddenCells))
            {
                if (TryAddShip(ship, gameCell, ShipOrientation.Horizontal, forbiddenCells))
                {
                    return true;
                }
            }

            if (CanPlaceShip(ship, gameCell, ShipOrientation.Vertical, forbiddenCells))
            {
                if (TryAddShip(ship, gameCell, ShipOrientation.Vertical, forbiddenCells))
                {
                    return true;
                }
            }

            return false;
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

        public GameMap(int size)
        {
            InitializeMap(size);
        }
    }
}