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

    // �̹� ���� �����ϴ� Ÿ�ϵ��� �̸����� ã�Ƽ� ���忡 ��ġ
    public void InitializeBoard()
    {
        for (int x = 1; x <= 8; x++)
        {
            for (int y = 1; y <= 8; y++)
            {
                // �� Ÿ���� �̸����� ã�Ƽ� �Ҵ� (�̸� ����: "Chess_Platform_1x1" ~ "Chess_Platform_8x8")
                string tileName = $"Chess_Platform_{x}x{y}";
                GameObject tile = GameObject.Find(tileName);
                if (tile != null)
                {
                    // Ÿ���� �迭�� ���� (�迭�� 0-based�̹Ƿ� x - 1, y - 1)
                    tiles[x - 1, y - 1] = tile;
                }
                else
                {
                    Debug.LogWarning($"{tileName}��(��) ã�� �� �����ϴ�.");
                }

                pieces[x - 1, y - 1] = null; // �ʱ⿡�� ���� ���ٰ� ����
            }
        }
    }

    public CChessPiece GetPieceAt(int x, int y)
    {
        // �迭 �ε��� ���� Ȯ��
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

    // ü�� ���� ���� Ÿ���� 3D ��ġ�� �̵���Ű��
    public void PlacePiece(CChessPiece piece, int x, int y)
    {
        // �迭 �ε��� ���� Ȯ��
        if (x >= 0 && x < 8 && y >= 0 && y < 8)
        {
            pieces[x, y] = piece;

            // Ÿ���� 3D ��ġ�� ������ ���� ��ġ ����
            if (tiles[x, y] != null)
            {
                Vector3 tilePosition = tiles[x, y].transform.position;
                piece.transform.position = new Vector3(tilePosition.x, tilePosition.y + 0.5f, tilePosition.z); // ���� ��ġ�� ����
                Debug.Log($"�� {piece.name}��(��) Ÿ�� ({x + 1}, {y + 1})�� ��ġ�߽��ϴ�.");
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
