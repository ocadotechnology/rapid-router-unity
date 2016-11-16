using NUnit.Framework;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]

namespace Tests
{
    public class BoardManagerTest
    {

        [Test]
        public void LoadLevelFromFileTest()
        {
            //Arrange
            //        var gameObject = new GameObject();
            // var level = new BoardManager();

            //Act
            //Try to rename the GameObject
            //        var newGameObjectName = "My game object";
            //        gameObject.name = newGameObjectName;
            // var levelJSON = level.LoadJSONFile();

            //Assert
            //The object has a new name
            //        Assert.AreEqual(newGameObjectName, gameObject.name);

			// Need to assert that:
			//	CFC origin has been placed correctly
			//	Destination node placed correctly
			//	Tiles placed correctly and of right type
            Assert.IsTrue(true);
        }
    }
}
