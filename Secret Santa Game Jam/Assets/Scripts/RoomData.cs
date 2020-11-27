using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


public enum Direction_Val
{
    INVALID = -1,
    NORTH,
    EAST,
    SOUTH,
    WEST,
    MAXDIRECTIONS

}

public struct Direction
{
    public Direction_Val GetOpposite(Direction_Val d)
    {
        switch (d)
        {
            case Direction_Val.NORTH:
                return Direction_Val.SOUTH;

            case Direction_Val.EAST:
                return Direction_Val.WEST;

            case Direction_Val.SOUTH:
                return Direction_Val.NORTH;

            case Direction_Val.WEST:
                return Direction_Val.EAST;
        }

        return Direction_Val.INVALID;
    }
}

public class RoomData : MonoBehaviour
{
    public GameObject[] Walls;
    public Transform RoomDecorationTransform;

    public float decoration_min_X = -10f;
    public float decoration_max_X = 17f;
    public float decoration_min_Z = -15f;
    public float decoration_max_Z = 12f;

    [Range(-10f, 17f)] private int decorationX;
    [Range(-15f, 12f)] private int decorationZ;

    public FloorGenerator floorGen;

    public List<Direction_Val> openDirections;

    public List<RoomData> connectedRooms = new List<RoomData>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Awake()
    {
        
    }

    public GameObject[] getWalls()
    {
        return Walls;
    }

    public void GetGenerationData(FloorGenerator gen)
    {
        floorGen = gen;
        floorGen.IncrementRoomCounter();

        //when assigning, check what directions are open, add to array
        for (int i = 0; i < (int) Direction_Val.MAXDIRECTIONS; i++)
        {
            if(!Walls[i].activeInHierarchy)
                openDirections.Add((Direction_Val)i);
        }

        for (int i = 0; i < (int)Direction_Val.MAXDIRECTIONS; i++)
        {
            connectedRooms.Insert(i, null);
        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Room")
            Destroy(gameObject);
    }

    //sets the neighbors of each, opens the wall values of each as well
    //returns true if it can link up the rooms together, returns false if it can't
    public bool SetNeighbor(Direction_Val d, RoomData neighbor)
    {
        //check if both the potential neighbor and this room can link up
        Direction dir;
        Direction_Val op = dir.GetOpposite(d);

        Debug.Log("Connected rooms: " + connectedRooms.Count);
        //check for this room in that direction and the neighbor room in the opposite direction
        if (connectedRooms[(int) d] == null && !neighbor.IsNeighborOccupied(op))
        {
            connectedRooms[(int)d] = neighbor;
            RemoveWall(op);
            
            neighbor.connectedRooms[(int) op] = this;
            neighbor.RemoveWall(d);

            return true;
        }
        
        return false;
    }

    //returns if room is already linked up at direction value d
    public bool IsNeighborOccupied(Direction_Val d)
    {
        if (connectedRooms[(int) d] != null)
            return true;

        return false;
    }

    public void RemoveWall(Direction_Val d)
    {
        Debug.Log("removing walls at " + d);
        Walls[(int)d].SetActive(false);
    }

    public void RemoveWall(int direction)
    {
        Debug.Log("removing walls at " + direction);
        Walls[direction].SetActive(false);
    }

    public void AddWall(Direction_Val d)
    {
        Walls[(int)d].SetActive(true);
    }

    public bool AddDecoration(Vector3 pos, GameObject obj)
    {
        //check if position is valid
        if ((pos.x >= Mathf.Min(decoration_min_X, decoration_max_X) && pos.x <= Mathf.Max(decoration_min_X, decoration_max_X)) &&
            (pos.z >= Mathf.Min(decoration_min_Z, decoration_max_Z) && pos.z <= Mathf.Max(decoration_min_Z, decoration_max_Z)))
        {
            obj.transform.SetParent(RoomDecorationTransform);
            obj.transform.position = pos;

            return true;
        }


        return true;
    }
}
