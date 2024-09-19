using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerTurn
{
    Player,
    AI
}

public class CChessManager : MonoBehaviour
{
    public CBoardManager boardManager;
    public CKing playerKing;
    public List<CChessPiece> playerPieces;
    public CKing aiKing; // AI 킹 추가 (필요 시)
    public List<CChessPiece> enemyPieces;
    public PlayerTurn currentTurn = PlayerTurn.Player;

    private CChessPiece selectedPiece = null;
    public LayerMask pieceLayerMask; // 플레이어 말이 있는 레이어


    private void Start()
    {
        // 보드 초기화
        boardManager.InitializeBoard();

        // 씬에서 플레이어 킹과 적 말들을 찾아 초기화
        InitializeGame();
    }

    void InitializeGame()
    {
        // 모든 말들을 올바른 위치에 배치
        foreach (var piece in playerPieces)
        {
            Vector2Int startPos = GetTilePosition(piece.transform.position);
            if (startPos.x != -1 && startPos.y != -1)
            {
                boardManager.PlacePiece(piece, startPos.x, startPos.y);
            }
            else
            {
                Debug.LogError($"말 {piece.name}의 위치를 찾을 수 없습니다.");
            }
        }
    }



    Vector2Int GetTilePosition(Vector3 position)
    {
        float minDistance = Mathf.Infinity;
        Vector2Int closestTilePos = new Vector2Int(-1, -1);

        // 8x8 타일을 순회하면서 가장 가까운 타일을 찾음
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (boardManager.tiles[x, y] != null)
                {
                    Vector3 tilePos = boardManager.tiles[x, y].transform.position;

                    // 타일의 위치와 말의 위치 간의 차이가 0.5 이하일 때 해당 타일을 선택
                    if (Mathf.Abs(position.x - tilePos.x) < 0.6f && Mathf.Abs(position.z - tilePos.z) < 0.6f)
                    {
                        return new Vector2Int(x, y); // 일치하는 타일을 찾으면 즉시 반환
                    }
                }
            }
        }

        Debug.LogWarning("말의 위치에 해당하는 타일을 찾을 수 없습니다. 위치: " + position);
        return new Vector2Int(-1, -1);
    }




    private void Update()
    {
        if (currentTurn == PlayerTurn.Player)
        {
            HandlePlayerTurn();
        }
        else if (currentTurn == PlayerTurn.AI)
        {
            HandleAITurn();
        }

        CheckGameState();
    }
    void CheckGameState()
    {
        bool isPlayerInCheck = IsKingInCheck(playerKing, enemyPieces);
        bool isAIInCheck = IsKingInCheck(aiKing, playerPieces);

        if (isPlayerInCheck && !CanKingEscape(playerKing, enemyPieces))
        {
            Debug.Log("플레이어 체크메이트! AI 승리!");
            EndGame("AI 승리");
        }

        if (isAIInCheck && !CanKingEscape(aiKing, playerPieces))
        {
            Debug.Log("AI 체크메이트! 플레이어 승리!");
            EndGame("플레이어 승리");
        }
    }

    void EndGame(string result)
    {
        Debug.Log(result);
        // 게임 종료 UI 또는 재시작 옵션 표시
    }


    bool IsKingInCheck(CKing king, List<CChessPiece> enemyPieces)
    {
        foreach (CChessPiece enemy in enemyPieces)
        {
            if (enemy.GetAvailableMoves().Contains(king.currentPos))
            {
                return true;
            }
        }
        return false;
    }

    bool CanKingEscape(CKing king, List<CChessPiece> enemyPieces)
    {
        List<Vector2Int> availableMoves = king.GetAvailableMoves();
        foreach (Vector2Int move in availableMoves)
        {
            // 가상의 움직임을 적용하여 체크 여부를 확인
            CChessPiece pieceAtMove = boardManager.GetPieceAt(move.x, move.y);
            if (pieceAtMove != null && pieceAtMove.tag == "Player") continue; // 플레이어의 말을 이동할 수 없는 경우

            // 임시로 말 이동
            CChessPiece capturedPiece = pieceAtMove;
            if (capturedPiece != null)
            {
                capturedPiece.isCaptured = true;
            }
            boardManager.PlacePiece(king, move.x, move.y);

            // 체크 상태인지 확인
            bool isStillInCheck = IsKingInCheck(king, enemyPieces);

            // 원상 복구
            boardManager.PlacePiece(king, king.currentPos.x, king.currentPos.y);
            if (capturedPiece != null)
            {
                capturedPiece.isCaptured = false;
                boardManager.PlacePiece(capturedPiece, move.x, move.y);
            }

            if (!isStillInCheck)
            {
                return true; // 탈출 가능한 움직임이 있음
            }
        }

        return false; // 모든 움직임이 체크 상태
    }


    // CChessPiece 클래스에서 캡처 시 호출
    public void HandleCapture(CChessPiece capturedPiece)
    {
        CapturePiece(capturedPiece);
        // 추가적인 캡처 처리 로직 (예: UI 업데이트)
    }

    // CChessPiece.cs에서 캡처 시 호출
    protected void CapturePiece(CChessPiece piece)
    {
        piece.isCaptured = true;
        this.HandleCapture(piece); // CChessManager의 캡처 처리 메소드 호출
        Destroy(piece.gameObject); // 캡처된 말 제거

        if (playerPieces.Contains(piece))
        {
            playerPieces.Remove(piece);
        }
        if (enemyPieces.Contains(piece))
        {
            enemyPieces.Remove(piece);
        }
    }

    void HandlePlayerTurn()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("됨");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, pieceLayerMask))
            {
                CChessPiece piece = hit.collider.GetComponent<CChessPiece>();
                if (piece != null && playerPieces.Contains(piece))
                {
                    SelectPiece(piece); // 말을 선택
                }
            }
            else
            {
                // 타일을 클릭한 경우 선택된 말을 이동
                if (selectedPiece != null)
                {
                    Ray tileRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit tileHit;

                    if (Physics.Raycast(tileRay, out tileHit))
                    {
                        string tileName = tileHit.collider.gameObject.name;
                        if (tileName.StartsWith("Tile_"))
                        {
                            Vector2Int targetPos = ParseTileName(tileName);
                            if (selectedPiece.IsMoveValid(targetPos))
                            {
                                // 말 이동
                                selectedPiece.MoveTo(targetPos);
                                boardManager.PlacePiece(selectedPiece, targetPos.x, targetPos.y);

                                selectedPiece = null; // 말 선택 해제
                                EndTurn(); // 턴 종료 및 AI로 넘어감
                            }
                        }
                    }
                }
            }
        }
    }


    void SelectPiece(CChessPiece piece)
    {
        selectedPiece = piece;
        // 선택된 말을 강조 표시하는 로직 (예: 말 주변에 하이라이트 표시)
        // 구현은 프로젝트에 맞게 추가
    }

    Vector2Int ParseTileName(string tileName)
    {
        // 타일 이름이 "Tile_x_y" 형식이라고 가정
        string[] parts = tileName.Split('_');
        int x = int.Parse(parts[1]) - 1;
        int y = int.Parse(parts[2]) - 1;
        return new Vector2Int(x, y);
    }

    void HandleAITurn()
    {
        StartCoroutine(AIMoveRoutine());
    }

    IEnumerator AIMoveRoutine()
    {
        yield return new WaitForSeconds(1f); // AI가 움직이기 전에 잠시 대기

        // AI의 모든 말들 중 하나를 선택하여 움직임
        foreach (CChessPiece aiPiece in enemyPieces)
        {
            List<Vector2Int> availableMoves = aiPiece.GetAvailableMoves();
            if (availableMoves.Count > 0)
            {
                Vector2Int bestMove = GetBestMoveTowardsKing(aiPiece, availableMoves);
                aiPiece.MoveTo(bestMove);
                boardManager.PlacePiece(aiPiece, bestMove.x, bestMove.y);
                break; // 한 말만 움직이도록 함
            }
        }

        currentTurn = PlayerTurn.Player; // AI 턴 종료 후 플레이어 턴으로 전환
    }


    Vector2Int GetBestMoveTowardsKing(CChessPiece enemyPiece, List<Vector2Int> availableMoves)
    {
        Vector2Int kingPos = playerKing.currentPos;
        Vector2Int bestMove = availableMoves[0];
        float minDistance = Vector2Int.Distance(kingPos, bestMove);

        foreach (var move in availableMoves)
        {
            float distance = Vector2Int.Distance(kingPos, move);
            if (distance < minDistance)
            {
                bestMove = move;
                minDistance = distance;
            }
        }

        return bestMove;
    }

    public void EndTurn()
    {
        if (currentTurn == PlayerTurn.Player)
        {
            currentTurn = PlayerTurn.AI;
            HandleAITurn(); // AI 턴 처리 시작
        }
        else
        {
            currentTurn = PlayerTurn.Player;
        }
    }

    Vector2Int GetEnemyStartPos(CChessPiece piece)
    {
        // 적 말의 초기 위치를 설정 (현재는 기본값을 사용)
        return new Vector2Int(Random.Range(1, 8), Random.Range(1, 8)); // 1 기반 좌표로 설정
    }

    public void MoveEnemiesTowardsKing()
    {
        foreach (CChessPiece enemyPiece in enemyPieces)
        {
            List<Vector2Int> availableMoves = enemyPiece.GetAvailableMoves();
            Vector2Int bestMove = GetBestMoveTowardsKing(enemyPiece, availableMoves);
            enemyPiece.MoveTo(bestMove);
        }
    }
}
