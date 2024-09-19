using System.Collections.Generic;
using UnityEngine;

public abstract class CChessPiece : MonoBehaviour
{
    public Vector2Int currentPos;
    protected CBoardManager boardManager;
    public bool isCaptured = false;
    protected CChessManager chessManager;

    public virtual void Initialize(CBoardManager manager, Vector2Int pos)
    {
        currentPos = pos;
        boardManager = manager;
        chessManager = FindObjectOfType<CChessManager>(); // ChessManager �ν��Ͻ� ã��
    }

    public abstract List<Vector2Int> GetAvailableMoves();

    public virtual bool IsMoveValid(Vector2Int targetPos)
    {
        return GetAvailableMoves().Contains(targetPos);
    }

    // 'virtual'�� �����Ͽ� �ڽ� Ŭ�������� �������� �� �ֵ��� ���
    public virtual void MoveTo(Vector2Int targetPos)
    {
        // �̵� ���� ĸó ���� Ȯ��
        CChessPiece targetPiece = boardManager.GetPieceAt(targetPos.x, targetPos.y);
        if (targetPiece != null)
        {
            CapturePiece(targetPiece);
        }

        // �� �̵�
        boardManager.PlacePiece(this, targetPos.x, targetPos.y);
        currentPos = targetPos;
    }

    protected void CapturePiece(CChessPiece piece)
    {
        piece.isCaptured = true;
        chessManager.HandleCapture(piece); // CChessManager�� ĸó ó�� �޼ҵ� ȣ��
        Destroy(piece.gameObject); // ĸó�� �� ����
    }
}
