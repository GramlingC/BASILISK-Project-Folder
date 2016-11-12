using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class A_Pathfinding : MonoBehaviour
{
    //Grid (includes which places are blocked-blocked does not go into open

    //open set (Nodes)
    //closed set (Nodes and parents)

    //current

    //Need to find way to add restricted nodes.
    //This may work, if they're converted into Nodes.
    public Vector3[] restrictedNodes;

    public int width;//???
    public int length;

    public Vector3 lowerLeft;  //In-game location of lowest and leftmost grid coordinate

    //Make driver monobehavior for this?

    private Grid grid;
    //private List<Node> illegalNodes;

    //Computes the shortest path from startVec to finishVec.  A* algorithm.
    public List<Vector3> FindPath(Vector3 startVec, Vector3 finishVec)
    {
        Grid grid = new Grid(width, length);
        Node[,] nodes = grid.GetGridNodes();
        foreach(Vector3 rVec in restrictedNodes)
        {
            int[] rNode = ConvertToGridCoord(rVec);
            //Debugging check - make sure each restricted node is on the grid.
            if (rNode[0] < 0 || rNode[1] < 0 || rNode[0] >= width || rNode[1] >= length)
                Debug.Log(rVec + " is off the grid.");
            else
                nodes[rNode[0], rNode[1]].SetIllegal();
        }

        //nodes[1, 0].SetIllegal(); //Tests
        //nodes[1, 1].SetIllegal();//Tests

        //Lists of node coordinates
        List<int[]> open = new List<int[]>();
        List<int[]> closed = new List<int[]>();
        

        int[] start = ConvertToGridCoord(startVec);
        int[] finish = ConvertToGridCoord(finishVec);

        //Debugging check
        if (start[0] < 0 || start[1] < 0 || start[0] >= width || start[1] >= length)
        {
            Debug.Log(startVec + " is not on the grid");
            List<Vector3> dummyList = new List<Vector3>();
            dummyList.Add(Vector3.zero);
        }
        else if (finish[0] < 0 || finish[1] < 0 || finish[0] >= width || finish[1] >= length)
        {
            Debug.Log(finishVec + " is not on the grid");
            List<Vector3> dummyList = new List<Vector3>();
            dummyList.Add(Vector3.zero);
            return dummyList;
        }

        //The starting position is added to the open list, and its G and H values are set.
        open.Add(start);
        nodes[start[0], start[1]].SetGDist(0);
        nodes[start[0], start[1]].SetHDist(grid.ManhattenDist(start[0], start[1], finish[0], finish[1]));

        //This will keep track of the current node coordinates
        int[] current = new int[2];
        while (true)
        {
            //Keeps track of the lowest F count
            int lowestF = width * length;//???
            foreach (int[] indices in open)
            {
                //Start to current will be parent start to current + 1
                if (nodes[indices[0], indices[1]].GetGDist() + nodes[indices[0], indices[1]].GetHDist() <= lowestF)
                {
                    current = indices;
                    lowestF = nodes[indices[0], indices[1]].GetGDist() + nodes[indices[0], indices[1]].GetHDist();
                }
            }

            int removeDex = -1;
            int currDex = 0;
            foreach (int[]o in open)
            {
                if(current[0] == o[0] && current[1] == o[1])
                {
                    removeDex = currDex;
                    break;
                }
                currDex++;
            }
            //Debug.Log(removeDex);
            if(removeDex > -1)
                open.RemoveAt(removeDex);
            open.Remove(current);
            closed.Add(current);
            
            if (current[0] == finish[0] && current[1] == finish[1])
                break;  //path has been found

            List<int[]> neighbors = grid.GetNeighborNodes(current[0], current[1]);

            foreach (int[] neighbor in neighbors)
            {
                bool closedContainsNeighbor = false;
                foreach (int[] o in closed)
                {
                    if (neighbor[0] == o[0] && neighbor[1] == o[1])
                    {
                        closedContainsNeighbor = true;
                        break;
                    }
                }

                if (!nodes[neighbor[0], neighbor[1]].GetIsLegal() || closedContainsNeighbor)
                {
                    continue;
                }

                bool openContainsNeighbor = false;
                foreach (int[] o in open)
                {
                    if (neighbor[0] == o[0] && neighbor[1] == o[1])
                    {
                        openContainsNeighbor = true;
                        break;
                    }
                }
                //nodes[neighbor[0], neighbor[1]].SetParentNode(current);//???
                if (!openContainsNeighbor || nodes[current[0], current[1]].GetGDist() < nodes[neighbor[0], neighbor[1]].GetGDist())
                {
                    nodes[neighbor[0], neighbor[1]].SetGDist(nodes[current[0], current[1]].GetGDist() + 1);
                    nodes[neighbor[0], neighbor[1]].SetParentNode(current);
                    //Debug.Log("current: " + current[0] + " " + current[1]);
                    //Debug.Log("Parent: " + nodes[neighbor[0], neighbor[1]].GetParentNode()[0] + " " + nodes[neighbor[0], neighbor[1]].GetParentNode()[1]);
                    if (!openContainsNeighbor)
                    {
                        open.Add(neighbor);
                        nodes[neighbor[0], neighbor[1]].SetHDist(grid.ManhattenDist(neighbor[0], neighbor[1], finish[0], finish[1]));
                    }
                }
            }
        }

        List<int[]> path = new List<int[]>();
        int[] curr = finish;

        //int test = 0;
        //List.Reverse() is a thing
        while (true)
        {
            //Debug.Log(curr[0] + " " + curr[1]);
            //Adds the node index to the front of the list - the list is being filled in reverse, after all.
            path.Add(curr);
            if(curr[0] == start[0] && curr[1] == start[1])
            {
                break;
            }
            curr = nodes[curr[0], curr[1]].GetParentNode();//Something is wrong with the parent relationship.
            
            //if (test == 8)
             //   break; //temp
            //test++;
        }

        path.Reverse();

        List<Vector3> vecPath = new List<Vector3>();
        foreach(int[] coords in path)
        {
            vecPath.Add(ConvertToVector(coords));
        }
        return vecPath;
    }

    //The methods below may need to be adjusted depending on the orientation of the game relative to the axis.

    //Converts a vector position to it's corresponding node on the grid
    public int[] ConvertToGridCoord(Vector3 pos)
    {
        int[] coords = new int[2];

        //Converts the vector coordinates into grid coordinates.  Also rounds them, since grid coordinates must be ints.
        coords[0] = (int)(Mathf.Round(pos.x) - Mathf.Round(lowerLeft.x));
        coords[1] = (int)(Mathf.Round(pos.z) - Mathf.Round(lowerLeft.z));
        return coords;
    }

    //Converts a node into a vector position in the game world.
    public Vector3 ConvertToVector(int[] nodeCoords)
    {
        return new Vector3(nodeCoords[0] + lowerLeft.x, 0, nodeCoords[1] + lowerLeft.z);
        //The y of the vector shouldn't matter, due to the use of a y offset
    }
}
