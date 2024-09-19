using System.Collections.Generic;
using UnityEngine;

public class CPawn : CChessPiece
{
    public bool isFirstMove = true;

    public override List<Vector2Int> GetAvailableMoves()
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        int direction = (this.tag == "PlayerPiece") ? 1 : -1; // 플레이어는 위로, AI는 아래로 이동

        // 전진 이동
        Vector2Int forwardPos = new Vector2Int(currentPos.x, currentPos.y + direction);
        if (IsMoveForwardValid(forwardPos))
        {
            moves.Add(forwardPos);

            // 첫 이동 시 두 칸 전진 가능
            if (isFirstMove)
            {
                Vector2Int doubleForwardPos = new Vector2Int(currentPos.x, currentPos.y + 2 * direction);
                if (IsMoveForwardValid(doubleForwardPos))
                {
                    moves.Add(doubleForwardPos);
                }
            }
        }

        // 공격 이동
        Vector2Int[] attackMoves = new Vector2Int[]
        {
            new Vector2Int(currentPos.x + 1, currentPos.y + direction),
            new Vector2Int(currentPos.x - 1, currentPos.y + direction)
        };

        foreach (var pos in attackMoves)
        {
            if (IsMoveAttackValid(pos))
            {
                moves.Add(pos);
            }
        }

        return moves;
    }

    bool IsMoveForwardValid(Vector2Int pos)
    {
        if (pos.x >= 0 && pos.x < 8 && pos.y >= 0 && pos.y < 8)
        {
            return boardManager.GetPieceAt(pos.x, pos.y) == null;
        }
        return false;
    }

    bool IsMoveAttackValid(Vector2Int pos)
    {
        if (pos.x >= 0 && pos.x < 8 && pos.y >= 0 && pos.y < 8)
        {
            CChessPiece piece = boardManager.GetPieceAt(pos.x, pos.y);
            return piece != null && piece.tag != this.tag;
        }
        return false;
    }

    public override void MoveTo(Vector2Int targetPos)
    {
        base.MoveTo(targetPos);
        if (isFirstMove)
        {
            isFirstMove = false;
        }
    }
}
