using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public int tileSize;
    public GameObject tilePrefab;
    public GameObject diamondPrefab;
    public GameObject endPrefab;
    public GameObject tPrefab;
    public GameObject fwPrefab;
    public GameObject roomPrefab;

    public int maxHallDistance = 10;
    public int roomChance;
    public int endChance;
    public int cornerChance;
    public int threeInterChance;
    public int fourInterChance;

    public string[] cardinal = new string[] {"North", "East", "South", "West"};
    public string[] OPcardinal = new string[] {"South", "West", "North", "East"};

    public int totalGenerations = 0;
    public int sizeLimit1;
    public int sizeLimit2;
    public int sizeLimit3;

    private HashSet<Vector2> occupiedPositions = new HashSet<Vector2>();

    // Start is called before the first frame update
    void Start()
    {
        spawnPrefab(tilePrefab, 0, 0);
        generate_hallway(cardinal[Random.Range(0, cardinal.Length)], 0, 0);
        generate_hallway(cardinal[Random.Range(0, cardinal.Length)], 0, 0);
        generate_hallway(cardinal[Random.Range(0, cardinal.Length)], 0, 0);
        generate_hallway(cardinal[Random.Range(0, cardinal.Length)], 0, 0);
    }

    private void spawnPrefab(GameObject prefab, float posX, float posY)
    {
        Vector2 position = new Vector2(posX, posY);

        if (!occupiedPositions.Contains(position))
        {
            GameObject a = Instantiate(prefab) as GameObject;
            a.transform.position = position;
            a.transform.localScale *= tileSize;
            occupiedPositions.Add(position);
        }
    }

    private void generate_hallway(string direction, float posX, float posY)
    {
        int distance = Random.Range(3, maxHallDistance);
        int roomRoll = Random.Range(0, 100);
        int JointRoll = Random.Range(0, 100);
        int i = 0;
        int dirX = 0;
        int dirY = 0;
        //bool placeableRoom = true;

        

        if (direction == "North")
        {
            dirX = 0;
            dirY = tileSize;
        }
        else if (direction == "East")
        {
            dirX = tileSize;
            dirY = 0;
        }
        else if (direction == "South")
        {
            dirX = 0;
            dirY = -tileSize;
        }
        else if (direction == "West")
        {
            dirX = -tileSize;
            dirY = 0;
        }

        while (i < distance)
        {
            if (i == 0)
            {
                posX += dirX;
                posY += dirY;
                i++;
            }
            else
            {
                spawnPrefab(tilePrefab, posX, posY);
                posX += dirX;
                posY += dirY;
                i++;
            }
        }

        totalGenerations++;
        updateChances();

        
        
        


        if (JointRoll <= endChance) //-2
        {
            spawnPrefab(tilePrefab, posX, posY);
        }

        else if (JointRoll <= endChance + cornerChance) //-1
        {
            spawnPrefab(tilePrefab, posX, posY);

            int randEntry1 = GetValidDirection(direction);

            generate_hallway(cardinal[randEntry1], posX, posY);
        }

        else if (JointRoll <= (endChance + cornerChance + threeInterChance))
        {
            //t intersection
            spawnPrefab(tPrefab, posX, posY);

            int randEntry1 = GetValidDirection(direction);
            int randEntry2 = GetValidDirection(direction, randEntry1);


            generate_hallway(cardinal[randEntry1], posX, posY);
            generate_hallway(cardinal[randEntry2], posX, posY);

            
        }
        else if (JointRoll <= (endChance + cornerChance + threeInterChance + fourInterChance))
        {
            //4 way intersection
            spawnPrefab(fwPrefab, posX, posY);
            for (int j = 0; j < cardinal.Length; j++)
            {
                if (direction != OPcardinal[j])
                {
                    generate_hallway(cardinal[j], posX, posY);
                }
            }
        }
        else
        {
            spawnPrefab(tilePrefab, posX, posY);
            //generate_hallway(cardinal[randEntry1], posX, posY);
        }
        
    }

    private int GetValidDirection(string currentDirection, int? excludedIndex = null)
    {
        int rand = Random.Range(0, cardinal.Length);

        while (currentDirection == OPcardinal[rand] || (excludedIndex != null && rand == excludedIndex))
        {
            rand = Random.Range(0, cardinal.Length);
        }
        return rand;
    }

    private void updateChances()
    {
        if (totalGenerations >= sizeLimit1 && totalGenerations <= sizeLimit2)
        {
            endChance = 10;
            cornerChance = 25;
            threeInterChance = 25;
            fourInterChance = 25;
            roomChance = 15;
        }
        else if (totalGenerations >= sizeLimit2)
        {
            endChance = 45;
            cornerChance = 10;
            threeInterChance = 10;
            fourInterChance = 10;
            roomChance = 25;
        }
    }
}
