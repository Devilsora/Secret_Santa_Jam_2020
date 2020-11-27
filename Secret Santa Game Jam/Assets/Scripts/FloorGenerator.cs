using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class FloorGenerator : MonoBehaviour
{
    public RoomData roomPrefab;

    public Transform floorParent;

    public List<GameObject> basicEnemies;
    public List<GameObject> bosses;
    public List<GameObject> treasures;
    public List<GameObject> decorations;

    public float roomWidth = 40f;
    public float roomLength = 40f;

    [SerializeField]
    [Range(10, 30)] 
    private int roomsToSpawn;

    private int roomsSpawned;
    public bool finishedSpawning;

    Direction opposite;

    // Start is called before the first frame update
    void Start()
    {
        //start by creating a room with no walls - should kick off whole room creation process
        GameObject entryRoom = Instantiate(roomPrefab.gameObject, floorParent);
        entryRoom.name = "Entry Room";
        RoomData entryData = entryRoom.GetComponent<RoomData>();

        entryData.GetGenerationData(this);

        //four different coroutines to handle for each direction vs. having the 1 for loop
        
        Debug.Log("Starting generation going north");
        entryRoom.GetComponent<RoomData>().RemoveWall(Direction_Val.NORTH);
        StartCoroutine(GenerateRoom(Direction_Val.NORTH, entryData));

        Debug.Log("Starting generation going south");
        entryRoom.GetComponent<RoomData>().RemoveWall(Direction_Val.SOUTH);
        StartCoroutine(GenerateRoom(Direction_Val.SOUTH, entryData));

        Debug.Log("Starting generation going east");
        entryRoom.GetComponent<RoomData>().RemoveWall(Direction_Val.EAST);
        StartCoroutine(GenerateRoom(Direction_Val.EAST, entryData));

        Debug.Log("Starting generation going west");
        entryRoom.GetComponent<RoomData>().RemoveWall(Direction_Val.WEST);
        StartCoroutine(GenerateRoom(Direction_Val.WEST, entryData));

        //for (int i = 0; i < (int) Direction_Val.MAXDIRECTIONS; i++)
        //{
        //    entryRoom.GetComponent<RoomData>().RemoveWall((Direction_Val)i);
        //    Debug.Log("Spawning rooms in start method");
        //    
        //    StartCoroutine(GenerateRoom((Direction_Val) i, entryData));
        //}

        //spawn rooms until we hit the spawn limit
        //have rooms start with no exits/entrances, then decide which ones to open up based on where they're connected to

    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator GenerateRoom(Direction_Val d, RoomData prev)
    {
        
        if (finishedSpawning)
        {
            yield break;
        }

        
        List<Direction_Val> openDirections = new List<Direction_Val>();

        GameObject newRoom = Instantiate(roomPrefab.gameObject, floorParent);
        RoomData newRoomData = newRoom.GetComponent<RoomData>();

        newRoom.name = "Room " + roomsSpawned;

        newRoomData.GetGenerationData(this);

        Vector3 newRoomPos = prev.gameObject.transform.position;

        switch (d)
        {
            case Direction_Val.NORTH:
                newRoomPos.z += roomLength;
                break;

            case Direction_Val.SOUTH:
                newRoomPos.z -= roomLength;
                break;

            case Direction_Val.EAST:
                newRoomPos.x += roomWidth;
                break;

            case Direction_Val.WEST:
                newRoomPos.x -= roomWidth;
                break;
        }
        
        newRoom.transform.position = newRoomPos;

        newRoomData.SetNeighbor(opposite.GetOpposite(d), prev);
        prev.SetNeighbor(d, newRoomData);

        for (int i = 0; i < (int)Direction_Val.MAXDIRECTIONS; i++)
        {
            if(!newRoomData.IsNeighborOccupied((Direction_Val)i))
                openDirections.Add((Direction_Val)i);
        }

        Debug.Log("Num possible directions: " + openDirections.Count);

        //randomize number of new directions to branch off from 
        int branchingRooms = Random.Range(1, 3);    //make this greater than 0, and fill in walls later

        for (int i = 0; i < branchingRooms; i++)
        {
            //choose a direction
            Direction_Val randDirection = openDirections[Random.Range(0, openDirections.Count - 1)];
            openDirections.Remove(randDirection);
            Debug.Log("Making room in random direction of " + randDirection);
            StartCoroutine(GenerateRoom(randDirection, newRoomData));
        }

    }

    public void IncrementRoomCounter()
    {
        roomsSpawned++;
        if (roomsSpawned >= roomsToSpawn)
            finishedSpawning = true;
    }

    public int GetRoomsSpawned()
    {
        return roomsSpawned;
    }

}
