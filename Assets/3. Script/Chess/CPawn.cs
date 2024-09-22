using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPawn : CChessman
{
    public override bool[,] PossibleMove()
    {
        bool[,] r = new bool[8, 8];
        CChessman c, c2;
        // White team move
        if (isWhite)
        {
            // Diagonal Left
            if (CurrentX != 0 && CurrentY != 7)
            {
                c = CBoardManager.instance.Chessmans[CurrentX - 1, CurrentY + 1];
                if (c != null && !c.isWhite)
                {
                    r[CurrentX - 1, CurrentY + 1] = true;
                }
            }

            // Diagonal Right
            if (CurrentX != 7 && CurrentY != 7)
            {
                c = CBoardManager.instance.Chessmans[CurrentX + 1, CurrentY + 1];
                if (c != null && !c.isWhite)
                {
                    r[CurrentX + 1, CurrentY + 1] = true;
                }
            }

            // Middle
            if (CurrentY != 7)
            {
                c = CBoardManager.instance.Chessmans[CurrentX, CurrentY + 1];
                if (c == null)
                {
                    r[CurrentX, CurrentY + 1] = true;
                }
            }

            // Middle on first move
            if (CurrentY == 1)
            {
                c = CBoardManager.instance.Chessmans[CurrentX, CurrentY + 1];
                c2 = CBoardManager.instance.Chessmans[CurrentX, CurrentY + 2];
                if (c == null && c2 == null)
                {
                    r[CurrentX, CurrentY + 2] = true;
                }
            }
        }
        else
        {
            // Diagonal Left
            if (CurrentX != 0 && CurrentY != 0)
            {
                c = CBoardManager.instance.Chessmans[CurrentX - 1, CurrentY - 1];
                if (c != null && c.isWhite)
                {
                    r[CurrentX - 1, CurrentY - 1] = true;
                }
            }

            // Diagonal Right
            if (CurrentX != 7 && CurrentY != 0)
            {
                c = CBoardManager.instance.Chessmans[CurrentX + 1, CurrentY - 1];
                if (c != null && c.isWhite)
                {
                    r[CurrentX + 1, CurrentY - 1] = true;
                }
            }

            // Middle
            if (CurrentY != 0)
            {
                c = CBoardManager.instance.Chessmans[CurrentX, CurrentY - 1];
                if (c == null)
                {
                    r[CurrentX, CurrentY - 1] = true;
                }
            }

            // Middle on first move
            if (CurrentY == 6)
            {
                c = CBoardManager.instance.Chessmans[CurrentX, CurrentY - 1];
                c2 = CBoardManager.instance.Chessmans[CurrentX, CurrentY - 2];
                if (c == null && c2 == null)
                {
                    r[CurrentX, CurrentY - 2] = true;
                }
            }
        }


        return r;
    }
}
