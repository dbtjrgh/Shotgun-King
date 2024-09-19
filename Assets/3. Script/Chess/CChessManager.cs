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
    public CKing aiKing; // AI ŷ �߰� (�ʿ� ��)
    public List<CChessPiece> enemyPieces;
    public PlayerTurn currentTurn = PlayerTurn.Player;

    private CChessPiece selectedPiece = null;
    public LayerMask pieceLayerMask; // �÷��̾� ���� �ִ� ���̾�


    private void Start()
    {
        // ���� �ʱ�ȭ
        boardManager.InitializeBoard();

        // ������ �÷��̾� ŷ�� �� ������ ã�� �ʱ�ȭ
        InitializeGame();
    }

    void InitializeGame()
    {
        // ��� ������ �ùٸ� ��ġ�� ��ġ
        foreach (var piece in playerPieces)
        {
            Vector2Int startPos = GetTilePosition(piece.transform.position);
            if (startPos.x != -1 && startPos.y != -1)
            {
                boardManager.PlacePiece(piece, startPos.x, startPos.y);
            }
            else
            {
                Debug.LogError($"�� {piece.name}�� ��ġ�� ã�� �� �����ϴ�.");
            }
        }
    }



    Vector2Int GetTilePosition(Vector3 position)
    {
        float minDistance = Mathf.Infinity;
        Vector2Int closestTilePos = new Vector2Int(-1, -1);

        // 8x8 Ÿ���� ��ȸ�ϸ鼭 ���� ����� Ÿ���� ã��
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (boardManager.tiles[x, y] != null)
                {
                    Vector3 tilePos = boardManager.tiles[x, y].transform.position;

                    // Ÿ���� ��ġ�� ���� ��ġ ���� ���̰� 0.5 ������ �� �ش� Ÿ���� ����
                    if (Mathf.Abs(position.x - tilePos.x) < 0.6f && Mathf.Abs(position.z - tilePos.z) < 0.6f)
                    {
                        return new Vector2Int(x, y); // ��ġ�ϴ� Ÿ���� ã���� ��� ��ȯ
                    }
                }
            }
        }

        Debug.LogWarning("���� ��ġ�� �ش��ϴ� Ÿ���� ã�� �� �����ϴ�. ��ġ: " + position);
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
            Debug.Log("�÷��̾� üũ����Ʈ! AI �¸�!");
            EndGame("AI �¸�");
        }

        if (isAIInCheck && !CanKingEscape(aiKing, playerPieces))
        {
            Debug.Log("AI üũ����Ʈ! �÷��̾� �¸�!");
            EndGame("�÷��̾� �¸�");
        }
    }

    void EndGame(string result)
    {
        Debug.Log(result);
        // ���� ���� UI �Ǵ� ����� �ɼ� ǥ��
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
            // ������ �������� �����Ͽ� üũ ���θ� Ȯ��
            CChessPiece pieceAtMove = boardManager.GetPieceAt(move.x, move.y);
            if (pieceAtMove != null && pieceAtMove.tag == "Player") continue; // �÷��̾��� ���� �̵��� �� ���� ���

            // �ӽ÷� �� �̵�
            CChessPiece capturedPiece = pieceAtMove;
            if (capturedPiece != null)
            {
                capturedPiece.isCaptured = true;
            }
            boardManager.PlacePiece(king, move.x, move.y);

            // üũ �������� Ȯ��
            bool isStillInCheck = IsKingInCheck(king, enemyPieces);

            // ���� ����
            boardManager.PlacePiece(king, king.currentPos.x, king.currentPos.y);
            if (capturedPiece != null)
            {
                capturedPiece.isCaptured = false;
                boardManager.PlacePiece(capturedPiece, move.x, move.y);
            }

            if (!isStillInCheck)
            {
                return true; // Ż�� ������ �������� ����
            }
        }

        return false; // ��� �������� üũ ����
    }


    // CChessPiece Ŭ�������� ĸó �� ȣ��
    public void HandleCapture(CChessPiece capturedPiece)
    {
        CapturePiece(capturedPiece);
        // �߰����� ĸó ó�� ���� (��: UI ������Ʈ)
    }

    // CChessPiece.cs���� ĸó �� ȣ��
    protected void CapturePiece(CChessPiece piece)
    {
        piece.isCaptured = true;
        this.HandleCapture(piece); // CChessManager�� ĸó ó�� �޼ҵ� ȣ��
        Destroy(piece.gameObject); // ĸó�� �� ����

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
            Debug.Log("��");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, pieceLayerMask))
            {
                CChessPiece piece = hit.collider.GetComponent<CChessPiece>();
                if (piece != null && playerPieces.Contains(piece))
                {
                    SelectPiece(piece); // ���� ����
                }
            }
            else
            {
                // Ÿ���� Ŭ���� ��� ���õ� ���� �̵�
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
                                // �� �̵�
                                selectedPiece.MoveTo(targetPos);
                                boardManager.PlacePiece(selectedPiece, targetPos.x, targetPos.y);

                                selectedPiece = null; // �� ���� ����
                                EndTurn(); // �� ���� �� AI�� �Ѿ
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
        // ���õ� ���� ���� ǥ���ϴ� ���� (��: �� �ֺ��� ���̶���Ʈ ǥ��)
        // ������ ������Ʈ�� �°� �߰�
    }

    Vector2Int ParseTileName(string tileName)
    {
        // Ÿ�� �̸��� "Tile_x_y" �����̶�� ����
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
        yield return new WaitForSeconds(1f); // AI�� �����̱� ���� ��� ���

        // AI�� ��� ���� �� �ϳ��� �����Ͽ� ������
        foreach (CChessPiece aiPiece in enemyPieces)
        {
            List<Vector2Int> availableMoves = aiPiece.GetAvailableMoves();
            if (availableMoves.Count > 0)
            {
                Vector2Int bestMove = GetBestMoveTowardsKing(aiPiece, availableMoves);
                aiPiece.MoveTo(bestMove);
                boardManager.PlacePiece(aiPiece, bestMove.x, bestMove.y);
                break; // �� ���� �����̵��� ��
            }
        }

        currentTurn = PlayerTurn.Player; // AI �� ���� �� �÷��̾� ������ ��ȯ
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
            HandleAITurn(); // AI �� ó�� ����
        }
        else
        {
            currentTurn = PlayerTurn.Player;
        }
    }

    Vector2Int GetEnemyStartPos(CChessPiece piece)
    {
        // �� ���� �ʱ� ��ġ�� ���� (����� �⺻���� ���)
        return new Vector2Int(Random.Range(1, 8), Random.Range(1, 8)); // 1 ��� ��ǥ�� ����
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
