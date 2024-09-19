using System.Collections.Generic;
using UnityEngine;

public class CQueen : CChessPiece
{
    public override List<Vector2Int> GetAvailableMoves()
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        // 룩과 비숍의 이동을 모두 포함
        // 룩 방향
        int[,] rookDirections = new int[,]
        {
            {1, 0}, {-1, 0}, {0, 1}, {0, -1}
        };

        for (int i = 0; i < rookDirections.GetLength(0); i++)
        {
            int dx = rookDirections[i, 0];
            int dy = rookDirections[i, 1];
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

        // 비숍 방향
        int[,] bishopDirections = new int[,]
        {
            {1, 1}, {1, -1}, {-1, -1}, {-1, 1}
        };

        for (int i = 0; i < bishopDirections.GetLength(0); i++)
        {
            int dx = bishopDirections[i, 0];
            int dy = bishopDirections[i, 1];
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
