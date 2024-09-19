using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CPlayerMoveableChess : CChessManager
{
    public GameObject coordPointer; // �̵� ���� ��ǥ ǥ�ÿ�
    private List<GameObject> coordPointers = new List<GameObject>();
    private bool selected = false;

    public abstract List<Vector2Int> MoveableCoord();

    public override void BoardSelect(Vector2Int coord)
    {
        if (coord == pos && selected == false) // ���� ���õ�
        {
            selected = true;
            MakeMovableCoordPointer();
        }
        else if (selected == true && MoveableCoord().Contains(coord)) // ���� ���õǰ�, ���� �̵� ĭ�� ���õ�
        {
            MoveToCoordinate(coord);
            selected = false;
            DeleteMoveableCoordPointer();
        }
        else if (coord != pos) // �̿��� ĭ�� ���õ�
        {
            selected = false;
            DeleteMoveableCoordPointer();
        }
    }

    public void MakeMovableCoordPointer()
    {
        List<Vector2Int> coords = MoveableCoord();
        foreach (Vector2Int coord in coords)
        {
            GameObject obj = Instantiate(coordPointer);
            obj.transform.position = CBoardManager.CoordinateToWorld(coord);
            coordPointers.Add(obj);
        }
    }

    public void DeleteMoveableCoordPointer()
    {
        foreach (GameObject obj in coordPointers)
        {
            Destroy(obj);
        }
        coordPointers.Clear();
    }
}
