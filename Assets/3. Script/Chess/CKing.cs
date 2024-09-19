using System.Collections.Generic;
using UnityEngine;

public class CKing : CChessPiece
{
    public override List<Vector2Int> GetAvailableMoves()
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                Vector2Int movePos = new Vector2Int(currentPos.x + x, currentPos.y + y);
                if (IsValidMove(movePos))
                {
                    moves.Add(movePos);
                }
            }
        }

        return moves;
    }

    bool IsValidMove(Vector2Int pos)
    {
        // 체스판 범위 확인 및 다른 말의 위치 확인
        if (pos.x >= 0 && pos.x < 8 && pos.y >= 0 && pos.y < 8)
        {
            CChessPiece pieceAtTarget = boardManager.GetPieceAt(pos.x, pos.y);
            return pieceAtTarget == null || pieceAtTarget.tag != "PlayerKing";
        }
        return false;
    }
}
