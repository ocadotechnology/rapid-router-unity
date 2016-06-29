using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;
using Road;
using Zenject;
using Moq;

[assembly: InternalsVisibleTo("Tests")]

namespace Tests
{
    public class RoadDrawerTest {

        DiContainer _container;
        BoardTranslator translatorMock;

        Vector3 Plane = Vector3.forward;

        [SetUp]
        public void Setup()
        {
            _container = new DiContainer();
            var mapSettings = new Installer.Settings.MapSettings();
            mapSettings.columns = 0;
            mapSettings.rows = 0;
            var translatorMock = new BoardTranslator(mapSettings);

            var roadTilesMock = new Installer.Settings.RoadTiles();
            roadTilesMock.cfcTile = new GameObject("CFC");
            roadTilesMock.deadEndRoadTile = new GameObject("DeadEnd");
            roadTilesMock.turnRoadTile = new GameObject("Turn");
            roadTilesMock.straightRoadTile = new GameObject("Straight");
            roadTilesMock.crossRoadTile = new GameObject("CrossRoad");
            roadTilesMock.tJunctionTile = new GameObject("TJunction");

            _container.BindInstance(translatorMock);
            _container.BindInstance(roadTilesMock);
            _container.Bind<RoadDrawer>().ToSingleGameObject();
        }

        [Test]
        public void SetupOriginTest() {

            var originNode = new OriginNode();
            originNode.coordinate = new int[2] { 10, 3 };
            originNode.direction = "E";
            var roadDrawer = _container.Resolve<RoadDrawer>();
            var originGameObject = roadDrawer.SetupOrigin(originNode);

            Assert.AreEqual("CFC(Clone)", originGameObject.name);
            Assert.AreEqual(new Vector3(10, 3, 0), originGameObject.transform.position);
        }

        [Test]
        public void SmallestRoadTest() {
            var originNode = new OriginNode();
            originNode.coordinate = new int[2] { 0, 0 };
            originNode.direction = "E";
            var nodes = SetupPathNodesWithCoordinates(new int[2][] { new int[2] { 0, 0 }, new int[2] { 1, 0 } });
            nodes[0].connectedNodes = new int[] { 1 };
            nodes[1].connectedNodes = new int[] { 0 };

            var roadDrawer = _container.Resolve<RoadDrawer>();
            GameObject[] gameObjects = roadDrawer.SetupRoadSegments(nodes);
            AssertThatRoadSegmentIsCorrect(gameObjects[0], "DeadEnd(Clone)", new Vector3(0, 0, 0), Direction.East);
            AssertThatRoadSegmentIsCorrect(gameObjects[1], "DeadEnd(Clone)", new Vector3(1, 0, 0), Direction.West);
        }

        [Test]
        public void StraightRoadTest()
        {
            OriginNode origin = SetupOriginNode("E", new int[2] { 0, 0 });
            PathNode[] nodes = SetupPathNodesWithCoordinates(new int[][] { new int[2] { 0, 0 }, new int[2] { 1, 0 }, new int[2] { 2, 0 }, new int[2] { 3, 0 } });
            nodes[0].connectedNodes = new int[] { 1 };
            nodes[1].connectedNodes = new int[] { 0, 2 };
            nodes[2].connectedNodes = new int[] { 1, 3 };
            nodes[3].connectedNodes = new int[] { 2 };

            var roadDrawer = _container.Resolve<RoadDrawer>();
            GameObject[] gameObjects = roadDrawer.SetupRoadSegments(nodes);
            AssertThatRoadSegmentIsCorrect(gameObjects[0], "DeadEnd(Clone)", new Vector3(0, 0, 0), Direction.East);
            AssertThatRoadSegmentIsCorrect(gameObjects[1], "Straight(Clone)", new Vector3(1, 0, 0), Direction.East);
            AssertThatRoadSegmentIsCorrect(gameObjects[2], "Straight(Clone)", new Vector3(2, 0, 0), Direction.East);
            AssertThatRoadSegmentIsCorrect(gameObjects[3], "DeadEnd(Clone)", new Vector3(3, 0, 0), Direction.West);
        }

        [Test]
        public void TurnLeftRoadTest()
        {
            OriginNode origin = SetupOriginNode("E", new int[2] { 0, 3 });
            PathNode[] nodes = SetupPathNodesWithCoordinates(new int[][] { new int[2] { 0, 3 }, new int[2] { 1, 3 }, new int[2] { 1, 4 }, new int[2] { 1, 5 } });
            nodes[0].connectedNodes = new int[] { 1 };
            nodes[1].connectedNodes = new int[] { 0, 2 };
            nodes[2].connectedNodes = new int[] { 1, 3 };
            nodes[3].connectedNodes = new int[] { 2 };

            var roadDrawer = _container.Resolve<RoadDrawer>();
            GameObject[] gameObjects = roadDrawer.SetupRoadSegments(nodes);
            AssertThatRoadSegmentIsCorrect(gameObjects[0], "DeadEnd(Clone)", new Vector3(0, 3, 0), Direction.East);
            AssertThatRoadSegmentIsCorrect(gameObjects[1], "Turn(Clone)", new Vector3(1, 3, 0), Direction.West);
            AssertThatRoadSegmentIsCorrect(gameObjects[2], "Straight(Clone)", new Vector3(1, 4, 0), Direction.North);
            AssertThatRoadSegmentIsCorrect(gameObjects[3], "DeadEnd(Clone)", new Vector3(1, 5, 0), Direction.South);
        }

