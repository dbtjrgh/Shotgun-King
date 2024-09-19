using System.Collections.Generic;
using UnityEngine;

public class CBoardManager : MonoBehaviour
{
    public GameObject[,] tiles = new GameObject[8, 8];
    public CChessPiece[,] pieces = new CChessPiece[8, 8];

    private void Awake()
    {
        InitializeBoard();
    }

    // 이미 씬에 존재하는 타일들을 이름으로 찾아서 보드에 배치
    public void InitializeBoard()
    {
        for (int x = 1; x <= 8; x++)
        {
            for (int y = 1; y <= 8; y++)
            {
                // 각 타일을 이름으로 찾아서 할당 (이름 형식: "Chess_Platform_1x1" ~ "Chess_Platform_8x8")
                string tileName = $"Chess_Platform_{x}x{y}";
                GameObject tile = GameObject.Find(tileName);
                if (tile != null)
                {
                    // 타일을 배열에 저장 (배열은 0-based이므로 x - 1, y - 1)
                    tiles[x - 1, y - 1] = tile;
                }
                else
                {
                    Debug.LogWarning($"{tileName}을(를) 찾을 수 없습니다.");
                }

                pieces[x - 1, y - 1] = null; // 초기에는 말이 없다고 가정
            }
        }
    }

    public CChessPiece GetPieceAt(int x, int y)
    {
        // 배열 인덱스 범위 확인
        if (x >= 0 && x < 8 && y >= 0 && y < 8)
        {
            return pieces[x, y];
        }
        else
        {
            Debug.LogWarning($"GetPieceAt: Invalid index ({x}, {y})");
            return null;
        }
    }

    // 체스 말을 실제 타일의 3D 위치로 이동시키기
    public void PlacePiece(CChessPiece piece, int x, int y)
    {
        // 배열 인덱스 범위 확인
        if (x >= 0 && x < 8 && y >= 0 && y < 8)
        {
            pieces[x, y] = piece;

            // 타일의 3D 위치를 가져와 말의 위치 설정
            if (tiles[x, y] != null)
            {
                Vector3 tilePosition = tiles[x, y].transform.position;
                piece.transform.position = new Vector3(tilePosition.x, tilePosition.y + 0.5f, tilePosition.z); // 말의 위치를 설정
                Debug.Log($"말 {piece.name}을(를) 타일 ({x + 1}, {y + 1})에 배치했습니다.");
            }
            else
            {
                Debug.LogWarning($"Tile at ({x + 1}, {y + 1}) is null.");
            }
        }
        else
        {
            Debug.LogError($"PlacePiece: Invalid index ({x}, {y})");
        }
    }

}
