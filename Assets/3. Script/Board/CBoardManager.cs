using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CBoardManager : MonoBehaviour
{
    public static CBoardManager instance { get; set; }
    private bool[,] allowedMoves { get; set; }
    public CChessman[,] Chessmans { get; set; }
    private CChessman selectedChessman;

    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = 0.5f;

    private int selectionX = -1;
    private int selectionY = -1;
    private CCameraTransView cameraTransView;

    public List<GameObject> chessmanPrefabs;
    private List<GameObject> activeChessman;
    private Material previousMat;
    public Material selectedMat;

    private CChessman lastMovedChessman = null;  // Track last moved piece
    private Vector2 lastMoveTarget = Vector2.negativeInfinity; // Track last move target

    public int[] EnPassantMove { get; set; }

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

        if (isWhiteTurn)
        {
            AIPlay();  // AI가 백색 말들의 턴을 수행
        }
        else
        {
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
    }

    private void AIPlay()
    {
        List<(CChessman piece, int x, int y, bool isCapture)> validMoves = new List<(CChessman, int, int, bool)>();
        List<(CChessman piece, int x, int y)> captureMoves = new List<(CChessman, int, int)>();

        // 모든 백색 말을 순회하면서 유효한 이동을 찾음
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                CChessman c = Chessmans[i, j];  // 백색 말 가져오기
                if (c != null && c.isWhite)  // 백색 말인지 확인
                {
                    bool[,] possibleMoves = c.PossibleMove();  // 해당 말의 이동 가능한 좌표 가져오기

                    // 이동 가능한 모든 좌표를 확인
                    for (int x = 0; x < 8; x++)
                    {
                        for (int y = 0; y < 8; y++)
                        {
                            // 유효한 이동이면서 마지막 이동과 동일하지 않은 경우만 처리
                            if (possibleMoves[x, y] && !(c == lastMovedChessman && new Vector2(x, y) == lastMoveTarget))
                            {
                                // 이동하려는 좌표에 말이 있는지 확인
                                CChessman targetPiece = Chessmans[x, y];
                                bool isCapture = (targetPiece != null && !targetPiece.isWhite); // 해당 좌표에 흑색 말이 있으면 잡기 가능

                                if (isCapture)  // 잡을 수 있는 말이 있는 경우
                                {
                                    captureMoves.Add((c, x, y)); // 잡기 가능한 이동을 별도 리스트에 추가
                                }
                                else  // 그냥 이동만 가능한 경우
                                {
                                    validMoves.Add((c, x, y, isCapture));
                                }
                            }
                        }
                    }
                }
            }
        }

        // 1. 먼저 잡을 수 있는 말이 있으면 해당 이동을 우선 실행
        if (captureMoves.Count > 0)
        {
            var captureMove = captureMoves[UnityEngine.Random.Range(0, captureMoves.Count)];  // 잡을 수 있는 말 중 하나를 랜덤으로 선택
            selectedChessman = captureMove.piece;

            // 선택된 말의 이동 가능한 좌표를 계산
            allowedMoves = selectedChessman.PossibleMove();

            // 잡기 이동 실행
            MoveChessman(captureMove.x, captureMove.y);

            // 이번 이동을 기록하여 반복 이동 방지
            lastMovedChessman = selectedChessman;
            lastMoveTarget = new Vector2(captureMove.x, captureMove.y);
        }
        // 2. 잡을 말이 없으면 일반적인 이동을 실행
        else if (validMoves.Count > 0)
        {
            var randomMove = validMoves[UnityEngine.Random.Range(0, validMoves.Count)];  // 가능한 이동 중 하나를 랜덤으로 선택
            selectedChessman = randomMove.piece;

            // 선택된 말의 이동 가능한 좌표를 계산
            allowedMoves = selectedChessman.PossibleMove();

            // 이동 실행
            MoveChessman(randomMove.x, randomMove.y);

            // 이번 이동을 기록하여 반복 이동 방지
            lastMovedChessman = selectedChessman;
            lastMoveTarget = new Vector2(randomMove.x, randomMove.y);
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

        bool hasAtleastOneMove = false;
        allowedMoves = Chessmans[x, y].PossibleMove();

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (allowedMoves[i, j])
                {
                    hasAtleastOneMove = true;
                }
            }
        }

        if (!hasAtleastOneMove)
        {
            return;
        }

        selectedChessman = Chessmans[x, y];
        previousMat = selectedChessman.GetComponent<MeshRenderer>().material;
        selectedMat.mainTexture = previousMat.mainTexture;
        selectedChessman.GetComponent<MeshRenderer>().material = selectedMat;
        CBoardHighlights.instance.HighlightAllowedMoves(allowedMoves);
    }
    private void MoveChessman(int x, int y)
    {
        if (allowedMoves != null && allowedMoves[x, y])
        {
            CChessman c = Chessmans[x, y];

            if (c != null && c.isWhite != isWhiteTurn)
            {
                // Capture a piece
                if (c.GetType() == typeof(CKing))
                {
                    EndGame();
                    return;
                }
                activeChessman.Remove(c.gameObject);
                Destroy(c.gameObject);
            }

            // Move the selected piece to the new position
            Chessmans[selectedChessman.CurrentX, selectedChessman.CurrentY] = null; // Remove from the old position
            selectedChessman.transform.position = GetTileCenter(x, y); // Update the position
            selectedChessman.SetPosition(x, y);
            Chessmans[x, y] = selectedChessman; // Add to the new position

            isWhiteTurn = !isWhiteTurn;
        }

        if (selectedChessman != null)
        {
            // Restore the original material after the move
            MeshRenderer renderer = selectedChessman.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material = selectedChessman.originalMaterial;  // Restore original material
            }
        }

        CBoardHighlights.instance.Hidehighlights();
        selectedChessman = null; // Deselect after moving
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

        // Store the original material of the piece
        MeshRenderer renderer = go.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            Chessmans[x, y].originalMaterial = renderer.material;  // Save the original material
        }

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
        EnPassantMove = new int[2] { -1, -1 };

        // 화이트 팀 스폰 (z축을 가로로 변경)
        // 킹
        SpawnChessMan(0, 4, 0);
        // 퀸
        SpawnChessMan(1, 3, 0);
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

    private void EndGame()
    {
        if (isWhiteTurn)
        {
            Debug.Log("White team wins");
        }
        else
        {
            Debug.Log("Black team wins");
        }
        foreach (GameObject go in activeChessman)
        {
            Destroy(go);
        }
        isWhiteTurn = true;
        CBoardHighlights.instance.Hidehighlights();
        SpawnAllChessmans();
    }
}
