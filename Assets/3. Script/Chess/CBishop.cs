using System.Collections.Generic;
using UnityEngine;

public class CBishop : CChessPiece
{
    public override List<Vector2Int> GetAvailableMoves()
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        // 대각선 방향 이동
        int[,] directions = new int[,]
        {
            {1, 1}, {1, -1}, {-1, -1}, {-1, 1}
        };

        for (int i = 0; i < directions.GetLength(0); i++)
        {
            int dx = directions[i, 0];
            int dy = directions[i, 1];
            Vector2Int pos = new Vector2Int(currentPos.x + dx, currentPos.y + dy);

            while (pos.x >= 0 && pos.x < 8 && pos.y >= 0 && pos.y < 8)
            {
                CChessPiece piece = boardManager.GetPieceAt(pos.x, pos.y);
                if (piece == null)
                {
                    moves.Add(pos);
                }
                else
                {
                    if (piece.tag != this.tag)
                    {
                        moves.Add(pos);
                    }
                    break;
                }

                pos += new Vector2Int(dx, dy);
            }
        }

        return moves;
    }
}
