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
            {
                break;
            }
            c = CBoardManager.instance.Chessmans[i, CurrentY];
            if (c == null)
            {
                r[i, CurrentY] = true;
            }
            else
            {
                if (c.isWhite != isWhite)
                {
                    r[i, CurrentY] = true;
                }
                break;
            }
        }

        // Left
        i = CurrentX;
        while (true)
        {
            i--;
            if (i < 0)
            {
                break;
            }
            c = CBoardManager.instance.Chessmans[i, CurrentY];
            if (c == null)
            {
                r[i, CurrentY] = true;
            }
            else
            {
                if (c.isWhite != isWhite)
                {
                    r[i, CurrentY] = true;
                }
                break;
            }
        }
        // Up
        i = CurrentX;
        while (true)
        {
            i++;
            if (i >= 8)
            {
                break;
            }
            c = CBoardManager.instance.Chessmans[CurrentX, i];
            if (c == null)
            {
                r[CurrentX, i] = true;
            }
            else
            {
                if (c.isWhite != isWhite)
                {
                    r[CurrentX, i] = true;
                }
                break;
            }
        }

        // Down
        i = CurrentX;
        while (true)
        {
            i--;
            if (i < 0)
            {
                break;
            }
            c = CBoardManager.instance.Chessmans[CurrentX, i];
            if (c == null)
            {
                r[CurrentX, i] = true;
            }
            else
            {
                if (c.isWhite != isWhite)
                {
                    r[CurrentX, i] = true;
                }
                break;
            }
        }
        return r;
    }
}
