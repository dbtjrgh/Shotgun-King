using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBoardManager : MonoBehaviour
{
    private static readonly int xLength = 8;
    private static readonly int yLength = 8;
    private static readonly float xSize = 1.0f;
    private static readonly float ySize = 1.0f;

    static public Vector2 CoordinateToWorld(Vector2Int pos)
    {
        return Vector2.up * pos.y * ySize + Vector2.right * pos.x * xSize;
    }

    static public Vector2Int WorldToCoordinate(Vector2 pos)
    {
        Vector2Int vec = Vector2Int.zero;
        vec.x = Mathf.CeilToInt(pos.x + 0.5f) - 1;
        vec.y = Mathf.CeilToInt(pos.y + 0.5f) - 1;
        return vec;
    }
    private CChessManager[,] chessmans;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int coord = WorldToCoordinate(mousePos);
            List<CChessManager> li = new List<CChessManager>();
            for (int i = 0; i < xLength; i++)
                for (int j = 0; j < yLength; j++)
                    if (chessmans[i, j] != null)
                        li.Add(chessmans[i, j]);
            // 말이 움직이며 생기는 중복 선택을 막기 위해 말을 고르고 후에 호출한다.
            li.ForEach((chessman) => chessman.BoardSelect(coord));
        }
    }
}
