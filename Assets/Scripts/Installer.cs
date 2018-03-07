using System;
using Zenject;
using Road;
using UnityEngine;

public class Installer : MonoInstaller
{
    [SerializeField]
    Settings _settings = null;

    public override void InstallBindings()
    {
        Container.Bind<IInitializable>().ToSingle<GameManager>();
        Container.Bind<GameManager>().ToSingle();
        Container.Bind<BoardManager>().ToSingleGameObject();
        Container.Bind<RoadBuilder>().ToSingleGameObject();
        Container.Bind<RoadDrawer>().ToSingleGameObject();
        Container.Bind<DecorDrawer>().ToSingleGameObject();
        Container.Bind<HouseDrawer>().ToSingleGameObject();
        InstallSettings();
    }

    public void InstallSettings() {
        Container.Bind<Settings.RoadTiles>().ToSingleInstance(_settings.roadTiles);
        Container.Bind<Settings.FloorTiles>().ToSingleInstance(_settings.floorTiles);
        Container.Bind<Settings.MapSettings>().ToSingleInstance(_settings.mapSettings);
        Container.Bind<Settings.DecorationTiles>().ToSingleInstance(_settings.decorationTiles);
    }

    [Serializable]
    public class Settings
    {

        public MapSettings mapSettings;

        public RoadTiles roadTiles;

        public FloorTiles floorTiles;

        public DecorationTiles decorationTiles;

        [Serializable]
        public class MapSettings {
            public int rows;
            public int columns;
            public int horizontalOffset;
            public int verticalOffset;
        }

        [Serializable]
        public class RoadTiles
        {
            public GameObject straightRoadTile;
            public GameObject deadEndRoadTile;
            public GameObject turnRoadTile;
            public GameObject tJunctionTile;
            public GameObject crossRoadTile;
            public GameObject cfcTile;
            public GameObject houseTile;
        }

        [Serializable]
        public class FloorTiles {
            public GameObject grassTile;
        }

        [Serializable]
        public class DecorationTiles {
            public GameObject tree1Tile;
            public GameObject tree2Tile;
            public GameObject bushTile;
            public GameObject pondTile;
        }
    }
}
