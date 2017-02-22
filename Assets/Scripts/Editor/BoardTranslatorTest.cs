using NUnit.Framework;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]

namespace Tests
{
    public class BoardTranslatorTest
    {

        [Test]
        public void TranslateRowTest()
        {
            var mapSettings = new Installer.Settings.MapSettings();
            mapSettings.rows = 8;
            mapSettings.columns = 10;
            var boardTranslator = new BoardTranslator(mapSettings);

            Assert.AreEqual(2, boardTranslator.translateToSceneRow(6));
            Assert.AreEqual(-4, boardTranslator.translateToSceneRow(0));
        }
        
        [Test]
        public void TranslateColumnTest()
        {
            var mapSettings = new Installer.Settings.MapSettings();
            mapSettings.rows = 10;
            mapSettings.columns = 8;
            var boardTranslator = new BoardTranslator(mapSettings);

            Assert.AreEqual(2, boardTranslator.translateToSceneColumn(6));
            Assert.AreEqual(-4, boardTranslator.translateToSceneColumn(0));
        }
    }
}
