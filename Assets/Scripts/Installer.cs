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
        Container.Bind<BoardTranslator>().ToSingle();
        Container.Bind<RoadDrawer>().ToSingleGameObject();
        InstallSettings();
    }
    
    public void InstallSettings() {
        Container.Bind<Settings.RoadTiles>().ToSingleInstance(_settings.roadTiles);
        Container.Bind<Settings.FloorTiles>().ToSingleInstance(_settings.floorTiles);
        Container.Bind<Settings.MapSettings>().ToSingleInstance(_settings.mapSettings);
    }

    [Serializable]
    public class Settings
    {

        public MapSettings mapSettings;

        public RoadTiles roadTiles;

        public FloorTiles floorTiles;
        
        [Serializable]
        public class MapSettings {
            public int rows;
            public int columns;
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
        }
        
        [Serializable]
        public class FloorTiles {
            public GameObject grassTile;
        }
    }
}