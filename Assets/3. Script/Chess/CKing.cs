using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class CKing : CPlayerMoveableChess
{
    private void Start()
    {
        MoveToCoordinate(new Vector2Int(3, 2));
        CBoardManager.PlaceChessman(this, pos);
    }

    public override List<Vector2Int> MoveableCoord()
    {
        List<Vector2Int> li = new List<Vector2Int>();
        int[] x = new int[3] { 1, 0, -1 };
        int[] y = new int[3] { 1, 0, -1 };
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                if (CBoardManager.IsCoordAvailable(pos + Vector2Int.right * x[i] + Vector2Int.up * y[j]))
                    li.Add(pos + Vector2Int.right * x[i] + Vector2Int.up * y[j]);
        return li;
    }
}
