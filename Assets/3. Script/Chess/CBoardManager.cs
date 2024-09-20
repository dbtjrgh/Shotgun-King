using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CBoardManager : MonoBehaviour
{
    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = 0.5f;

    private int selectionX = -1;
    private int selectionY = -1;
    private CCameraTransView cameraTransView;

    public List<GameObject> chessmanPrefabs;
    private List<GameObject> activeChessman = new List<GameObject>();

    private void Start()
    {
        SpawnAllChessmans();
    }
    private void Awake()
    {
        cameraTransView = FindObjectOfType<CCameraTransView>();
    }
    private void Update()
    {
        UpdateSelection();
        DrawChessboard();
    }

    private void UpdateSelection()
    {
        if (!Camera.main && !cameraTransView.isInTopView)
        {
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 50.0f, LayerMask.GetMask("ChessPlane")))
        {
            // 월드 좌표를 체스판 타일 좌표로 변환
            selectionX = (int)(hit.point.x / TILE_SIZE);
            selectionY = (int)(hit.point.z / TILE_SIZE); // y 대신 z를 사용해야 함 (월드 좌표에서 forward는 z축)
        }
        else
        {
            selectionX = -1;
            selectionY = -1;
        }
    }

    private void SpawnChessMan(int index, Vector3 position)
    {
        GameObject go = Instantiate(chessmanPrefabs[index], position, quaternion.identity) as GameObject;
        go.transform.SetParent(transform);
        activeChessman.Add(go);
    }
    private void SpawnAllChessmans()
    {
        activeChessman = new List<GameObject>();
        // 화이트 팀 스폰
        // 킹
        SpawnChessMan(0, GetTileCenter(3, 0));
        // 퀸
        SpawnChessMan(1, GetTileCenter(4, 0));
        // 룩
        SpawnChessMan(2, GetTileCenter(0, 0));
        SpawnChessMan(2, GetTileCenter(7, 0));
        // 비숍
        SpawnChessMan(3, GetTileCenter(2, 0));
        SpawnChessMan(3, GetTileCenter(5, 0));
        // 나이트
        SpawnChessMan(4, GetTileCenter(1, 0));
        SpawnChessMan(4, GetTileCenter(6, 0));
        // 폰
        for(int i = 0; i < 8; i++)
        {
            SpawnChessMan(5, GetTileCenter(i, 1));
        }
    }

    private Vector3 GetTileCenter(int x, int y)
    {
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * x) + TILE_OFFSET;
        origin.y += (TILE_SIZE * y) + TILE_OFFSET;
        return origin;
    }
    private void DrawChessboard()
    {
        Vector3 widthLine = Vector3.right * 8;
        Vector3 heightLine = Vector3.forward * 8;

        for (int i = 0; i <= 8; i++)
        {
            Vector3 start = Vector3.forward * i;
            Debug.DrawLine(start, start + widthLine);
            for (int j = 0; j <= 8; j++)
            {
                start = Vector3.right * j;
                Debug.DrawLine(start, start + heightLine);
            }
        }
        // Draw the selection
        if (selectionX >= 0 && selectionY >= 0)
        {
            Vector3 topLeft = Vector3.forward * selectionY + Vector3.right * selectionX;
            Vector3 topRight = Vector3.forward * selectionY + Vector3.right * (selectionX + 1);
            Vector3 bottomLeft = Vector3.forward * (selectionY + 1) + Vector3.right * selectionX;
            Vector3 bottomRight = Vector3.forward * (selectionY + 1) + Vector3.right * (selectionX + 1);

            Debug.DrawLine(topLeft, bottomRight); // 대각선 1
            Debug.DrawLine(topRight, bottomLeft); // 대각선 2
        }

    }
}
