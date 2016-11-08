using UnityEngine;
using System.Collections;

public class Node
{
    //Manually insert obstacle nodes?

    int[] parentNode;
    bool isLegal;
    int gDist;
    int hDist;

	public Node ()
    {
        parentNode = null;
        isLegal = true;
    }

    public Node (int[] newParentNode)
    {
        parentNode = newParentNode;//May not be necessary
        isLegal = true;
    }

    /*public int[] GetNodePos()
    {
        return node;
    }*/

    public void SetParentNode(int[] newCoords)
    {
        parentNode = newCoords;
    }

    public int[] GetParentNode()
    {
        return parentNode;
    }

    public void SetIllegal()
    {
        isLegal = false;
    }

    public bool GetIsLegal()
    {
        return isLegal;
    }

    public void SetGDist(int newG)
    {
        gDist = newG;
    }

    public int GetGDist()
    {
        return gDist;
    }

    public void SetHDist(int newH)
    {
        hDist = newH;
    }

    public int GetHDist()
    {
        return hDist;
    }
}
