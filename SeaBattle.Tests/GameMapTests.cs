using SeaBattleGame;

namespace SeaBattle.Tests
{
    public class GameMapTests
    {
        [Fact]
        public void Should_Add_Ships_Correctly()
        {
            var ship1 = new Ship(1);
            var ship2 = new Ship(1);
            var ship3 = new Ship(1);
            var ship4 = new Ship(1);

            var ship5 = new Ship(2);
            var ship6 = new Ship(2);
            var ship7 = new Ship(2);

            var ship8 = new Ship(3);
            var ship9 = new Ship(3);

            var ship10 = new Ship(4);

            int gameMapSize = 10;
            var gameMap = new GameMap(gameMapSize);

            Assert.True(gameMap.TryAddShip(ship1, new GameCell(9, 2), ShipOrientation.Vertical));
            Assert.True(gameMap.TryAddShip(ship2, new GameCell(2, 2), ShipOrientation.Vertical));
            Assert.True(gameMap.TryAddShip(ship3, new GameCell(9, 4), ShipOrientation.Vertical));
            Assert.True(gameMap.TryAddShip(ship4, new GameCell(8, 9), ShipOrientation.Vertical));

            Assert.True(gameMap.TryAddShip(ship5, new GameCell(2, 0), ShipOrientation.Horizontal));
            Assert.True(gameMap.TryAddShip(ship6, new GameCell(5, 0), ShipOrientation.Vertical));
            Assert.True(gameMap.TryAddShip(ship7, new GameCell(0, 6), ShipOrientation.Vertical));

            Assert.True(gameMap.TryAddShip(ship8, new GameCell(2, 4), ShipOrientation.Horizontal));
            Assert.True(gameMap.TryAddShip(ship9, new GameCell(5, 6), ShipOrientation.Horizontal));

            Assert.True(gameMap.TryAddShip(ship10, new GameCell(3, 9), ShipOrientation.Horizontal));
        }

        [Fact]
        public void Should_Detect_Collisions_Correctle()
        {
            var ship1 = new Ship(1);
            var ship2 = new Ship(1);
            var ship3 = new Ship(1);
            var ship4 = new Ship(1);

            var ship5 = new Ship(2);
            var ship6 = new Ship(2);
            var ship7 = new Ship(2);

            var ship8 = new Ship(3);
            var ship9 = new Ship(3);

            var ship10 = new Ship(4);

            int gameMapSize = 10;
            var gameMap = new GameMap(gameMapSize);

            Assert.True(gameMap.TryAddShip(ship1, new GameCell(9, 2), ShipOrientation.Vertical));
            Assert.True(gameMap.TryAddShip(ship2, new GameCell(2, 2), ShipOrientation.Vertical));
            Assert.False(gameMap.TryAddShip(ship3, new GameCell(9, 3), ShipOrientation.Vertical));
            Assert.True(gameMap.TryAddShip(ship4, new GameCell(8, 9), ShipOrientation.Vertical));

            Assert.True(gameMap.TryAddShip(ship5, new GameCell(0, 0), ShipOrientation.Vertical));
            Assert.True(gameMap.TryAddShip(ship6, new GameCell(5, 0), ShipOrientation.Vertical));
            Assert.True(gameMap.TryAddShip(ship7, new GameCell(0, 6), ShipOrientation.Vertical));

            Assert.True(gameMap.TryAddShip(ship8, new GameCell(2, 4), ShipOrientation.Horizontal));
            Assert.True(gameMap.TryAddShip(ship9, new GameCell(5, 6), ShipOrientation.Vertical));

            Assert.False(gameMap.TryAddShip(ship10, new GameCell(3, 9), ShipOrientation.Horizontal));
        }
    }
}