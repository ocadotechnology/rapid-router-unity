using NUnit.Framework;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]

namespace Tests
{
    public class LevelReaderTest {

        [Test]
        public void CanReadSimpleLevel() {
            Level level = LevelReader.ReadLevelFromFile(1);
            Assert.IsNotNull(level);
        }

        [Test]
        public void CanReadIntermediateLevel() {
            Level level = LevelReader.ReadLevelFromFile(65);
            Assert.IsNotNull(level);
        }

        [Test]
        public void CanReadAdvancedLevel() {
            Level level = LevelReader.ReadLevelFromFile(105);
            Assert.IsNotNull(level);
        }
    }

}