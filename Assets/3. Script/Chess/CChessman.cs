using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CChessman : MonoBehaviour
{
    public int CurrentX { get; private set; }
    public int CurrentY { get; private set; }
    public bool isWhite;

    // Store the original material for each chess piece
    public Material originalMaterial { get; set; }

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
