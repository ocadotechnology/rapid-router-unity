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

            Assert.AreEqual(2, boardTranslator.translateRow(6));
            Assert.AreEqual(-4, boardTranslator.translateRow(0));
        }
        
        [Test]
        public void TranslateColumnTest()
        {
            var mapSettings = new Installer.Settings.MapSettings();
            mapSettings.rows = 10;
            mapSettings.columns = 8;
            var boardTranslator = new BoardTranslator(mapSettings);

            Assert.AreEqual(2, boardTranslator.translateColumn(6));
            Assert.AreEqual(-4, boardTranslator.translateColumn(0));
        }
    }
}
