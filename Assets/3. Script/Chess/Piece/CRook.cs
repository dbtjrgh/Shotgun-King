using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRook : CChessman
{
    public override bool[,] PossibleMove()
    {
        bool[,] r = new bool[8, 8];
        CChessman c;
        int i;

        // Right
        i = CurrentX;
        while (true)
        {
            i++;
            if (i >= 8)
                break;

            c = CBoardManager.instance.Chessmans[i, CurrentY];
            if (c == null)
            {
                r[i, CurrentY] = true;  // 빈 칸
            }
            else
            {
                if (isWhite != c.isWhite) // 다른 팀의 말일 때
                {
                    r[i, CurrentY] = true;
                }
                break; // 말이 있으면 경로 차단
            }
        }

        // Left
        i = CurrentX;
        while (true)
        {
            i--;
            if (i < 0)
                break;

            c = CBoardManager.instance.Chessmans[i, CurrentY];
            if (c == null)
            {
                r[i, CurrentY] = true;
            }
            else
            {
                if (isWhite != c.isWhite)
                {
                    r[i, CurrentY] = true;
                }
                break;
            }
        }

        // Up
        i = CurrentY;
        while (true)
        {
            i++;
            if (i >= 8)
                break;

            c = CBoardManager.instance.Chessmans[CurrentX, i];
            if (c == null)
            {
                r[CurrentX, i] = true;
            }
            else
            {
                if (isWhite != c.isWhite)
                {
                    r[CurrentX, i] = true;
                }
                break;
            }
        }

        // Down
        i = CurrentY;
        while (true)
        {
            i--;
            if (i < 0)
                break;

            c = CBoardManager.instance.Chessmans[CurrentX, i];
            if (c == null)
            {
                r[CurrentX, i] = true;
            }
            else
            {
                if (isWhite != c.isWhite)
                {
                    r[CurrentX, i] = true;
                }
                break;
            }
        }

        return r;
    }
}
