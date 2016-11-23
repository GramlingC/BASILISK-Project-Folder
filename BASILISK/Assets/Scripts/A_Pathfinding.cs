using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class A_Pathfinding : MonoBehaviour
{
    //public Vector3[] restrictedNodes;

    //restrictedVecObj can be an object that has all prefab obstacles attached (their children and all)
    public GameObject restrictedVecObj;
    private List<int[]> restrictedNodes = new List<int[]>();

    public int width;  //x direction
    public int length;  //z direction

    public Vector3 lowerLeft;  //In-game location of lowest and leftmost grid coordinate

    private Grid grid;

    void Start()
    {
        //Debug.Log("Starting");
        //List<Vector3> restrictedVecs = new List<Vector3>();
        foreach(Transform obj in restrictedVecObj.GetComponentInChildren<Transform>())
        {
            //Looks for children in children(for prefabs)
            foreach (Transform child in obj.GetComponentInChildren<Transform>())
            {
                //This only reads the transform.positions of gameObjects that have "collider" in their names
                if (child.name.Contains("collider"))
                {
                    //Debug.Log(child.name);
                    //Get dimensions of object
                    float c_length = child.transform.lossyScale.z;
                    float c_width = child.transform.lossyScale.x;

                    //iterate through dimensions
                    for (float l = -.5F * (c_length); l < .5F * (c_length); l++)
                    {
                        for (float w = -.5F * (c_width); w < .5F * (c_width); w++)
                        {
                            //Debug.Log(obj.name);
                            int[] rNode = ConvertToGridCoord(child.transform.position + new Vector3(w, 0f, l)); //???
                            //Debug.Log(rNode[0] + " " + rNode[1]); //Shows all the right coordinates
                            //Debugging check - make sure each restricted node is on the grid
                            if (rNode[0] < 0 || rNode[1] < 0 || rNode[0] >= width || rNode[1] >= length)
                                Debug.Log(child.position + ": restricted node is off the grid.  It will not be added.");
                            else
                                restrictedNodes.Add(rNode);
                        }
                    }
                }
                //May try to find way to prevent duplicate restricted nodes from being added.  Is not top priority.
            }
        }
        //Debugging code.  Can be uncommented out to print which nodes are in the restricted list.
        
        /*foreach(int[] node in restrictedNodes)
        {
            Debug.Log("Restricted node: " + node[0] + " " + node[1]);
        }*/
    }//end Start

    //Computes the shortest path from startVec to finishVec.  A* algorithm.
    public List<Vector3> FindPath(Vector3 startVec, Vector3 finishVec)
    {
        Grid grid = new Grid(width, length);
        Node[,] nodes = grid.GetGridNodes();
        //Debug.Log("To vector" + ConvertToVector(new int[] { width-1, length-1 }));


        int[] start = ConvertToGridCoord(startVec);
        int[] finish = ConvertToGridCoord(finishVec);

        //OLD CODE.  Being kept around in case.
        /*foreach (Vector3 rVec in restrictedNodes)
        {
            int[] rNode = ConvertToGridCoord(rVec);

            //Debugging check - make sure each restricted node is on the grid, make sure finishing point is not a restricted node.
            if (rNode[0] < 0 || rNode[1] < 0 || rNode[0] >= width || rNode[1] >= length)
                Debug.Log(rVec + ": restricted node is off the grid.");
            else if(rNode[0] == finish[0] && rNode[1] == finish[1])
                Debug.Log(finishVec + " is also a restricted node.");
            else
                nodes[rNode[0], rNode[1]].SetIllegal();
        }*/

        foreach (int[] rNode in restrictedNodes)
        {
            //Debugging check - make sure finishing point is not a restricted node.
            if (rNode[0] == finish[0] && rNode[1] == finish[1])
                Debug.Log(finishVec + " is also a restricted node.  Not setting that restricted node to illegal to avoid crash.");
            else
            {
                //Debug.Log(rNode[0] + " " + rNode[1]);
                nodes[rNode[0], rNode[1]].SetIllegal(); //problem- the game keeps saying that this array index is out of range
            }
        }

        //Lists of node coordinates
        List<int[]> open = new List<int[]>();
        List<int[]> closed = new List<int[]>();
        

        
        //Debugging check.  Will route enemy back to finishVec if startVec or finishVec is not on the grid.
        if (start[0] < 0 || start[1] < 0 || start[0] >= width || start[1] >= length)
        {
            Debug.Log(startVec + ": start is not on the grid");
            List<Vector3> dummyList = new List<Vector3>();
            dummyList.Add(finishVec);
            return dummyList;
        }
        else if (finish[0] < 0 || finish[1] < 0 || finish[0] >= width || finish[1] >= length)
        {
            Debug.Log(finishVec + ": finish is not on the grid");
            List<Vector3> dummyList = new List<Vector3>();
            dummyList.Add(finishVec);
            return dummyList;
        }

        //The starting position is added to the open list, and its G and H values are set.
        open.Add(start);
        nodes[start[0], start[1]].SetGDist(0);
        nodes[start[0], start[1]].SetHDist(grid.DiagDist(start[0], start[1], finish[0], finish[1]));

        //This will keep track of the current node coordinates
        int[] current = new int[2];
        //int whileTracker = 0;
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

            if (removeDex > -1)
            {
                open.RemoveAt(removeDex);
                closed.Add(current);
            }
            
            if (current[0] == finish[0] && current[1] == finish[1])
                break;  //path has been found

            //Will also need to find diagonal neighbors
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
                
                if (!openContainsNeighbor || nodes[current[0], current[1]].GetGDist() < nodes[neighbor[0], neighbor[1]].GetGDist())
                {
                    nodes[neighbor[0], neighbor[1]].SetGDist(nodes[current[0], current[1]].GetGDist() + neighbor[2]);
                    nodes[neighbor[0], neighbor[1]].SetParentNode(current);

                    if (!openContainsNeighbor)
                    {
                        open.Add(new int[] { neighbor[0], neighbor[1] });
                        nodes[neighbor[0], neighbor[1]].SetHDist(grid.DiagDist(neighbor[0], neighbor[1], finish[0], finish[1]));
                    }
                }
            }
        }

        List<int[]> path = new List<int[]>();
        int[] curr = finish;

        while (true)
        {
            //Adds the node index to the front of the list - the list is being filled in reverse, after all.
            path.Add(curr);
            if(curr[0] == start[0] && curr[1] == start[1])
            {
                break;
            }
            curr = nodes[curr[0], curr[1]].GetParentNode();
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
        coords[0] = (int)(Mathf.Round(pos.x - lowerLeft.x));
        coords[1] = (int)(Mathf.Round(pos.z - lowerLeft.z));
        return coords;
    }

    //Converts a node into a vector position in the game world.
    public Vector3 ConvertToVector(int[] nodeCoords)
    {
        return new Vector3(nodeCoords[0] + lowerLeft.x, 0, nodeCoords[1] + lowerLeft.z);
        //The y of the vector shouldn't matter, due to the use of a y offset
    }


    //Functions that calculate the boundaries of the grid.  Can be used to constrain the player and the enemies to the grid.
    public float GetLeftBound()
    {
        return lowerLeft.x;
    }

    public float GetRightBound()
    {
        return lowerLeft.x + width;
    }

    public float GetBottomBound()
    {
        return lowerLeft.z;
    }

    public float GetTopBound()
    {
        return lowerLeft.z + length;
    }
}
