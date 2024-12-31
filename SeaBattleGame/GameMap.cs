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

        public void Hit(GameCell gameCell)
        {
            var shipOrNull = IsShipOnCell(gameCell);

            if(shipOrNull is not null)
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
        }
        private bool IsShipDestroyed(Ship ship)
        {
            var shipLocation = _shipToLocation[ship];

            if(shipLocation.TrueForAll(cell => cell.Hitted))
            {
                return true;
            }

            return false;
        }

        public bool ValidateHit(GameCell hit)
        {
            if(hit.X >  _size) return false;

            if(hit.Y > _size) return false;

            return true;
        }

        public Ship? IsShipOnCell(GameCell gameCell)
        {
            return _cellToShip[gameCell];
        }

        public bool TryAddShip(Ship ship, GameCell startPosition, ShipOrientation shipOrientation)
        {
            if(!ValidateShipLocation(ship, startPosition, shipOrientation))
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
        private bool ValidateShipLocation(Ship ship, GameCell startPosition, ShipOrientation shipOrientation)
        {
            if(shipOrientation == ShipOrientation.Vertical)
            {
                if (startPosition.X > _size || startPosition.Y + ship.Size > _size)
                {
                    return false;
                }
            }

            if(shipOrientation == ShipOrientation.Horizontal)
            {
                if (startPosition.X + ship.Size  > _size || startPosition.Y > _size)
                {
                    return false;
                }
            }

            return true;
        }
        public GameMap(int size)
        {
            InitializeMap(size);
        }

        private void InitializeMap(int size)
        {
            _size = size;
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
