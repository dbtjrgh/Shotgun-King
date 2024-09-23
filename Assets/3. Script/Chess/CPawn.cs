using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPawn : CChessman
{
    public override bool[,] PossibleMove()
    {
        bool[,] r = new bool[8, 8];
        CChessman c, c2;
        int i, j;
        int[] e = CBoardManager.instance.EnPassantMove;

        i = CurrentX;
        j = CurrentY;

        // White team move
        if (isWhite)
        {
            // Diagonal Left
            if (i != 0 && j != 7)
            {
                if (e[0] == i - 1 && e[1] == j + 1)
                {
                    r[i - 1, j + 1 ] = true;
                }
                c = CBoardManager.instance.Chessmans[i - 1, j + 1];
                if (c != null && !c.isWhite)
                {
                    r[i - 1, j + 1] = true;
                }
            }

            // Diagonal Right
            if (i != 7 && j != 7)
            {
                if (e[0] == i + 1 && e[1] == j + 1)
                {
                    r[i - 1, j + 1] = true;
                }
                c = CBoardManager.instance.Chessmans[i + 1, j + 1];
                if (c != null && !c.isWhite)
                {
                    r[i + 1, j + 1] = true;
                }
            }

            // Middle
            if (j != 7)
            {
                c = CBoardManager.instance.Chessmans[i, j + 1];
                if (c == null)
                {
                    r[i, j + 1] = true;
                }
            }

            // Middle on first move
            if (j == 1)
            {
                c = CBoardManager.instance.Chessmans[i, j + 1];
                c2 = CBoardManager.instance.Chessmans[i, j + 2];
                if (c == null && c2 == null)
                {
                    r[i, j + 2] = true;
                }
            }
        }
        else
        {
            // Diagonal Left
            if (i != 0 && j != 0)
            {
                if (e[0] == i - 1 && e[1] == j - 1)
                {
                    r[i - 1, j - 1] = true;
                }
                c = CBoardManager.instance.Chessmans[i - 1, j - 1];
                if (c != null && c.isWhite)
                {
                    r[i - 1, j - 1] = true;
                }
            }

            // Diagonal Right
            if (i != 7 && j != 0)
            {
                if (e[0] == i + 1 && e[1] == j - 1)
                {
                    r[i + 1, j - 1] = true;
                }
                c = CBoardManager.instance.Chessmans[i + 1, j - 1];
                if (c != null && c.isWhite)
                {
                    r[i + 1, j - 1] = true;
                }
            }

            // Middle
            if (j != 0)
            {
                c = CBoardManager.instance.Chessmans[i, j - 1];
                if (c == null)
                {
                    r[i, j - 1] = true;
                }
            }

            // Middle on first move
            if (j == 6)
            {
                c = CBoardManager.instance.Chessmans[i, j - 1];
                c2 = CBoardManager.instance.Chessmans[i, j - 2];
                if (c == null && c2 == null)
                {
                    r[i, j - 2] = true;
                }
            }
        }


        return r;
    }
}
