// using NUnit.Framework;
// using System;
//
// [assembly: InternalsVisibleTo("Tests")]
//
// namespace Tests
// {
//     public class DeviceCommunicatorTest
//     {
//         [Test]
//         public void TestEndLevelMessageSent()
//         {
//             using (ShimsContext.Create()) {
//               System.Fakes.ShimDeviceCommunicator.EndLevel = (status) => LevelCompleteStatus.SUCCESSFUL;
//               Assert.AreEqual(LevelCompleteStatus.SUCCESSFUL, DeviceCommunicator.EndLevel(LevelCompleteStatus.FAILED));
//             }
//         }
//     }
// }
