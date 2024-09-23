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
            AIPlay();  // AI�� ��� ������ ���� ����
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

        // ��� ��� ���� ��ȸ�ϸ鼭 ��ȿ�� �̵��� ã��
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                CChessman c = Chessmans[i, j];  // ��� �� ��������
                if (c != null && c.isWhite)  // ��� ������ Ȯ��
                {
                    bool[,] possibleMoves = c.PossibleMove();  // �ش� ���� �̵� ������ ��ǥ ��������

                    // �̵� ������ ��� ��ǥ�� Ȯ��
                    for (int x = 0; x < 8; x++)
                    {
                        for (int y = 0; y < 8; y++)
                        {
                            // ��ȿ�� �̵��̸鼭 ������ �̵��� �������� ���� ��츸 ó��
                            if (possibleMoves[x, y] && !(c == lastMovedChessman && new Vector2(x, y) == lastMoveTarget))
                            {
                                // �̵��Ϸ��� ��ǥ�� ���� �ִ��� Ȯ��
                                CChessman targetPiece = Chessmans[x, y];
                                bool isCapture = (targetPiece != null && !targetPiece.isWhite); // �ش� ��ǥ�� ��� ���� ������ ��� ����

                                if (isCapture)  // ���� �� �ִ� ���� �ִ� ���
                                {
                                    captureMoves.Add((c, x, y)); // ��� ������ �̵��� ���� ����Ʈ�� �߰�
                                }
                                else  // �׳� �̵��� ������ ���
                                {
                                    validMoves.Add((c, x, y, isCapture));
                                }
                            }
                        }
                    }
                }
            }
        }

        // 1. ���� ���� �� �ִ� ���� ������ �ش� �̵��� �켱 ����
        if (captureMoves.Count > 0)
        {
            var captureMove = captureMoves[UnityEngine.Random.Range(0, captureMoves.Count)];  // ���� �� �ִ� �� �� �ϳ��� �������� ����
            selectedChessman = captureMove.piece;

            // ���õ� ���� �̵� ������ ��ǥ�� ���
            allowedMoves = selectedChessman.PossibleMove();

            // ��� �̵� ����
            MoveChessman(captureMove.x, captureMove.y);

            // �̹� �̵��� ����Ͽ� �ݺ� �̵� ����
            lastMovedChessman = selectedChessman;
            lastMoveTarget = new Vector2(captureMove.x, captureMove.y);
        }
        // 2. ���� ���� ������ �Ϲ����� �̵��� ����
        else if (validMoves.Count > 0)
        {
            var randomMove = validMoves[UnityEngine.Random.Range(0, validMoves.Count)];  // ������ �̵� �� �ϳ��� �������� ����
            selectedChessman = randomMove.piece;

            // ���õ� ���� �̵� ������ ��ǥ�� ���
            allowedMoves = selectedChessman.PossibleMove();

            // �̵� ����
            MoveChessman(randomMove.x, randomMove.y);

            // �̹� �̵��� ����Ͽ� �ݺ� �̵� ����
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
            selectionY = (int)(hit.point.z / TILE_SIZE); // z���� ������ forward ����

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
        origin.x += (TILE_SIZE * x) + TILE_OFFSET; // x�࿡ ���� ����
        origin.z += (TILE_SIZE * z) + TILE_OFFSET; // z�࿡ ���� ����
        origin.y = 0.0f; // ü���� ����

        // Ÿ�� �߾� ��ǥ Ȯ��
        // Debug.Log($"Tile Center: X: {origin.x}, Z: {origin.z}");
        return origin;
    }



    private void SpawnAllChessmans()
    {
        activeChessman = new List<GameObject>();
        Chessmans = new CChessman[8, 8];
        EnPassantMove = new int[2] { -1, -1 };

        // ȭ��Ʈ �� ���� (z���� ���η� ����)
        // ŷ
        SpawnChessMan(0, 4, 0);
        // ��
        SpawnChessMan(1, 3, 0);
        // ��
        SpawnChessMan(2, 0, 0);
        SpawnChessMan(2, 7, 0);
        // ���
        SpawnChessMan(3, 2, 0);
        SpawnChessMan(3, 5, 0);
        // ����Ʈ
        SpawnChessMan(4, 1, 0);
        SpawnChessMan(4, 6, 0);
        // ��
        for (int i = 0; i < 8; i++)
        {
            SpawnChessMan(5, i, 1); // z������ �� ��ġ
        }

        // �� �� �÷��̾� ����
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

            Debug.DrawLine(topLeft, bottomRight); // �밢�� 1
            Debug.DrawLine(topRight, bottomLeft); // �밢�� 2
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
