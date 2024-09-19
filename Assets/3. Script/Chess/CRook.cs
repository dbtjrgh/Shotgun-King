using System.Collections.Generic;
using UnityEngine;

public class CRook : CChessPiece
{
    public override List<Vector2Int> GetAvailableMoves()
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        // 가로 이동
        for (int x = currentPos.x + 1; x < 8; x++)
        {
            if (AddMove(moves, new Vector2Int(x, currentPos.y))) break;
        }
        for (int x = currentPos.x - 1; x >= 0; x--)
        {
            if (AddMove(moves, new Vector2Int(x, currentPos.y))) break;
        }

        // 세로 이동
        for (int y = currentPos.y + 1; y < 8; y++)
        {
            if (AddMove(moves, new Vector2Int(currentPos.x, y))) break;
        }
        for (int y = currentPos.y - 1; y >= 0; y--)
        {
            if (AddMove(moves, new Vector2Int(currentPos.x, y))) break;
        }

        return moves;
    }

    private bool AddMove(List<Vector2Int> moves, Vector2Int pos)
    {
        if (pos.x < 0 || pos.x >= 8 || pos.y < 0 || pos.y >= 8)
            return true;

        CChessPiece piece = boardManager.GetPieceAt(pos.x, pos.y);
        if (piece == null)
        {
            moves.Add(pos);
            return false;
        }
        else
        {
            if (piece.tag != this.tag) // 다른 팀 말일 경우
            {
                moves.Add(pos);
            }
            return true; // 이동을 더 이상 진행하지 않음
        }
    }
}
