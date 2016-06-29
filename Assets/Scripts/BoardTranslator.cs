using System;
using Zenject;

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
    
    public float translateRow(float x)
    {
        return x - halfRows;
    }

    public float translateColumn(float y)
    {
        return y - halfColumns;
    }

}