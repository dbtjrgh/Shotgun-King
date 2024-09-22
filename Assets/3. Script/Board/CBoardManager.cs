using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CBoardManager : MonoBehaviour
{
    public static CBoardManager instance {  get; set; }
    private bool[,] allowedMoves { get; set; }
    public CChessman[,] Chessmans { get; set; }
    private CChessman selectedChessman;

    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = 0.5f;

    private int selectionX = -1;
    private int selectionY = -1;
    private CCameraTransView cameraTransView;

    public List<GameObject> chessmanPrefabs;
    private List<GameObject> activeChessman = new List<GameObject>();

    public bool isWhiteTurn = true;

    private void Awake()
    {
        cameraTransView = FindObjectOfType<CCameraTransView>();
    }
    private void Start()
    {
        instance = this;
        SpawnAllChessmans();
    }
    private void Update()
    {
        UpdateSelection();
        DrawChessboard();

        if (Input.GetMouseButtonDown(0))
        {
            if (selectionX >= 0 && selectionY >= 0)
            {
                if (selectedChessman == null)
                {
                    SelectChessman(selectionX, selectionY);
                }
                else
                {
                    MoveChessman(selectionX, selectionY);
                }
            }
        }
    }
    private void SelectChessman(int x, int y)
    {
        if (Chessmans[x, y] == null)
        {
            return;
        }
        if (Chessmans[x, y].isWhite != isWhiteTurn)
        {
            return;
        }
        allowedMoves = Chessmans[x, y].PossibleMove();
        selectedChessman = Chessmans[x, y];
        CBoardHighlights.instance.HighlightAllowedMoves(allowedMoves);
    }
    private void MoveChessman(int x, int y)
    {
        if (allowedMoves[x, y])
        {
            CChessman c = Chessmans[x, y];
            if(c != null&&c.isWhite != isWhiteTurn)
            {
                // Capture a piece

                // If it is the King
                if(c.GetType() == typeof(CKing))
                {
                    return;
                }
                activeChessman.Remove(c.gameObject);
                Destroy(c.gameObject);
            }
            Chessmans[selectedChessman.CurrentX, selectedChessman.CurrentY] = null; // 이전 위치에서 체스말 삭제
            selectedChessman.transform.position = GetTileCenter(x, y); // 체스말 위치 갱신
            selectedChessman.SetPosition(x, y);
            Chessmans[x, y] = selectedChessman; // 새 위치에 체스말 등록
            isWhiteTurn = !isWhiteTurn;
        }
        CBoardHighlights.instance.Hidehighlights();
        selectedChessman = null; // 선택 해제
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
            selectionX = (int)(hit.point.x / TILE_SIZE);
            selectionY = (int)(hit.point.z / TILE_SIZE); // z축이 월드의 forward 축임

        }
        else
        {
            selectionX = -1;
            selectionY = -1;
        }
    }


    private void SpawnChessMan(int index, int x, int y)
    {
        GameObject go = Instantiate(chessmanPrefabs[index], GetTileCenter(x, y), quaternion.identity) as GameObject;
        go.transform.SetParent(transform);
        Chessmans[x, y] = go.GetComponent<CChessman>();
        Chessmans[x, y].SetPosition(x, y);
        activeChessman.Add(go);
    }
    private Vector3 GetTileCenter(int x, int z)
    {
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * x) + TILE_OFFSET; // x축에 값을 적용
        origin.z += (TILE_SIZE * z) + TILE_OFFSET; // z축에 값을 적용
        origin.y = 0.0f; // 체스판 높이

        // 타일 중앙 좌표 확인
        // Debug.Log($"Tile Center: X: {origin.x}, Z: {origin.z}");
        return origin;
    }



    private void SpawnAllChessmans()
    {
        activeChessman = new List<GameObject>();
        Chessmans = new CChessman[8, 8];

        // 화이트 팀 스폰 (z축을 가로로 변경)
        // 킹
        SpawnChessMan(0, 3, 0);
        // 퀸
        SpawnChessMan(1, 4, 0);
        // 룩
        SpawnChessMan(2, 0, 0);
        SpawnChessMan(2, 7, 0);
        // 비숍
        SpawnChessMan(3, 2, 0);
        SpawnChessMan(3, 5, 0);
        // 나이트
        SpawnChessMan(4, 1, 0);
        SpawnChessMan(4, 6, 0);
        // 폰
        for (int i = 0; i < 8; i++)
        {
            SpawnChessMan(5, i, 1); // z축으로 폰 배치
        }

        // 블랙 팀 플레이어 스폰
        SpawnChessMan(6, 4, 7);
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
