using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CBoardManager : MonoBehaviour
{
    #region 변수
    public static CBoardManager instance { get; set; }
    public CChessman[,] chessMans { get; set; }
    private bool[,] allowedMoves { get; set; }
    private CChessman selectedChessman;
    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = 0.5f;
    private int selectionX = -1;
    private int selectionY = -1;
    public List<GameObject> chessmanPrefabs;
    public Material selectedMat;

    private CCameraTransView cameraTransView;
    private CPlayerShooting playerShooting;
    private CStageResultUI stageResultUI;
    private CStageDefeatUI stageDefeatUI;
    private List<GameObject> activeChessman;
    private Material previousMat;
    private CChessman lastMovedChessman = null;
    private Vector2 lastMoveTarget = Vector2.negativeInfinity;

    public int shotgunDamage;
    public int MaxshotgunDistance;
    public float shotAngle;

    public int[] EnPassantMove { get; set; }
    private bool kingSelected = false; // 킹이 선택되었는지 여부 확인
    public bool isWhiteTurn;
    public bool isTutorial;
    public int stageFloor;
    #endregion

    private void Awake()
    {
        if (isTutorial)
        {
            stageFloor = 0;
        }
        else
        {
            stageFloor = 1;
        }
        isWhiteTurn = false;
        cameraTransView = FindObjectOfType<CCameraTransView>();
        stageResultUI = FindAnyObjectByType<CStageResultUI>();
        stageDefeatUI = FindAnyObjectByType<CStageDefeatUI>();

    }
    private void Start()
    {
        instance = this;
        SpawnAllChessmans();
    }
    private void Update()
    {
        if (playerShooting == null)
        {
            playerShooting = FindObjectOfType<CPlayerShooting>();
        }
        if (playerShooting != null)
        {
            // 플레이어 능력치 관리
            playerShooting.shotgunDamage = shotgunDamage;
            playerShooting.MaxshotgunDistance = MaxshotgunDistance;
            playerShooting.shotAngle = shotAngle;
        }

        UpdateSelection();
        DrawChessboard();

        if (isWhiteTurn)  // AI 턴일 때
        {
            if (!IsInvoking(nameof(AIPlay))) // AI가 이미 실행 중이 아니면
            {
                Invoke(nameof(AIPlay), 3f);  // 2초 후 AIPlay 실행
            }
        }
        else if (IsInvoking(nameof(AIPlay)) && !isWhiteTurn)
        {
            isWhiteTurn = false;
            CancelInvoke(nameof(AIPlay));
        }
        else  // 흑색 플레이어의 턴 (플레이어 조작)
        {
            HandleBlackPlayerTurn();
        }
    }

    /// <summary>
    /// 플레이어 턴일 때 행동 할 수 있는 로직
    /// </summary>
    private void HandleBlackPlayerTurn()
    {
        if (Input.GetKeyDown(KeyCode.W) && !cameraTransView.isInTopView) // 1인칭 시점일 때, 첫 번째 W 입력 시
        {
            if (!kingSelected) // 아직 킹이 선택되지 않았을 때
            {
                for (int i = 0; i < 8; i++) // 모든 체스말을 순회하여 흑색 킹을 찾고 선택
                {
                    for (int j = 0; j < 8; j++)
                    {
                        CChessman chessman = chessMans[i, j];
                        if (chessman != null && chessman.GetType() == typeof(CKing) && !chessman.isWhite)
                        {
                            SelectChessman(i, j); // 흑색 킹을 선택
                            kingSelected = true; // 킹 선택 상태를 true로 변경
                            return;
                        }
                    }
                }
            }
            else // 두 번째 W 입력 시
            {
                if (selectionX >= 0 && selectionY >= 0 && selectedChessman != null)
                {
                    MoveChessman(selectionX, selectionY); // 킹 이동
                    kingSelected = false; // 킹 선택 해제
                }
            }
        }
        // 플레이어를 선택했고 스페이스를 눌렀을 때 선택된 플레이어 해제
        else if (Input.GetKeyDown(KeyCode.Space) && kingSelected) 
        {
            DeselectChessman();
            kingSelected = false;
        }
        // 탑뷰 시점일 때 플레이어를 했다면 선택, 선택이 이미 됐다면 이동
        else if (Input.GetMouseButtonDown(0) && cameraTransView.isInTopView)
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

    /// <summary>
    /// 선택된 체스말의 강조 표시를 제거하고 선택 해제
    /// </summary>
    private void DeselectChessman()
    {
        if (selectedChessman != null)
        {
            MeshRenderer renderer = selectedChessman.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material = selectedChessman.originalMaterial; // 원래 재질로 복원
            }
            CBoardHighlights.instance.HideHighlights(); // 강조 표시 숨김
            selectedChessman = null; // 선택된 체스말 초기화
        }
    }

    /// <summary>
    /// 상대 AI턴일 때 행동하는 로직
    /// </summary>
    public void AIPlay()
    {
        List<(CChessman piece, int x, int y, bool isCapture)> validMoves = new List<(CChessman, int, int, bool)>();
        List<(CChessman piece, int x, int y)> captureMoves = new List<(CChessman, int, int)>();

        // 모든 백색 말을 순회하면서 유효한 이동을 찾음
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                CChessman c = chessMans[i, j];  // 백색 말 가져오기
                if (c != null && c.isWhite)  // 백색 말인지 확인
                {
                    bool[,] possibleMoves = c.PossibleMove();  // 해당 말의 이동 가능한 좌표 가져오기

                    for (int x = 0; x < 8; x++) // 이동 가능한 모든 좌표를 확인
                    {
                        for (int y = 0; y < 8; y++)
                        {
                            // 유효한 이동이면서 마지막 이동과 동일하지 않은 경우만 처리
                            if (possibleMoves[x, y] && !(c == lastMovedChessman && new Vector2(x, y) == lastMoveTarget))
                            {
                                CChessman targetPiece = chessMans[x, y]; // 이동하려는 좌표에 말이 있는지 확인
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
            // 튜토리얼에서 재시작 여부 UI 활성화
            if (isTutorial)
            {
                stageFloor = 0;
                stageDefeatUI.defeatUI.SetActive(true);
            }
            // 메인 스테이지라면 1층으로 처리 및 패배 씬 로드
            else
            {
                stageFloor = 1;
                Invoke("ShowDefeatUI", 2f);
            }
            Debug.Log("화이트 윈");
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
    /// <summary>
    /// 체스 말이 선택됐다면 실행되는 함수
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void SelectChessman(int x, int y)
    {
        if (chessMans[x, y] == null)
        {
            return;
        }
        if (chessMans[x, y].isWhite != isWhiteTurn)
        {
            return;
        }

        bool hasAtleastOneMove = false;
        allowedMoves = chessMans[x, y].PossibleMove();

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

        selectedChessman = chessMans[x, y];
        previousMat = selectedChessman.GetComponent<MeshRenderer>().material;
        selectedMat.mainTexture = previousMat.mainTexture;
        selectedChessman.GetComponent<MeshRenderer>().material = selectedMat;
        CBoardHighlights.instance.HighlightAllowedMoves(allowedMoves);
    }
    /// <summary>
    /// 선택된 체스말을 움직이게 하는 함수
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void MoveChessman(int x, int y)
    {
        if (allowedMoves != null && allowedMoves[x, y])
        {

            CSoundManager.Instance.PlaySfx(6);
            // 플레이어 턴일 때 움직이는 거라면 재장전 함수 실행
            if (!isWhiteTurn)
            {
                playerShooting.MoveAndReload();
            }
            CChessman targetChessman = chessMans[x, y];

            // 기존 위치를 비우고 새 위치로 체스말 이동
            chessMans[selectedChessman.CurrentX, selectedChessman.CurrentY] = null;
            Vector3 targetPosition = GetTileCenter(x, y);

            // DoTween을 사용하여 부드럽게 이동
            selectedChessman.transform.DOMove(targetPosition, 0.5f).SetEase(Ease.InOutQuad); // 0.5초 동안 이동

            // 위치 업데이트
            selectedChessman.SetPosition(x, y);
            chessMans[x, y] = selectedChessman;
            // 목표 좌표에 적이 있으면 제거
            if (targetChessman != null && targetChessman.isWhite != isWhiteTurn)
            {
                // 
                if (targetChessman.GetType() == typeof(CKing))
                {
                    Invoke("EndGame", 2.0f);
                }
                activeChessman.Remove(targetChessman.gameObject);
                Destroy(targetChessman.gameObject);
            }
            isWhiteTurn = !isWhiteTurn;
        }

        // 체스말 선택 해제 및 강조 표시 제거
        if (selectedChessman != null)
        {
            MeshRenderer renderer = selectedChessman.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material = selectedChessman.originalMaterial;
            }
        }


        CBoardHighlights.instance.HideHighlights();
        selectedChessman = null;
    }

    /// <summary>
    /// 보드판 형성 로직
    /// </summary>
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
    /// <summary>
    /// 디버그용 보드판 체크 함수
    /// </summary>
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

    /// <summary>
    /// 체스판 위치 설정 함수
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public Vector3 GetTileCenter(int x, int z)
    {
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * x) + TILE_OFFSET; // x축에 값을 적용
        origin.z += (TILE_SIZE * z) + TILE_OFFSET; // z축에 값을 적용
        origin.y = 0.0f; // 체스판 높이

        // 타일 중앙 좌표 확인
        // Debug.Log($"Tile Center: X: {origin.x}, Z: {origin.z}");
        return origin;
    }

    /// <summary>
    /// 스테이지별 체스 말 형성 로직
    /// </summary>
    private void SpawnAllChessmans()
    {
        activeChessman = new List<GameObject>();
        chessMans = new CChessman[8, 8];
        EnPassantMove = new int[2] { -1, -1 };

        // SpawnChessMan ( 체스말 인덱스, x좌표, y좌표)
        switch (stageFloor)
        {
            case 0: // 킹1, 비숍1, 나이트1, 폰2
                shotgunDamage = 10;
                MaxshotgunDistance = 6;
                shotAngle = 55;
                SpawnChessMan(0, 4, 0); // 킹
                SpawnChessMan(3, 5, 0); // 비숍
                SpawnChessMan(4, 3, 0); // 나이트
                for (int i = 3; i < 5; i++) // 폰
                {
                    SpawnChessMan(5, i, 1); // z축으로 폰 배치
                }
                break;
            case 1: // 킹1, 비숍1, 나이트1, 폰4
                shotgunDamage = 4;
                MaxshotgunDistance = 5;
                shotAngle = 55;
                SpawnChessMan(0, 4, 0); // 킹
                SpawnChessMan(3, 5, 0); // 비숍
                SpawnChessMan(4, 3, 0); // 나이트
                for (int i = 3; i < 7; i++) // 폰
                {
                    SpawnChessMan(5, i, 1); // z축으로 폰 배치
                }
                break;

            case 2: // 킹1, 비숍1, 룩1, 나이트1, 폰5
                // 킹
                SpawnChessMan(0, 4, 0); // 킹
                SpawnChessMan(3, 5, 0); // 비숍
                SpawnChessMan(2, 0, 0); // 룩
                SpawnChessMan(4, 3, 0); // 나이트
                for (int i = 2; i < 7; i++) // 폰
                {
                    SpawnChessMan(5, i, 1); // z축으로 폰 배치
                }
                break;
            case 3: // 킹1, 비숍2, 룩1, 나이트2, 폰6
                SpawnChessMan(0, 4, 0); // 킹
                SpawnChessMan(3, 3, 0); // 비숍
                SpawnChessMan(3, 5, 0); // 비숍
                SpawnChessMan(2, 0, 0); // 룩
                SpawnChessMan(4, 1, 0); // 나이트
                SpawnChessMan(4, 6, 0); // 나이트 
                for (int i = 2; i < 8; i++) // 폰
                {
                    SpawnChessMan(5, i, 1); // z축으로 폰 배치
                }
                break;

            case 4: // 킹1, 퀸1, 비숍2, 룩1, 나이트2, 폰7
                SpawnChessMan(0, 4, 0); // 킹
                SpawnChessMan(1, 3, 0); // 퀸
                SpawnChessMan(2, 0, 0); // 룩
                SpawnChessMan(3, 2, 0); // 비숍
                SpawnChessMan(3, 5, 0); // 비숍
                SpawnChessMan(4, 1, 0); // 나이트
                SpawnChessMan(4, 6, 0); // 나이트
                for (int i = 1; i < 8; i++) // 폰
                {
                    SpawnChessMan(5, i, 1); // z축으로 폰 배치
                }
                break;
            case 5: // 킹1, 퀸1, 비숍2, 룩2, 나이트2, 폰8
                SpawnChessMan(0, 4, 0); // 킹
                SpawnChessMan(1, 3, 0); // 퀸
                SpawnChessMan(2, 0, 0); // 룩
                SpawnChessMan(2, 7, 0); // 룩
                SpawnChessMan(3, 2, 0); // 비숍
                SpawnChessMan(3, 5, 0); // 비숍
                SpawnChessMan(4, 1, 0); // 나이트
                SpawnChessMan(4, 6, 0); // 나이트
                for (int i = 0; i < 8; i++) // 폰
                {
                    SpawnChessMan(5, i, 1); // z축으로 폰 배치
                }
                break;
        }
        SpawnChessMan(6, 4, 7); // 블랙 킹 플레이어 스폰
    }
    /// <summary>
    /// 체스 말 스폰 함수
    /// </summary>
    /// <param name="index"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void SpawnChessMan(int index, int x, int y)
    {
        GameObject go = Instantiate(chessmanPrefabs[index], GetTileCenter(x, y), quaternion.identity) as GameObject;
        go.transform.SetParent(transform);

        chessMans[x, y] = go.GetComponent<CChessman>();
        chessMans[x, y].SetPosition(x, y);

        MeshRenderer renderer = go.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            chessMans[x, y].originalMaterial = renderer.material;
        }

        activeChessman.Add(go);
    }

    /// <summary>
    /// 킹이 죽었을때 호출되며 게임을 재시작하는 함수
    /// </summary>
    public void EndGame()
    {
        isWhiteTurn = false;
        foreach (GameObject go in activeChessman)
        {
            Destroy(go);
        }
        CBoardHighlights.instance.HideHighlights(); // 올바른 호출
        SpawnAllChessmans();
        // 킹이 죽었을 때 기준 2, 3, 4 스테이지일 때 결과 UI 함수 호출
        if (stageFloor != 0 && stageFloor != 1 && stageFloor != 6)
        {
            ShowResultUI();
        }
    }
    /// <summary>
    /// 스테이지를 깼을 때 결과 UI 함수
    /// </summary>
    public void ShowResultUI()
    {
        stageResultUI.resultUI.SetActive(true);
    }

    /// <summary>
    /// 패배했을 때 Invoke를 통해 불러오는 함수로 패배 씬으로 로드
    /// </summary>
    public void ShowDefeatUI()
    {
        if (stageFloor != 0)
        {
            SceneManager.LoadScene("DefeatScene");
        }
    }
}