        [Test]
        public void TurnRightRoadTest()
        {
            OriginNode origin = SetupOriginNode("E", new int[2] { 0, 3 });
            PathNode[] nodes = SetupPathNodesWithCoordinates(new int[][] { new int[2] { 0, 3 }, new int[2] { 1, 3 }, new int[2] { 1, 2 }, new int[2] { 1, 1 } });
            nodes[0].connectedNodes = new int[] { 1 };
            nodes[1].connectedNodes = new int[] { 0, 2 };
            nodes[2].connectedNodes = new int[] { 1, 3 };
            nodes[3].connectedNodes = new int[] { 2 };

            var roadDrawer = _container.Resolve<RoadDrawer>();
            GameObject[] gameObjects = roadDrawer.SetupRoadSegments(nodes);
            AssertThatRoadSegmentIsCorrect(gameObjects[0], "DeadEnd(Clone)", new Vector3(0, 3, 0), Direction.East);
            AssertThatRoadSegmentIsCorrect(gameObjects[1], "Turn(Clone)", new Vector3(1, 3, 0), Direction.South);
            AssertThatRoadSegmentIsCorrect(gameObjects[2], "Straight(Clone)", new Vector3(1, 2, 0), Direction.South);
            AssertThatRoadSegmentIsCorrect(gameObjects[3], "DeadEnd(Clone)", new Vector3(1, 1, 0), Direction.North);
        }

        [Test]
        public void TJunctionRoadTest() {
            OriginNode origin = SetupOriginNode("E", new int[2] { 0, 3 });
            PathNode[] nodes = SetupPathNodesWithCoordinates(new int[][] { new int[2] { 0, 3 }, new int[2] { 1, 3 }, new int[2] { 1, 4 }, new int[2] { 1, 2 } });
            nodes[0].connectedNodes = new int[] { 1 };
            nodes[1].connectedNodes = new int[] { 0, 2, 3 };
            nodes[2].connectedNodes = new int[] { 1 };
            nodes[3].connectedNodes = new int[] { 1 };

            var roadDrawer = _container.Resolve<RoadDrawer>();
            GameObject[] gameObjects = roadDrawer.SetupRoadSegments(nodes);
            AssertThatRoadSegmentIsCorrect(gameObjects[0], "DeadEnd(Clone)", new Vector3(0, 3, 0), Direction.East);
            AssertThatRoadSegmentIsCorrect(gameObjects[1], "TJunction(Clone)", new Vector3(1, 3, 0), Direction.West);
            AssertThatRoadSegmentIsCorrect(gameObjects[2], "DeadEnd(Clone)", new Vector3(1, 4, 0), Direction.South);
            AssertThatRoadSegmentIsCorrect(gameObjects[3], "DeadEnd(Clone)", new Vector3(1, 2, 0), Direction.North);
        }

        [Test]
        public void CrossRoadTest() {
            OriginNode origin = SetupOriginNode("E", new int[2] { 0, 3 });
            PathNode[] nodes = SetupPathNodesWithCoordinates(new int[][] { new int[2] { 0, 3 }, new int[2] { 1, 3 }, new int[2] { 1, 4 }, new int[2] { 1, 2 }, new int[] { 2, 3 } });
            nodes[0].connectedNodes = new int[] { 1 };
            nodes[1].connectedNodes = new int[] { 0, 2, 3, 4 };
            nodes[2].connectedNodes = new int[] { 1 };
            nodes[3].connectedNodes = new int[] { 1 };
            nodes[4].connectedNodes = new int[] { 1 };

            var roadDrawer = _container.Resolve<RoadDrawer>();
            GameObject[] gameObjects = roadDrawer.SetupRoadSegments(nodes);
            AssertThatRoadSegmentIsCorrect(gameObjects[0], "DeadEnd(Clone)", new Vector3(0, 3, 0), Direction.East);
            AssertThatRoadSegmentIsCorrect(gameObjects[1], "CrossRoad(Clone)", new Vector3(1, 3, 0), Direction.North);
            AssertThatRoadSegmentIsCorrect(gameObjects[2], "DeadEnd(Clone)", new Vector3(1, 4, 0), Direction.South);
            AssertThatRoadSegmentIsCorrect(gameObjects[3], "DeadEnd(Clone)", new Vector3(1, 2, 0), Direction.North);
            AssertThatRoadSegmentIsCorrect(gameObjects[4], "DeadEnd(Clone)", new Vector3(2, 3, 0), Direction.West);
        }

        private void AssertThatRoadSegmentIsCorrect(GameObject roadSegment, string name, Vector3 position, Direction direction) {
            Assert.AreEqual(name, roadSegment.name);
            Assert.AreEqual(position, roadSegment.transform.position);
            Assert.AreEqual(NormaliseAngle((float)direction), NormaliseAngle(roadSegment.transform.eulerAngles.z));
        }

        private PathNode[] SetupPathNodesWithCoordinates(int[][] coordinates) {
            PathNode[] nodes = new PathNode[coordinates.Length];
            for (int i = 0; i < coordinates.Length; i++) {
                int[] coordinate = coordinates[i];
                PathNode node = new PathNode();
                node.coordinate = coordinate;
                nodes[i] = node;
            }
            return nodes;
        }

        private OriginNode SetupOriginNode(string direction, int[] coordinate) {
            OriginNode origin = new OriginNode();
            origin.coordinate = coordinate;
            origin.direction = direction;
            return origin;
        }

        private float NormaliseAngle(float angle) {
            float nomralisedAngle = angle;
            while (angle < 0) {
                angle += 360;
            }
            while (angle > 360) {
                angle -= 360;
            }
            return angle;
        }
    }
}