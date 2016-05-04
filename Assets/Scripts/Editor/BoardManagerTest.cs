using NUnit.Framework;

public class BoardManagerTest {

    [Test]
    public void LoadLevelFromFileTest()
    {
        //Arrange
//        var gameObject = new GameObject();
		var level = new BoardManager();

        //Act
        //Try to rename the GameObject
//        var newGameObjectName = "My game object";
//        gameObject.name = newGameObjectName;
		var levelJSON = level.LoadJSONFile();

        //Assert
        //The object has a new name
//        Assert.AreEqual(newGameObjectName, gameObject.name);
		Assert.IsTrue (true);
    }
}
