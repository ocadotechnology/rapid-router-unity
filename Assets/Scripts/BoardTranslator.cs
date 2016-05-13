using System;
using Zenject;

public class BoardTranslator
{

    float halfRows = Board.rows / 2f;
    float halfColumns = Board.columns / 2f;
    
    public float translateRow(float x)
    {
        return x - halfRows;
    }

    public float translateColumn(float y)
    {
        return y - halfColumns;
    }

}