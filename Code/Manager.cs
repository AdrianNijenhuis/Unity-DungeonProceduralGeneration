using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    //public bool DEBUGG = true;
    public int tileSize;
    public GameObject tilePrefab;
    public GameObject vTile;
    public GameObject hTile;
    public GameObject diamondPrefab;
    public GameObject endPrefab;
    public GameObject tPrefab;
    public GameObject fwPrefab;
    public GameObject roomPrefab;
    public GameObject NEcorner;
    public GameObject SEcorner;
    public GameObject SWcorner;
    public GameObject NWcorner;


    public int maxHallDistance = 10;
    public int roomChance;
    public int endChance;
    public int cornerChance;
    public int threeInterChance;
    public int fourInterChance;

    int[] cardinal = new int[] {1, 2, 3, 4};
    int[] OPcardinal = new int[] {3, 4, 1, 2};

    public int totalGenerations = 0;
    public int sizeLimit1;
    public int sizeLimit2;
    public int sizeLimit3;

    private HashSet<Vector2> occupiedPositions = new HashSet<Vector2>();

    // Start is called before the first frame update
    void Start()
    {
        Debug.LogError("Game Start");

        for (int i = 0; i < cardinal.Length; i++)
        {
            Debug.LogError(cardinal[i]);
        }
        
        
        spawnPrefab(tilePrefab, 0, 0);
        generate_hallway(cardinal[0], 0, 0);
        generate_hallway(cardinal[1], 0, 0);
        generate_hallway(cardinal[2], 0, 0);
        generate_hallway(cardinal[3], 0, 0);
    }

    private void spawnPrefab(GameObject prefab, float posX, float posY)
    {
        Vector2 position = new Vector2(posX, posY);

        if (!occupiedPositions.Contains(position))
        {
            //Debug.LogError("Spawning Prefab");

            GameObject a = Instantiate(prefab) as GameObject;
            a.transform.position = position;
            a.transform.localScale *= tileSize;
            occupiedPositions.Add(position);
        }
    }

    private void generate_hallway(int direction, float posX, float posY)
    {
        //Debug.LogError("Generating Hallway");
        int OPdirection = OPcardinal[direction - 1];
        int distance = Random.Range(3, maxHallDistance);
        int roomRoll = Random.Range(0, 100);
        int JointRoll = Random.Range(0, 100);
        int i = 0;
        int dirX = 0;
        int dirY = 0;
        //bool placeableRoom = true;

        

        if (direction == 1)
        {
            dirX = 0;
            dirY = tileSize;
        }
        else if (direction == 2)
        {
            dirX = tileSize;
            dirY = 0;
        }
        else if (direction == 3)
        {
            dirX = 0;
            dirY = -tileSize;
        }
        else if (direction == 4)
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
                if (direction == 1 || direction == 3)
                {
                    spawnPrefab(vTile, posX, posY);
                }
                else
                {
                    spawnPrefab(hTile, posX, posY);
                }
                
                posX += dirX;
                posY += dirY;
                i++;
            }
        }

        totalGenerations++;
        updateChances();

        
        
        


        if (JointRoll <= endChance) //dead end
        {
            spawnPrefab(tilePrefab, posX, posY);
        }

        else if (JointRoll <= endChance + cornerChance) //corner
        {
            int randEntry1 = GetValidDirection(direction, direction - 1);
            placeCorner(OPdirection, cardinal[randEntry1], posX, posY);
            generate_hallway(cardinal[randEntry1], posX, posY);


            // spawnPrefab(tilePrefab, posX, posY);
            // int randEntry1 = GetValidDirection(direction);
            // generate_hallway(cardinal[randEntry1], posX, posY);
        }

        else if (JointRoll <= (endChance + cornerChance + threeInterChance)) //t intersection
        {
            
            spawnPrefab(tPrefab, posX, posY);

            int randEntry1 = GetValidDirection(direction);
            int randEntry2 = GetValidDirection(direction, randEntry1);


            generate_hallway(cardinal[randEntry1], posX, posY);
            generate_hallway(cardinal[randEntry2], posX, posY);

            
        }
        else if (JointRoll <= (endChance + cornerChance + threeInterChance + fourInterChance)) //4 way intersection
        {
            
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

    private int GetValidDirection(int currentDirection, int? excludedIndex = null)
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

    private void placeCorner(int OPdir, int NEWdir, float posX, float posY)
    {
        int total = OPdir + NEWdir;

        //spawnPrefab(diamondPrefab, posX, posY);

        if (total == 3)
        {
            spawnPrefab(NEcorner, posX, posY);
        }
        else if (total == 5)
        {
            if (OPdir == 1 || OPdir == 4)
            {
                spawnPrefab(NWcorner, posX, posY);
            }
            else
            {
                spawnPrefab(SEcorner, posX, posY);
            }
        }
        else if (total == 7)
        {
            spawnPrefab(SWcorner, posX, posY);
        }
        else
        {
            spawnPrefab(diamondPrefab, posX, posY);
        }
    }
}
