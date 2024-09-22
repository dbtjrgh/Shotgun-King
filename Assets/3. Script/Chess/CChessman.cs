using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CChessman : MonoBehaviour
{
    public int CurrentX { get; private set; }
    public int CurrentY { get; private set; }
    public bool isWhite;

    public void SetPosition(int x, int y)
    {
        CurrentX = x;
        CurrentY = y;
    }

    public virtual bool[,] PossibleMove()
    {
        return new bool[8, 8];
    }
}
