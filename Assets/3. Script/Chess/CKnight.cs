using System.Collections.Generic;
using UnityEngine;

public class CKnight : CChessPiece
{
    public override List<Vector2Int> GetAvailableMoves()
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        int[,] directions = new int[,]
        {
            {1, 2}, {2, 1}, {2, -1}, {1, -2},
            {-1, -2}, {-2, -1}, {-2, 1}, {-1, 2}
        };

        for (int i = 0; i < directions.GetLength(0); i++)
        {
            Vector2Int movePos = new Vector2Int(currentPos.x + directions[i, 0], currentPos.y + directions[i, 1]);
            if (IsValidMove(movePos))
            {
                moves.Add(movePos);
            }
        }

        return moves;
    }

    bool IsValidMove(Vector2Int pos)
    {
        if (pos.x >= 0 && pos.x < 8 && pos.y >= 0 && pos.y < 8)
        {
            CChessPiece pieceAtTarget = boardManager.GetPieceAt(pos.x, pos.y);
            return pieceAtTarget == null || pieceAtTarget.tag != this.tag;
        }
        return false;
    }
}
