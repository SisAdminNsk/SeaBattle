using SeaBattleGame;
using SeaBattleGame.Map;

namespace SeaBattle.Tests
{
    public class GameMapTests
    {
        [Fact]
        public void Should_Add_Ships_Correctly()
        {
            var ships = GetShips();

            int gameMapSize = 10;
            var gameMap = new GameMap(gameMapSize);

            Assert.True(gameMap.TryAddShip(ships[0], new GameCell(9, 2), ShipOrientation.Vertical));
            Assert.True(gameMap.TryAddShip(ships[1], new GameCell(2, 2), ShipOrientation.Vertical));
            Assert.True(gameMap.TryAddShip(ships[2], new GameCell(9, 4), ShipOrientation.Vertical));
            Assert.True(gameMap.TryAddShip(ships[3], new GameCell(8, 9), ShipOrientation.Vertical));

            Assert.True(gameMap.TryAddShip(ships[4], new GameCell(2, 0), ShipOrientation.Horizontal));
            Assert.True(gameMap.TryAddShip(ships[5], new GameCell(5, 0), ShipOrientation.Vertical));
            Assert.True(gameMap.TryAddShip(ships[6], new GameCell(0, 6), ShipOrientation.Vertical));

            Assert.True(gameMap.TryAddShip(ships[7], new GameCell(2, 4), ShipOrientation.Horizontal));
            Assert.True(gameMap.TryAddShip(ships[8], new GameCell(5, 6), ShipOrientation.Horizontal));

            Assert.True(gameMap.TryAddShip(ships[9], new GameCell(3, 9), ShipOrientation.Horizontal));
        }

        [Fact]
        public void Should_Detect_Collisions_Correctly()
        {
            var ships = GetShips();

            int gameMapSize = 10;
            var gameMap = new GameMap(gameMapSize);

            Assert.True(gameMap.TryAddShip(ships[0], new GameCell(9, 2), ShipOrientation.Vertical));
            Assert.True(gameMap.TryAddShip(ships[1], new GameCell(2, 2), ShipOrientation.Vertical));
            Assert.False(gameMap.TryAddShip(ships[2], new GameCell(9, 3), ShipOrientation.Vertical));
            Assert.True(gameMap.TryAddShip(ships[3], new GameCell(8, 9), ShipOrientation.Vertical));

            Assert.True(gameMap.TryAddShip(ships[4], new GameCell(0, 0), ShipOrientation.Vertical));
            Assert.True(gameMap.TryAddShip(ships[5], new GameCell(5, 0), ShipOrientation.Vertical));
            Assert.True(gameMap.TryAddShip(ships[6], new GameCell(0, 6), ShipOrientation.Vertical));

            Assert.True(gameMap.TryAddShip(ships[7], new GameCell(2, 4), ShipOrientation.Horizontal));
            Assert.True(gameMap.TryAddShip(ships[8], new GameCell(5, 6), ShipOrientation.Vertical));

            Assert.False(gameMap.TryAddShip(ships[9], new GameCell(3, 9), ShipOrientation.Horizontal));
        }

        [Fact]
        public void Should_Change_Ships_Location_Correctly()
        {
            var ships = GetShips();

            int gameMapSize = 10;
            var gameMap = new GameMap(gameMapSize);

            Assert.True(gameMap.TryAddShip(ships[9], new GameCell(6,9), ShipOrientation.Horizontal));
            Assert.True(gameMap.TryChangeShipLocation(ships[9], new GameCell(0,9), ShipOrientation.Horizontal));

            var location = gameMap.GetShipLocation(ships[9]);

            foreach(var gameCell in location)
            {
                var ship = gameMap.IsShipOnCell(gameCell);
                Assert.Same(ship, ships[9]);
            }
        }

        [Fact]
        public void Should_Change_Ships_Location_With_Collisions_Correctly()
        {
            var ships = GetShips();

            int gameMapSize = 10;
            var gameMap = new GameMap(gameMapSize);

            Assert.True(gameMap.TryAddShip(ships[0], new GameCell(0,0), ShipOrientation.Horizontal));
            Assert.True(gameMap.TryAddShip(ships[1], new GameCell(0,2), ShipOrientation.Horizontal));
            Assert.True(gameMap.TryAddShip(ships[2], new GameCell(0,4), ShipOrientation.Horizontal));
            Assert.True(gameMap.TryAddShip(ships[3], new GameCell(0,6), ShipOrientation.Horizontal));

            Assert.True(gameMap.TryAddShip(ships[4], new GameCell(2,0), ShipOrientation.Horizontal));
            Assert.True(gameMap.TryAddShip(ships[5], new GameCell(2,2), ShipOrientation.Horizontal));
            Assert.True(gameMap.TryAddShip(ships[6], new GameCell(2,4), ShipOrientation.Horizontal));

            Assert.True(gameMap.TryAddShip(ships[7], new GameCell(6,0), ShipOrientation.Vertical));
            Assert.True(gameMap.TryAddShip(ships[8], new GameCell(6,4), ShipOrientation.Horizontal));

            Assert.True(gameMap.TryAddShip(ships[9], new GameCell(5,9), ShipOrientation.Horizontal));

            Assert.False(gameMap.TryChangeShipLocation(ships[9], new GameCell(7,6), ShipOrientation.Horizontal));
            Assert.False(gameMap.TryChangeShipLocation(ships[0], new GameCell(10,0), ShipOrientation.Horizontal));
        }
        private static List<Ship> GetShips()
        {
            return new List<Ship>
            {
                new Ship(1),
                new Ship(1),
                new Ship(1),
                new Ship(1),

                new Ship(2),
                new Ship(2),
                new Ship(2),

                new Ship(3),
                new Ship(3),

                new Ship(4)
            };
        }
    }
} 
            