using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generation : MonoBehaviour
{
    public int mapWidth = 1;
    public int mapHeight = 1;
    public int roomsToGenerate = 1;

    private int roomCount;
    private bool roomsInstatiated;

    private Vector2 firstRoomPos;

    private bool[,] map;
    public GameObject roomPrefab;

    private List<Room> roomObjects = new List<Room>();

    public static Generation instance;

    //spawn chances
    public float enemySpawnChance;
    public float coinSpawnChance;
    public float healthSpawnChance;

    public int maxEnemiesPerRoom;
    public int maxCoinsPerRoom;
    public int maxHealthPerRoom;

    //singelton for this shit
    private void Awake() 
    {
        instance = this;
    }

    public void OnPlayerMove()
    {
        Vector2 playerPos = FindObjectOfType<Player>().transform.position;
        Vector2 roomPos = new Vector2 (((int)playerPos.x + 6) / 12, ((int)playerPos.y + 6) / 12);

        UI.instance.map.texture = MapTextureGenerator.Generate(map, roomPos);
    }

    //called at the start of the game- begins the generation process
    public void Generate()
    {
        map = new bool[mapWidth, mapHeight];
        CheckRoom(3, 3, 0, Vector2.zero, true);
        InstantiateRooms();
        FindObjectOfType<Player>().transform.position = firstRoomPos * 12;

        UI.instance.map.texture = MapTextureGenerator.Generate(map, firstRoomPos);
    }

    //check to see if we can place room here
    void CheckRoom(int x, int y, int remaining, Vector2 generalDirection, bool firstRoom = false)
    {
        //return if we placed all of the rooms
        if (roomCount >= roomsToGenerate)
            return;

        //return if the room is outside of the map bounds
        if (x < 0 || x > mapWidth - 1 || y < 0 || y > mapHeight - 1)
            return;

        //return if we got no remaining rooms to work with
        if (firstRoom == false && remaining <= 0)
            return;

        //return if this room already exists
        if (map[x,y] == true)
            return;

        //set the first room position
        if(firstRoom == true)
            firstRoomPos = new Vector2(x, y);

        roomCount++;
        map[x, y] = true;

        bool top = Random.value > (generalDirection == Vector2.up ? 0.2f : 0.8f);
        bool bottom = Random.value > (generalDirection == Vector2.down ? 0.2f : 0.8f);
        bool left = Random.value > (generalDirection == Vector2.left ? 0.2f : 0.8f);
        bool right = Random.value > (generalDirection == Vector2.right ? 0.2f : 0.8f);

        int maxRemaining = roomsToGenerate / 4;

        if (top || firstRoom)
            CheckRoom(x, y + 1, firstRoom ? maxRemaining : remaining - 1, firstRoom ? Vector2.up : generalDirection);

        if (bottom || firstRoom)
            CheckRoom(x, y - 1, firstRoom ? maxRemaining : remaining - 1, firstRoom ? Vector2.down : generalDirection);

        if (left || firstRoom)
            CheckRoom(x - 1, y, firstRoom ? maxRemaining : remaining - 1, firstRoom ? Vector2.left : generalDirection);

        if (right || firstRoom)
            CheckRoom(x + 1, y, firstRoom ? maxRemaining : remaining - 1, firstRoom ? Vector2.right : generalDirection);
    }

    void InstantiateRooms()
    {
        if (roomsInstatiated)
            return;

        roomsInstatiated = true;

        for (int x = 0; x < mapWidth; ++x)
        {
            for (int y = 0; y < mapHeight; ++y)
            {
                if (map[x, y] == false)
                    continue;

                GameObject roomObj = Instantiate(roomPrefab, new Vector3(x, y, 0) * 12, Quaternion.identity);
                Room room = roomObj.GetComponent<Room>();

                //check if we have room above and if yes then enable top doors and disable top wall
                if (y < mapHeight -1 && map[x, y + 1] == true)
                {
                    room.topDoor.gameObject.SetActive(true);
                    room.topWall.gameObject.SetActive(false);
                }

                if (y > 0  && map[x, y - 1] == true)
                {
                    room.bottomDoor.gameObject.SetActive(true);
                    room.bottomWall.gameObject.SetActive(false);
                }

                if (x > 0 && map[x - 1, y] == true)
                {
                    room.leftDoor.gameObject.SetActive(true);
                    room.leftWall.gameObject.SetActive(false);
                }

                if (x < mapWidth - 1 && map[x + 1, y] == true)
                {
                    room.rightDoor.gameObject.SetActive(true);
                    room.rightWall.gameObject.SetActive(false);
                }

                //if is isin't first room then generate something inside room
                if (firstRoomPos != new Vector2(x, y))
                    room.GenerateInterior();

                roomObjects.Add(room);
            }
        }
        CalculateKeyAndExit();
    }

    //place the key and exit in the level
    void CalculateKeyAndExit()
    {
        float maxDistance = 0;
        Room keyRoom = null;
        Room doorRoom = null;

        foreach(Room kRoom in roomObjects) //calculating most distanced rooms 
        {
            foreach(Room dRoom in roomObjects)
            {
                float distance = Vector3.Distance(kRoom.transform.position, dRoom.transform.position);

                if(distance > maxDistance)
                {
                    keyRoom = kRoom;
                    doorRoom = dRoom;
                    maxDistance = distance;
                }
            }
        }

        keyRoom.SpawnPrefab(keyRoom.keyPrefab);
        doorRoom.SpawnPrefab(doorRoom.exitDoorPrefab);
    }
}
