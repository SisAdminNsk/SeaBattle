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

                if(cell != null)
                {
                    GameMapBody.OnCellHitted(cell);
                }
            }
        }

        private List<GameCell> GetNeighboursCells(Ship ship)
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

        public GameMap(int size)
        {
            InitializeMap(size);
        }
    }
}