using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CChessManager : MonoBehaviour
{
    public CBoardManager boardManager;
    public Vector2Int pos;

    public void MoveToCoordinate(Vector2Int destination)
    {
        boardManager.DeplaceChessman(pos);
        boardManager.PlaceChessman(this, destination);
        pos = destination;
        transform.position = (CBoardManager.CoordinateToWorld(destination));
    }

    public abstract void BoardSelect(Vector2Int coord);
}
