using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class CBoardManager : MonoBehaviour
{
    #region ����
    public static CBoardManager instance { get; set; }
    public CChessman[,] Chessmans { get; set; }
    private bool[,] allowedMoves { get; set; }
    private CChessman selectedChessman;

    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = 0.5f;

    private int selectionX = -1;
    private int selectionY = -1;
    private CCameraTransView cameraTransView;
    public CPlayerShooting playerShooting;
    private CStageResultUI stageResultUI;

    public int shotgunDamage;
    public int MaxshotgunDistance;
    public float shotAngle;

    public List<GameObject> chessmanPrefabs;
    public Material selectedMat;
    private List<GameObject> activeChessman;
    private Material previousMat;

    private CChessman lastMovedChessman = null; 
    private Vector2 lastMoveTarget = Vector2.negativeInfinity; 

    public int[] EnPassantMove { get; set; }

    private bool kingSelected = false; // ŷ�� ���õǾ����� ���� Ȯ��
    public bool isWhiteTurn = true;

    public int stageFloor;
    #endregion

    private void Awake()
    {
        stageFloor = 1;
        cameraTransView = FindObjectOfType<CCameraTransView>();
        stageResultUI = FindAnyObjectByType<CStageResultUI>();
        shotgunDamage = 4;
        MaxshotgunDistance = 5;
        shotAngle = 55;

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
        // �÷��̾� �ɷ�ġ ����
        playerShooting.shotgunDamage = shotgunDamage;
        playerShooting.MaxshotgunDistance = MaxshotgunDistance;
        playerShooting.shotAngle = shotAngle;

        UpdateSelection();
        DrawChessboard();

        if (isWhiteTurn)  // AI ���� ��
        {
            if (!IsInvoking(nameof(AIPlay))) // AI�� �̹� ���� ���� �ƴϸ�
            {
                Invoke(nameof(AIPlay), 2.0f);  // 2�� �� AIPlay ����
            }
        }
        else  // ��� �÷��̾��� �� (�÷��̾� ����)
        {
            HandleBlackPlayerTurn();
        }
    }


    private void HandleBlackPlayerTurn()
    {
        if (Input.GetKeyDown(KeyCode.W) && !cameraTransView.isInTopView) // ù ��° W �Է� ��
        {
            if (!kingSelected) // ���� ŷ�� ���õ��� �ʾ��� ��
            {
                // ��� ü������ ��ȸ�Ͽ� ��� ŷ�� ã�� ����
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        CChessman chessman = Chessmans[i, j];
                        if (chessman != null && chessman.GetType() == typeof(CKing) && !chessman.isWhite)
                        {
                            SelectChessman(i, j); // ��� ŷ�� ����
                            kingSelected = true; // ŷ ���� ���¸� true�� ����
                            return;
                        }
                    }
                }
            }
            else // �� ��° W �Է� ��
            {
                if (selectionX >= 0 && selectionY >= 0 && selectedChessman != null)
                {
                    MoveChessman(selectionX, selectionY); // ŷ �̵�
                    kingSelected = false; // ŷ ���� ����
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space) && kingSelected) // Space �Է� �� ���� ���
        {
            // ���õ� ŷ�� ������ ���
            DeselectChessman();
            kingSelected = false;
        }
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
        else if (Input.GetMouseButtonUp(0) && !cameraTransView.isInTopView)
        {
            // ü���� ���� ���� �� ���� ǥ�� ����
            if (selectedChessman != null)
            {
                MeshRenderer renderer = selectedChessman.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    renderer.material = selectedChessman.originalMaterial;
                }
            }

            CBoardHighlights.instance.Hidehighlights();
            selectedChessman = null;
        }
    }
    

    private void DeselectChessman()
    {
        // ���õ� ü������ ���� ǥ�ø� �����ϰ� ���� ����
        if (selectedChessman != null)
        {
            MeshRenderer renderer = selectedChessman.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material = selectedChessman.originalMaterial; // ���� ������ ����
            }
            CBoardHighlights.instance.Hidehighlights(); // ���� ǥ�� ����
            selectedChessman = null; // ���õ� ü���� �ʱ�ȭ
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
            // ������Ƿ� �������� �ʱ�ȭ �� �÷��̾� �й�
            stageFloor = 1;
            Debug.Log("ȭ��Ʈ ��");
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




    public void SelectChessman(int x, int y)
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
    public void MoveChessman(int x, int y)
    {
        if (allowedMoves != null && allowedMoves[x, y])
        {
            if (!isWhiteTurn)
            {
                Debug.Log("����");
                playerShooting.MoveAndReload();
            }
            CChessman targetChessman = Chessmans[x, y];

            // ���� ��ġ�� ���� �� ��ġ�� ü���� �̵�
            Chessmans[selectedChessman.CurrentX, selectedChessman.CurrentY] = null;
            Vector3 targetPosition = GetTileCenter(x, y);

            // DoTween�� ����Ͽ� �ε巴�� �̵�
            selectedChessman.transform.DOMove(targetPosition, 0.5f).SetEase(Ease.InOutQuad); // 0.5�� ���� �̵�

            // ��ġ ������Ʈ
            selectedChessman.SetPosition(x, y);
            Chessmans[x, y] = selectedChessman;
            // ��ǥ ��ǥ�� ���� ������ ����
            if (targetChessman != null && targetChessman.isWhite != isWhiteTurn)
            {
                if (targetChessman.GetType() == typeof(CKing))
                {
                    Invoke("EndGame", 2.0f);
                }
                activeChessman.Remove(targetChessman.gameObject);
                Destroy(targetChessman.gameObject);
            }
            isWhiteTurn = !isWhiteTurn;
        }

        // ü���� ���� ���� �� ���� ǥ�� ����
        if (selectedChessman != null)
        {
            MeshRenderer renderer = selectedChessman.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material = selectedChessman.originalMaterial;
            }
        }

        
        CBoardHighlights.instance.Hidehighlights();
        selectedChessman = null;
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


    public Vector3 GetTileCenter(int x, int z)
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

        // SpawnChessMan ( ü���� �ε���, x��ǥ, y��ǥ)
        // �������� �� ���� ����
        switch (stageFloor)
        {
            case 1: // ŷ1, ���1, ����Ʈ1, ��4
                // ŷ
                SpawnChessMan(0, 4, 0);
                // ���
                SpawnChessMan(3, 5, 0);
                // ����Ʈ
                SpawnChessMan(4, 3, 0);
                // ��
                for (int i = 3; i < 7; i++)
                {
                    SpawnChessMan(5, i, 1); // z������ �� ��ġ
                }
                break;

            case 2: // ŷ1, ���1, ��1, ����Ʈ1, ��5
                // ŷ
                SpawnChessMan(0, 4, 0);
                // ���
                SpawnChessMan(3, 5, 0);
                // ��
                SpawnChessMan(2, 0, 0);
                // ����Ʈ
                SpawnChessMan(4, 3, 0);
                // ��
                for (int i = 2; i < 7; i++)
                {
                    SpawnChessMan(5, i, 1); // z������ �� ��ġ
                }
                break;
            case 3: // ŷ1, ���2, ��1, ����Ʈ2, ��6
                // ŷ
                SpawnChessMan(0, 4, 0);
                // ���
                SpawnChessMan(3, 3, 0);
                SpawnChessMan(3, 5, 0);
                // ��
                SpawnChessMan(2, 0, 0);
                // ����Ʈ
                SpawnChessMan(4, 1, 0);
                SpawnChessMan(4, 6, 0);
                // ��
                for (int i = 2; i < 8; i++)
                {
                    SpawnChessMan(5, i, 1); // z������ �� ��ġ
                }
                break;

            case 4: // ŷ1, ��1, ���2, ��1, ����Ʈ2, ��7
                // ŷ
                SpawnChessMan(0, 4, 0);
                // ��
                SpawnChessMan(1, 3, 0);
                // ��
                SpawnChessMan(2, 0, 0);
                // ���
                SpawnChessMan(3, 2, 0);
                SpawnChessMan(3, 5, 0);
                // ����Ʈ
                SpawnChessMan(4, 1, 0);
                SpawnChessMan(4, 6, 0);
                // ��
                for (int i = 1; i < 8; i++)
                {
                    SpawnChessMan(5, i, 1); // z������ �� ��ġ
                }
                break;
            case 5: // ŷ1, ��1, ���2, ��2, ����Ʈ2, ��8
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
                break;
        }
        // �� �� �÷��̾� ����
        SpawnChessMan(6, 4, 7);
    }
    private void SpawnChessMan(int index, int x, int y)
    {
        GameObject go = Instantiate(chessmanPrefabs[index], GetTileCenter(x, y), quaternion.identity) as GameObject;
        go.transform.SetParent(transform);

        Chessmans[x, y] = go.GetComponent<CChessman>();
        Chessmans[x, y].SetPosition(x, y);

        MeshRenderer renderer = go.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            Chessmans[x, y].originalMaterial = renderer.material;
        }

        activeChessman.Add(go);
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

    public void EndGame()
    {
        ShowResultUI();
        foreach (GameObject go in activeChessman)
        {
            Destroy(go);
        }
        CBoardHighlights.instance.Hidehighlights();
        SpawnAllChessmans();
        isWhiteTurn = false;
    }

    public void ShowResultUI()
    {
        stageResultUI.resultUI.SetActive(true);
    }

}
