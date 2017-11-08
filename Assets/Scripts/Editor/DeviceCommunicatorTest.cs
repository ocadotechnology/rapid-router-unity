using NUnit.Framework;
using (ShimsContext.Create());

[assembly: InternalsVisibleTo("Tests")]

namespace Tests
{
    public class DeviceCommunicatorTest
    {
        [Test]
        public void TestEndLevelMessageSent()
        {
            ShimDeviceCommunicator.EndLevel = (status) => LevelCompleteStatus.SUCCESSFUL;
            Assert.AreEqual(LevelCompleteStatus.SUCCESSFUL, DeviceCommunicator.EndLevel(LevelCompleteStatus.FAILED));
        }
    }
}