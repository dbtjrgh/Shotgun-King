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
        chessManager = FindObjectOfType<CChessManager>(); // ChessManager 인스턴스 찾기
    }

    public abstract List<Vector2Int> GetAvailableMoves();

    public virtual bool IsMoveValid(Vector2Int targetPos)
    {
        return GetAvailableMoves().Contains(targetPos);
    }

    // 'virtual'로 수정하여 자식 클래스에서 재정의할 수 있도록 허용
    public virtual void MoveTo(Vector2Int targetPos)
    {
        // 이동 전에 캡처 여부 확인
        CChessPiece targetPiece = boardManager.GetPieceAt(targetPos.x, targetPos.y);
        if (targetPiece != null)
        {
            CapturePiece(targetPiece);
        }

        // 말 이동
        boardManager.PlacePiece(this, targetPos.x, targetPos.y);
        currentPos = targetPos;
    }

    protected void CapturePiece(CChessPiece piece)
    {
        piece.isCaptured = true;
        chessManager.HandleCapture(piece); // CChessManager의 캡처 처리 메소드 호출
        Destroy(piece.gameObject); // 캡처된 말 제거
    }
}
