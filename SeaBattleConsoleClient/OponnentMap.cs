using SeaBattleGame.Game.Responses;
using SeaBattleGame;

namespace SeaBattleConsoleClient
{
    public class OponnentMap
    {
        private int _size;

        private Dictionary<string, Ship> _shipIdToShip = new();

        private Dictionary<GameCell, Ship?> _cellToShip = new();
        public OponnentMap(int size)
        {
            _size = size;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    _cellToShip.Add(new GameCell(i, j), null);
                }
            }
        }
        private void FillDestroyedShipArea(Ship destroyedShip)
        {
            var neighbours = GetNeighboursCells(destroyedShip);

            foreach (var neighbour in neighbours)
            {
                neighbour.Hitted = true;
            }
        }

        public List<GameCell> GetNeighboursCells(Ship ship)
        {
            var shipLocation = ShipToLocation(ship);

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

            foreach (var neighbour in neighbours)
            {
                neighboursGameCells.Add(_cellToShip.Keys.FirstOrDefault(c => c.Equals(neighbour)));
            }

            return neighboursGameCells;
        }

        private List<GameCell> ShipToLocation(Ship ship)
        {
            List<GameCell> location = new();

            foreach (var cell in _cellToShip.Keys)
            {
                Ship? s = _cellToShip[cell];

                if (s != null && s.Id == ship.Id)
                {
                    location.Add(cell);
                }
            }

            return location;
        }

        private Ship? IsShipOnCell(GameCell gameCell)
        {
            return _cellToShip[gameCell];
        }

        public void Print()
        {
            var matrixGameMapView = GetMapMatrixView();

            for (int i = 0; i < matrixGameMapView.Count; i++)
            {
                for (int j = 0; j < matrixGameMapView.Count; j++)
                {
                    var currnetCell = matrixGameMapView[i][j];

                    Ship? ship = IsShipOnCell(currnetCell);

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

        private List<List<GameCell>> GetMapMatrixView()
        {
            List<List<GameCell>> matrixGameMapView = new();

            for (int i = 0; i < _size; i++)
            {
                var currentLevel = _cellToShip.Keys.Where(c => c.Y == i).ToList();

                matrixGameMapView.Add(currentLevel);
            }

            return matrixGameMapView;
        }

        public void Hit(PlayerHitResponse hitResponse)
        {
            if (hitResponse.Success)
            {
                var gameCell = hitResponse.HitGameMapResponse.HittedCell;

                _cellToShip.Keys.FirstOrDefault(c => c.Equals(gameCell)).Hitted = true;

                if (hitResponse.HitGameMapResponse.HitStatus == SeaBattleGame.Map.MapResponses.HitStatus.Hitted)
                {
                    var hittedShip = hitResponse.HitGameMapResponse.HittedShip;

                    if (hittedShip != null)
                    {
                        _shipIdToShip[hittedShip.Id] = hittedShip;

                        _cellToShip[gameCell] = _shipIdToShip[hittedShip.Id];
                    }

                    if (hittedShip.Killed)
                    {
                        MarkAllCellsWithShipAsKilled(hittedShip);
                        FillDestroyedShipArea(hittedShip);
                    }
                }
            }
        }

        private void MarkAllCellsWithShipAsKilled(Ship ship)
        {
            foreach(var cell in _cellToShip.Keys)
            {
                if (_cellToShip[cell] != null && _cellToShip[cell].Id  == ship.Id)
                {
                    _cellToShip[cell].Killed = true;
                }
            }
        }

        private bool IsCellOnGameMap(GameCell cell)
        {
            return (cell.X >= 0 && cell.X < _size) && (cell.Y >= 0 && cell.Y < _size);
        }
    }
}
