using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid
{
    private int width;
    private int length;

    //Using a 2D array
    private Node[,] gridNodes;

    //Make off-limits nodes here?

	// Use this for initialization
	public Grid()
    {
        Debug.Log("Missing length and width");
    }

    //Sets up the grid based on the inputted width and length
    public Grid(int newWidth, int newLength)
    {
        width = newWidth;
        length = newLength;
        gridNodes = new Node[width, length];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
            {
                gridNodes[i,j] = new Node(new int[] { i, j });
            }
        }
        //Debug.Log(gridNodes.Length);
    }

    //Returns a list of neighbor indexes.  GetNeighborIndices?
    public List<int[]> GetNeighborNodes(int x, int y)
    {
        //Each int[] in the list will contain the x index, the y/z index, and the weight (up/down is 2, diagonal is 3)

        //The list that will contain the coords of the neighboring nodes.
        List<int[]> neighbors = new List<int[]>();

        //Checks in four directions.
        if(x - 1 >= 0)
        {
            if (gridNodes[x-1, y].GetIsLegal())
            {
                neighbors.Add(new int[] { x - 1, y, 2 });
                //neighbors.Add(gridNodes[x - 1, y].GetNodePos()
            }
        }
        if (x + 1 < width)
        {
            if (gridNodes[x + 1, y].GetIsLegal())
            {
                neighbors.Add(new int[] { x + 1, y, 2 });
            }
        }
        if (y - 1 >= 0)
        {
            if (gridNodes[x, y - 1].GetIsLegal())
            {
                neighbors.Add(new int[] { x, y - 1, 2 });
            }
        }
        if (y + 1 < length)
        {
            if (gridNodes[x, y + 1].GetIsLegal())
            {
                neighbors.Add(new int[] { x, y + 1, 2});
            }
        }

        //Checks diagonals
        if (x - 1 >= 0 && y - 1 >= 0)
        {
            if (gridNodes[x - 1, y - 1].GetIsLegal())
            {
                neighbors.Add(new int[] { x - 1, y - 1, 3 });
                //neighbors.Add(gridNodes[x - 1, y].GetNodePos()
            }
        }
        if (x + 1 < width && y - 1 >= 0)
        {
            if (gridNodes[x + 1, y - 1].GetIsLegal())
            {
                neighbors.Add(new int[] { x + 1, y - 1, 3 });
            }
        }
        if (x - 1 >= 0 && y + 1 < length)
        {
            if (gridNodes[x - 1, y + 1].GetIsLegal())
            {
                neighbors.Add(new int[] { x - 1, y + 1, 3 });
            }
        }
        if (x + 1 < width && y + 1 < length)
        {
            if (gridNodes[x + 1, y + 1].GetIsLegal())
            {
                neighbors.Add(new int[] { x + 1, y + 1, 3 });
            }
        }

        return neighbors;
    }

    public Node[,] GetGridNodes()
    {
        //Node List instead?  Will allow for easy removal of restricted nodes.
        return gridNodes;
    }

    //Likely depricated with change from 4 to 8 direction movement
    public int ManhattenDist(int x1, int y1, int x2, int y2)
    {
        return (Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2));
    }

    //Roughly approximates distance between two points
    public int DiagDist(int x1, int y1, int x2, int y2)
    {
        return 2 * Mathf.Max(Mathf.Abs(x1 - x2), Mathf.Abs(y1 - y2));
    }
}
