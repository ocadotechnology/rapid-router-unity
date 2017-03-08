using System;
using Zenject;
using UnityEngine;

public class BoardTranslator
{

    float halfRows;
    float halfColumns;

    Installer.Settings.MapSettings mapDimensions;

    public BoardTranslator(Installer.Settings.MapSettings mapDimensions) {
        this.mapDimensions = mapDimensions;
        halfRows = mapDimensions.rows / 2f;
        halfColumns = mapDimensions.columns / 2f;
    }
    
    public float translateToSceneRow(float x, bool drawingMap = false)
    {
        if (drawingMap == true) {
            return x - halfColumns;
        }
        return x - (halfColumns - 2f);
    }

    public float translateToSceneColumn(float y, bool drawingMap = false)
    {
        if (drawingMap == true) {
            return y - halfRows;
        }
        return y - (halfRows / 2f);
    }

    public Vector3 translateToSceneVector(Vector3 gameVector) {
        return new Vector3(translateToSceneRow(gameVector.x), translateToSceneColumn(gameVector.y), 0);
    }

    public Vector3 translateToGameVector(Vector3 sceneVector) {
        return new Vector3(sceneVector.x + (halfColumns - 2f), sceneVector.y + (halfRows / 2f));
    }

}